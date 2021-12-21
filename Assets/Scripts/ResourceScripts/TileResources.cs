using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileResources {

	public Tile tileGrass = Resources.Load<Tile> ("Tiles/Tile_Grass");
	public Tile tileGrassBig = Resources.Load<Tile> ("Tiles/TileGrassLarge");
	public RuleTile tileSandRule = Resources.Load<RuleTile> ("Tiles/Sand_Rule");
	public Tile tileSand = Resources.Load<Tile> ("Tiles/Tile_Sand");
	public Tile plainChunk = Resources.Load<Tile> ("Tiles/TilePlainChunk");
	public Tile tileWater1 = Resources.Load<Tile> ("Tiles/Water_0");
	public Tile tileWater2 = Resources.Load<Tile> ("Tiles/Water_1");
	public Tile tileWater3 = Resources.Load<Tile> ("Tiles/Water_2");
	public Tile tileWater4 = Resources.Load<Tile> ("Tiles/Water_3");


	// grass details
	// public Sprite [] grassDetails = Resources.LoadAll<Sprite>("Tiles/GrassBlades");
	public Tile [] grassDetailsChunk;

	public TileResources ()
	{
		grassDetailsChunk = new Tile [8];
		for (int i = 0; i < 8; i ++) {
			grassDetailsChunk [i] = ScriptableObject.CreateInstance<Tile> ();
			grassDetailsChunk [i].sprite = Resources.Load<Sprite> ("Tiles/GrassBlades/GrassChunk" + i.ToString());
		}
	}

}
