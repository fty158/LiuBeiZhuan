using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	// 当前忙碌游戏对象的数量
	private int busy_count;

	public PlayerController pc;

	public void EnterWork()
	{
		busy_count++;
		pc.Freeze();
	}

	public void ExitWork()
	{
		busy_count--;
		if (busy_count == 0)
			pc.UnFreeze();
	}

	// Use this for initialization
	void Start () {
		busy_count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
