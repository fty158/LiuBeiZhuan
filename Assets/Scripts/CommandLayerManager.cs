using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于管理控制层显示
// 包括移动范围，攻击范围，技能范围的显示与限制
public class CommandLayerManager : MonoBehaviour {
	public enum CommandType
	{
		Move,
		Attack,
		Skill
	}

	public GameObject move_tile;

	private List<GameObject> command_tiles = new List<GameObject>();

	public void Generate(CommandType t, Unit u)
	{
		switch (t)
		{
			case CommandType.Move:
			{
				List<Vector2> path = u.CalculateMovePath();
				if (path == null)
					break;
				
				foreach (Vector2 v in path)
				{
					// 创建控制格
					GameObject tmp_tile = Instantiate(move_tile);
					tmp_tile.transform.position = v;
					tmp_tile.transform.parent = transform;
					command_tiles.Add(tmp_tile);
				}
				break;
			}
		}
	}

	public void Reset()
	{
		foreach (GameObject ct in command_tiles)
		{
			Destroy(ct);
		}
		command_tiles.Clear();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
