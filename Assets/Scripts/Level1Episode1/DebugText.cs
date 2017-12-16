using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugText : MonoBehaviour {
	private string test = @"王朗：来者可是诸葛孔明?
诸葛亮：正是。
王朗：久闻公之大名，今日有幸相会！
诸葛亮：……
王朗：公既知天命，识时务，为何要兴无名之师，犯我疆界？
诸葛亮：我奉诏讨贼，何谓之无名？
王朗：哈哈哈哈哈哈，哈，哈哈哈。
王朗：天数有变，神器更易，而归有德之人，此乃自然之理。
诸葛亮：曹贼篡汉，霸占中原，何称有德之人？
王朗：自桓帝、灵帝以来，黄巾猖獗，天下纷争，社稷累卵之危，生灵有倒悬之急。
王朗：我太祖武皇帝，扫清六合，席卷八荒，万姓倾心，四方仰德；此非以权势取之，实乃天命所归也！
王朗：我世祖文皇帝，神文圣武，继承大统，应天合人，法尧禅舜，处中国以治万邦，这岂非天心人意乎？
王朗：今公蕴大才，抱大器，自比管仲、乐毅，何乃强要逆天理，背人情而行事？
王朗：岂不闻古人云：顺天者昌，逆天者亡？
王朗：今我大魏带甲百万，良将千员。谅尔等腐草之萤光，何比天空之皓月？
王朗：你若倒戈卸甲，以礼来降，仍不失封侯之位，国安民乐，岂不美哉？
诸葛亮：哈，哈哈哈哈哈哈。
诸葛亮：我操你妈逼！";
	private int len = 0;
	private float accu_delta_time = 0f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (len < test.Length)
		{
			accu_delta_time += Time.deltaTime;
			if (accu_delta_time >= 0.02f)
			{
				len++;
				GetComponent<Text>().text = test.Substring(0, len);
				accu_delta_time -= 0.02f;
			}
		}
		else
		{
			SceneManager.LoadScene("level1act1");
		}
	}
}
