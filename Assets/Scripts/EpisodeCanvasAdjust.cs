using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeCanvasAdjust : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject debug_log_panel = transform.Find("DebugLogPanel").gameObject;
        debug_log_panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(95 - Screen.width / 2, 0);
        debug_log_panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
        GameObject debug_log_scroll_bar = transform.Find("DebugLogScrollBar").gameObject;
        debug_log_scroll_bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(195 - Screen.width / 2, 0);
        debug_log_scroll_bar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

        // 将控制面板附着在屏幕正下方
        GameObject console_board = transform.Find("ConsoleBoard").gameObject;
        console_board.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50 - Screen.height / 2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
