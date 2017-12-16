using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {
    private int id_; // 技能id
    private bool avaliable_; // 技能是否可用
    private int cool_down; // 冷却回合
    private int current_cool_down; // 当前冷却回合
}
