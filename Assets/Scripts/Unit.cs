using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    // 单位类型
    public enum Type
    {
        Infantry, // 步兵
        Cavalry, // 骑兵
        AirForce, // 空军
        Machinery // 机械
    }

    // 信息类型1，用于兵力、士气、移动能力、攻击能力、攻击范围、生命、攻击力、防御力、纪律、伤害次数
    [Serializable]
    public class InfoType1
    {
        public int now; // 当前值
        public int max; // 最大值
        public float max_bonus_percent; // 最大值百分比奖励
        public int max_bouns_value; // 最大值值奖励
    }

    // 信息类型2，用于命中率、格挡率
    [Serializable]
    public class InfoType2
    {
        public float base_value; // 基础
        public float bonus_percent; // 百分比奖励
    }

    // 信息类型3，用于冲锋
    [Serializable]
    public class InfoType3
    {
        public bool ready; // 冲锋就绪

        public float base_atk_def_ratio; // 基础攻防加成
        public float atk_def_ratio_bonus_percent; // 攻防加成百分比奖励

        public int base_morale_strike; // 基础士气打击
        public float morale_strike_bonus_percent; // 士气打击百分比奖励
        public int morale_strike_bonus_value; // 士气打击值奖励
    }

    // 单位行为类型
    public enum BehaviourType
    {
        Move,
        Attack,
        OnAttack
    }

    [Serializable]
    public class Behaviour
    {
        public BehaviourType behaviour_type;
        public float duration; // 行为时长
        public Vector2 target_pos; // 目标点
        public GameObject target_object; // 目标单位
        public int skill_id; // 技能id

        public Behaviour(BehaviourType t, float d, Vector2 p, GameObject o = null, int s = 0)
        {
            behaviour_type = t;
            duration = d;
            target_pos = p;
            target_object = o;
            skill_id = s;
        }
    }

    public string unit_name; // 单位名称
    public Type type; // 单位类型
    // 势力，0代表玩家，1代表友军，2代表敌军
    public int force;
    public InfoType1 army; // 兵力
    public InfoType1 morale; // 士气
    public InfoType1 move_ability; // 移动能力
    public InfoType1 attack_ability; // 攻击能力
    public InfoType1 attack_range; // 攻击范围
    public InfoType1 life; // 生命
    public InfoType1 attack; // 攻击力
    public InfoType1 defense; // 防御力
    public InfoType1 discipline; // 纪律
    public InfoType1 hurt_count; // 伤害次数
    public InfoType2 hit_ratio; // 命中率
    public InfoType2 dodge_ratio; // 格挡率
    public InfoType3 assault; // 冲锋

    // 技能列表，以id作为索引
    private SortedList<int, Skill> skills_;
    // 将领
    private General general_;

    // 行为队列
    private Queue<Behaviour> behaviour_queue = new Queue<Behaviour>();

    private struct IntVector2
    {
        public int x;
        public int y;
        public IntVector2(int px, int py)
        {
            x = px;
            y = py;
        }
    }

    // 存储路径的前驱，最短距离
    private IntVector2[,] path_parent = null;
    private int[,] path_distance = null;

    // 计算可移动路径，返回可移动位置集
    // 每个单位移动方式可能不一样
    // 初期使用广搜解决
    public List<Vector2> CalculateMovePath()
    {
        int ma = move_ability.now;
        if (ma == 0)
            return null;
        
        int sx = (int)(transform.position.x);
        int sy = (int)(transform.position.y);
        
        MapManager mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        if (path_parent == null)
        {
            path_parent = new IntVector2[mm.map_width, mm.map_height];
            for (int i = 0; i < mm.map_width; i++)
                for (int j = 0; j < mm.map_height; j++)
                    path_parent[i, j] = new IntVector2(-1, -1);
        }
        else
        {
            for (int i = 0; i < mm.map_width; i++)
                for (int j = 0; j < mm.map_height; j++)
                    path_parent[i, j].x = path_parent[i, j].y = -1;
        }
        if (path_distance == null)
        {
            path_distance = new int[mm.map_width, mm.map_height];
        }
        int max_distance = mm.map_width + mm.map_height + 1;
        for (int i = 0; i < mm.map_width; i++)
            for (int j = 0; j < mm.map_height; j++)
                path_distance[i, j] = max_distance;

        bool[,] is_in_queue = new bool[mm.map_width, mm.map_height];
        for (int i = 0; i < mm.map_width; i++)
            for (int j = 0; j < mm.map_height; j++)
                is_in_queue[i, j] = false;

        path_distance[sx, sy] = 0;
        Queue<IntVector2> path_queue = new Queue<IntVector2>();
        path_queue.Enqueue(new IntVector2(sx, sy));
        is_in_queue[sx, sy] = true;

        int[] dir_x = {0, 1, 0, -1};
        int[] dir_y = {1, 0, -1, 0};

        // 开始广搜
        while (path_queue.Count > 0)
        {
            IntVector2 head = path_queue.Dequeue();
            is_in_queue[head.x, head.y] = false;
            if (path_distance[head.x, head.y] != ma)
            {
                for (int i = 0; i < 4; i++)
                {
                    int new_x = head.x + dir_x[i];
                    int new_y = head.y + dir_y[i];

                    if (new_x < 0 || new_x >= mm.map_width)
                        continue;
                    if (new_y < 0 || new_y >= mm.map_height)
                        continue;
                    if (!mm.IsAccessible(new_x, new_y))
                        continue;

                    if (path_distance[new_x, new_y] > path_distance[head.x, head.y] + 1)
                    {
                        path_distance[new_x, new_y] = path_distance[head.x, head.y] + 1;
                        path_parent[new_x, new_y] = head;
                        if (!is_in_queue[new_x, new_y])
                        {
                            path_queue.Enqueue(new IntVector2(new_x, new_y));
                            is_in_queue[new_x, new_y] = true;
                        }
                    }
                }
            }
        }

        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < mm.map_width; i++)
        {
            for (int j = 0; j < mm.map_height; j++)
            {
                if (i == sx && j == sy)
                    continue;
                if (path_distance[i, j] <= ma)
                {
                    result.Add(new Vector2(i, j));
                }
            }
        }
        return result;
    }

    void Start()
    {

    }

    void Update()
    {
        // 处理行为队列
        if (behaviour_queue.Count > 0)
        {
            Behaviour b_now = behaviour_queue.Peek();
            switch (b_now.behaviour_type)
            {
                case BehaviourType.Move:
                {
                    float delta_time = Time.deltaTime;
                    if (delta_time >= b_now.duration)
                    {
                        transform.position = b_now.target_pos;
                        // 移动完毕，弹出行为
                        behaviour_queue.Dequeue();
                        if (behaviour_queue.Count == 0)
                            GameObject.Find("GameController").GetComponent<GameController>().ExitWork();
                    }
                    else
                    {
                        // 向目标点移动
                        float delta_x = b_now.target_pos.x - transform.position.x;
                        float delta_y = b_now.target_pos.y - transform.position.y;
                        delta_x *= delta_time / b_now.duration;
                        delta_y *= delta_time / b_now.duration;
                        transform.Translate(delta_x, delta_y, 0f);
                        b_now.duration -= delta_time;

                        // 测试：每次移动减少5兵力
                        // army.now -= 5;
                    }
                    break;
                }
            }
        }
    }

    // 确认单位是否就绪
    bool Ready()
    {
        return behaviour_queue.Count == 0;
    }

    // 令单位花费指定时间移动至坐标处
    public void Move(Vector2 target, float time)
    {
        behaviour_queue.Enqueue(new Behaviour(BehaviourType.Move, time, target, null, 0));
    }

    // 按地图块移动
    // 需要使用寻路算法
    // time为每移动一块所需时间
    public void MoveBlock(int x, int y, float time)
    {
        Stack<Vector2> s = new Stack<Vector2>();
        int sx = (int)(transform.position.x);
        int sy = (int)(transform.position.y);

        while (x != sx || y != sy)
        {
            s.Push(new Vector2(x, y));
            int px = path_parent[x, y].x;
            int py = path_parent[x, y].y;
            x = px;
            y = py;
        }

        while (s.Count > 0)
        {
            GameObject.Find("DebugLog").GetComponent<Text>().text += "路径：" + s.Peek() + "\n";
            behaviour_queue.Enqueue(new Behaviour(BehaviourType.Move, time, s.Pop()));
        }

        GameObject.Find("GameController").GetComponent<GameController>().EnterWork();
    }
}
