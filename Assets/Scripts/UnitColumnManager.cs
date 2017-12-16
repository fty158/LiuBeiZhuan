using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitColumnManager : MonoBehaviour {
	private Text unit_name_text;
	private Text unit_army_text;
	private Text unit_morale_text;
	private Text unit_attack_text;
	private Text unit_defense_text;
	private Button unit_move_button;

	private Unit selected_unit = null;

	public void SetSelectedUnit(Unit u)
	{
		selected_unit = u;
	}

	private bool is_move_available;

	public bool IsMoveAvailable
	{
		set
		{
			is_move_available = value;
		}
	}

	// Use this for initialization
	void Start () {
		unit_name_text = transform.Find("UnitName").gameObject.GetComponent<Text>();
		unit_army_text = transform.Find("UnitArmy").gameObject.GetComponent<Text>();
		unit_morale_text = transform.Find("UnitMorale").gameObject.GetComponent<Text>();
		unit_attack_text = transform.Find("UnitAttack").gameObject.GetComponent<Text>();
		unit_defense_text = transform.Find("UnitDefense").gameObject.GetComponent<Text>();
		unit_move_button = transform.Find("UnitMoveButton").gameObject.GetComponent<Button>();
		is_move_available = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (selected_unit)
		{
			unit_name_text.text = selected_unit.unit_name;
			unit_army_text.text = "兵力 " + selected_unit.army.now.ToString() + "/" + selected_unit.army.max.ToString();
            unit_morale_text.text = "士气 " + selected_unit.morale.now.ToString() + "/" + selected_unit.morale.max.ToString();
            unit_attack_text.text = "攻击力 " + selected_unit.attack.max.ToString();
            unit_defense_text.text = "防御力 " + selected_unit.defense.max.ToString();

            if (!is_move_available || selected_unit.move_ability.now == 0)
            {
            	unit_move_button.interactable = false;
            }
            else
            {
            	unit_move_button.interactable = true;
            }
        }
	}
}
