using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour {

	// 单位行为类型
    private enum BehaviourType
    {
        MoveCamera,
        Say
    }

    [Serializable]
    private class MoveCameraBehaviour
    {
        public float duration; // 镜头移动时长
        public Vector2 target_pos; // 目标点

        public MoveCameraBehaviour(float d, Vector2 p)
        {
            duration = d;
            target_pos = p;
        }
    }

    [Serializable]
    private class SayBehaviour
    {
    	public string who; // 说话人姓名
    	public string content; // 说话内容
    	public float span; // 每个字出现的间隔
    	public bool wait_click; // 说完后是否等待点击

    	public int content_index;
    	public float span_now;

    	public SayBehaviour(string w, string c, float s, bool k)
    	{
    		who = w;
    		content = c;
    		span = s;
    		wait_click = k;

    		content_index = 0;
    		span_now = 0;
    	}
    }

    // 行为总接口
    [Serializable]
    private class Behaviour
    {
        public BehaviourType b_type;
        public object b_body;

        public Behaviour(BehaviourType t, object o)
        {
            b_type = t;
            b_body = o;
        }
    }

	// 行为队列
    private Queue<Behaviour> behaviour_queue = new Queue<Behaviour>();

    public Camera main_camera;
    public DialogBoardManager dialog_board_manager;

    // 注册镜头移动事件
    public void MoveCamera(float d, Vector2 p)
    {
    	if (behaviour_queue.Count == 0)
    	{
    		GameObject.Find("GameController").GetComponent<GameController>().EnterWork();
    	}
    	behaviour_queue.Enqueue(new Behaviour(BehaviourType.MoveCamera, new MoveCameraBehaviour(d, p) as object));
    }

    // 让某人说话
    public void Say(string w, string c, float s = 0.1f, bool k = true)
    {
    	if (behaviour_queue.Count == 0)
    	{
    		GameObject.Find("GameController").GetComponent<GameController>().EnterWork();
    	}
    	behaviour_queue.Enqueue(new Behaviour(BehaviourType.Say, new SayBehaviour(w, c, s, k) as object));
    }

	// Use this for initialization
	void Start () {
		dialog_board_manager.Hide();
	}
	
	// Update is called once per frame
	void Update () {
		// 处理行为队列
        if (behaviour_queue.Count > 0)
        {
            Behaviour b_now = behaviour_queue.Peek();
            switch (b_now.b_type)
            {
                case BehaviourType.MoveCamera:
                {
                	MoveCameraBehaviour b = b_now.b_body as MoveCameraBehaviour;
                    float delta_time = Time.deltaTime;
                    if (delta_time >= b.duration)
                    {
                        main_camera.transform.position = new Vector3(b.target_pos.x, b.target_pos.y, main_camera.transform.position.z);
                        // 移动完毕，弹出行为
                        behaviour_queue.Dequeue();
                        if (behaviour_queue.Count == 0)
                            GameObject.Find("GameController").GetComponent<GameController>().ExitWork();
                    }
                    else
                    {
                        // 向目标点移动
                        float delta_x = b.target_pos.x - main_camera.transform.position.x;
                        float delta_y = b.target_pos.y - main_camera.transform.position.y;
                        delta_x *= delta_time / b.duration;
                        delta_y *= delta_time / b.duration;
                        main_camera.transform.Translate(delta_x, delta_y, 0f);
                        b.duration -= delta_time;
                    }
                    break;
                }
                case BehaviourType.Say:
                {
                	SayBehaviour b = b_now.b_body as SayBehaviour;

                	// 首先显示对话框
                	if (!dialog_board_manager.IsActive())
                	{
                		dialog_board_manager.Show(b.who);
                	}
                	if (b.content_index == b.content.Length)
                	{
                		if (b.wait_click)
                		{
                			if (Input.GetMouseButtonDown(0))
                			{
                				behaviour_queue.Dequeue();
                				dialog_board_manager.Hide();
                        		if (behaviour_queue.Count == 0)
                            		GameObject.Find("GameController").GetComponent<GameController>().ExitWork();
                			}
                		}
                		else
                		{
                			behaviour_queue.Dequeue();
            				dialog_board_manager.Hide();
                    		if (behaviour_queue.Count == 0)
                        		GameObject.Find("GameController").GetComponent<GameController>().ExitWork();
                		}
                	}
                	else
                	{
                		b.span_now += Time.deltaTime;
                		if (b.span_now >= b.span)
                		{
                			b.span_now -= b.span;
                			dialog_board_manager.AppendSay(b.content[b.content_index]);
                			b.content_index++;
                		}
                	}
                	break;
                }
            }
        }
	}
}
