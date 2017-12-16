using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoardManager : MonoBehaviour {
	private Image char_avator;
	private Text char_name;
	private Text char_say;

	public void Show(string cn)
	{
		gameObject.SetActive(true);
		char_name.text = cn;
		char_say.text = "";
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public bool IsActive()
	{
		return gameObject.activeSelf;
	}

	public void AppendSay(string s)
	{
		char_say.text += s;
	}

	public void AppendSay(char c)
	{
		char_say.text += c;
	}

	// Use this for initialization
	void Start () {
		char_avator = transform.Find("CharAvator").gameObject.GetComponent<Image>();
		char_name = transform.Find("CharName").gameObject.GetComponent<Text>();
		char_say = transform.Find("CharSay").gameObject.GetComponent<Text>();

		char_name.text = "";
		char_say.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
