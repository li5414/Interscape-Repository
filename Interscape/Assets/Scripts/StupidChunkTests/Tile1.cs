using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile1 {
	public enum Type { grass1, grass2 }
	Type type;

	public int x { get; private set;}
	public int y { get; private set;}
	public int z { get; private set;}

	public Tile1 (int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;

		type = Type.grass1;
	}
}
