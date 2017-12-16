using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {
    private enum Status
    {
        Idle,
        EnterMoving,
        EnterAttacking
    }

    // 玩家控制器当前状态
    private Status status_ = Status.Idle;

    private UnitColumnManager unit_column_manager;

    // 记录当前选择单位
    private Unit current_unit = null;

    // 标识玩家控制器是否被冻结
    private bool is_frozen = false;

    // 滚屏
    private int limit_width;
    private int limit_height;
    public float scroll_speed;

    public Unit GetCurrentUnit()
    {
        return current_unit;
    }

    // 冻结当前控制器
    public void Freeze()
    {
        is_frozen = true;
        unit_column_manager.IsMoveAvailable = false;
    }

    public void UnFreeze()
    {
        is_frozen = false;
        unit_column_manager.IsMoveAvailable = true;
    }

	// Use this for initialization
	/// <summary>
    /// 
    /// </summary>
    void Start () {
        unit_column_manager = GameObject.Find("UnitColumn").GetComponent<UnitColumnManager>();
        
        // 单位栏初始是不可见的
        unit_column_manager.gameObject.SetActive(false);

        // 读地图大小
        GameObject map_manager = GameObject.Find("MapManager");
        limit_width = map_manager.GetComponent<MapManager>().map_width - 1;
        limit_height = map_manager.GetComponent<MapManager>().map_height - 1;

        // 测试：对话
        /*GameObject.Find("Director").GetComponent<Director>().Say("市民", "大家快来看，这里帖出了告示！\n上面说，有一些黄金贼在附近流窜！");
        GameObject.Find("Director").GetComponent<Director>().Say("官兵", "是啊，所以大家晚上千万不要乱跑！");
        GameObject.Find("Director").GetComponent<Director>().Say("市民", "（议论纷纷）");
        GameObject.Find("Director").GetComponent<Director>().Say("刘备", "（超大声）唉！！！！！！\n在这样的乱世里，我一个有着（大声喊）皇族高贵血统的人却无能为力，真是悲哀！\n干脆学点技术，苟且过完下半生吧！");
        GameObject.Find("Director").GetComponent<Director>().Say("市民", "……………………");
        GameObject.Find("Director").GetComponent<Director>().Say("官兵", "……………………");
        GameObject.Find("Director").GetComponent<Director>().Say("草席青年", "……………………");
        GameObject.Find("Director").GetComponent<Director>().Say("草席青年", "大家掏出家伙准备整死他！");
        GameObject.Find("Director").GetComponent<Director>().Say("官兵", "整死他！");
        GameObject.Find("Director").GetComponent<Director>().Say("市民", "整死他！干他妈的！");*/
    }
	
	// Update is called once per frame
	void Update () {
        if (is_frozen)
            return;

        // 滚屏
        Vector3 mouse_pos = Input.mousePosition;
        float scroll_delta = scroll_speed * Time.deltaTime;
        if ((int)(mouse_pos.x) <= 0)
            transform.Translate(new Vector3(-scroll_delta, 0, 0));
        else if ((int)(mouse_pos.x) >= Screen.width - 1)
            transform.Translate(new Vector3(scroll_delta, 0, 0));
        
        if (transform.position.x < 0)
            transform.Translate(new Vector3(0 - transform.position.x, 0, 0));
        else if (transform.position.x > limit_width)
            transform.Translate(new Vector3(limit_width - transform.position.x, 0, 0));
        
        if ((int)(mouse_pos.y) <= 0)
            transform.Translate(new Vector3(0, -scroll_delta, 0));
        else if ((int)(mouse_pos.y) >= Screen.height - 1)
            transform.Translate(new Vector3(0, scroll_delta, 0));

        if (transform.position.y < 0)
            transform.Translate(new Vector3(0, 0 - transform.position.y, 0));
        else if (transform.position.y > limit_height)
            transform.Translate(new Vector3(0, limit_width - transform.position.y, 0));

		if (Input.GetMouseButtonDown(0))
		{
            // 首先判断点击的是否是UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            switch (status_)
            {
                case Status.Idle:
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
                    if (hit.collider != null)
                    {
                        GameObject.Find("DebugLog").GetComponent<Text>().text += "选中物体：" + hit.collider.gameObject.transform.position + "\n";

                        // 判断被选中物体是否为一个单位
                        Unit hit_unit = hit.collider.gameObject.GetComponent<Unit>();
                        if (hit_unit)
                        {
                            GameObject.Find("DebugLog").GetComponent<Text>().text += "激活单位栏\n";
                            current_unit = hit_unit;
                            unit_column_manager.SetSelectedUnit(hit_unit);
                            unit_column_manager.gameObject.SetActive(true);
                        }
                        else
                        {
                            current_unit = null;
                            unit_column_manager.SetSelectedUnit(null);
                            unit_column_manager.gameObject.SetActive(false);
                        }
                    }
                    break;
                }
                case Status.EnterMoving:
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject.tag == "MoveTile")
                    {
                        Vector2 pos = hit.collider.gameObject.transform.position;
                        GameObject.Find("DebugLog").GetComponent<Text>().text += "建立路径：" + pos + "\n";

                        current_unit.MoveBlock((int)(pos.x), (int)(pos.y), 0.1f);

                        GameObject.Find("CommandLayerManager").GetComponent<CommandLayerManager>().Reset();
                        status_ = Status.Idle;
                    }
                    break;
                }
            }
		}
        else if (Input.GetMouseButtonDown(1)) // 右键为取消
        {
            switch (status_)
            {
                case Status.EnterMoving:
                {
                    GameObject.Find("DebugLog").GetComponent<Text>().text += "取消移动准备。\n";
                    GameObject.Find("CommandLayerManager").GetComponent<CommandLayerManager>().Reset();
                    status_ = Status.Idle;
                    unit_column_manager.IsMoveAvailable = true;
                    break;
                }
            }
        }
	}

    // 进入移动准备阶段
    // 点击“移动”按钮触发
    public void EnterMoving()
    {
        if (current_unit == null)
            return;
        if (current_unit.move_ability.now == 0)
            return;

        GameObject.Find("DebugLog").GetComponent<Text>().text += "进入移动准备。\n";
        GameObject.Find("CommandLayerManager").GetComponent<CommandLayerManager>().Generate(CommandLayerManager.CommandType.Move, current_unit);
        status_ = Status.EnterMoving;
        unit_column_manager.IsMoveAvailable = false;
    }
}
