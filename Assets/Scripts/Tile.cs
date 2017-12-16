using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
	public enum Type
	{
		desert,
		plain,
		mountain,
		forest,
	}

	public Type type; // 格子类型
	public bool accessible; // 是否可通行

	public Tile(Type t, bool a)
	{
		type = t;
		accessible = a;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
