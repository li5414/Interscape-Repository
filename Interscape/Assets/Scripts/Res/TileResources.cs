using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileResources {

	public static Tile tileGrass = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Grass");
	public static RuleTile tileSandRule = Resources.Load<RuleTile> ("Sprites/Map/Tiles/Sand_Rule");
	public static Tile tileSand = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Sand");
	public static Tile tileWater1 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_0");
	public static Tile tileWater2 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_1");
	public static Tile tileWater3 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_2");
	public static Tile tileWater4 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_3");


	// grass details
	public static Tile [] grassDetails = { Resources.Load<Tile> ("Sprites/Map/Tiles/detail_1"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_2"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_4"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_5"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_3")
											};


}
