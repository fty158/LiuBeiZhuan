using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
	public int map_width;
	public int map_height;

	private Tile[,] tiles;

	// Use this for initialization
	void Start () {
		tiles = new Tile[map_width, map_height];
		foreach (Transform child in transform)
		{
			int x = (int)(child.position.x);
			int y = (int)(child.position.y);
			tiles[x, y] = child.GetComponent<Tile>();
		}
	}

	public bool IsAccessible(int x, int y)
	{
        if (tiles[x, y] != null)
            return tiles[x, y].accessible;
        else
            return true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
