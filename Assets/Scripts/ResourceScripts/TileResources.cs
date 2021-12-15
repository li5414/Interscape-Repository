using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileResources {

	public Tile tileGrass = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Grass");
	public Tile tileGrassBig = Resources.Load<Tile> ("Sprites/Map/Tiles/TileGrassLarge");
	public RuleTile tileSandRule = Resources.Load<RuleTile> ("Sprites/Map/Tiles/Sand_Rule");
	public Tile tileSand = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Sand");
	public Tile plainChunk = Resources.Load<Tile> ("Sprites/Map/Tiles/TilePlainChunk");
	public Tile tileWater1 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_0");
	public Tile tileWater2 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_1");
	public Tile tileWater3 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_2");
	public Tile tileWater4 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_3");


	// grass details
	public Sprite [] grassDetails = Resources.LoadAll<Sprite>("Sprites/Map/Greenery/GrassBlades");
	public Tile [] grassDetailsChunk;

	public TileResources ()
	{
		grassDetailsChunk = new Tile [8];
		for (int i = 0; i < 8; i ++) {
			grassDetailsChunk [i] = ScriptableObject.CreateInstance<Tile> ();
			grassDetailsChunk [i].sprite = Resources.Load<Sprite> ("Sprites/Map/Tiles/GrassBlades/GrassChunk" + i.ToString());
		}
	}

}
