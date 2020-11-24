using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileResources {

	public Tile tileGrass = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Grass");
	public RuleTile tileSandRule = Resources.Load<RuleTile> ("Sprites/Map/Tiles/Sand_Rule");
	public Tile tileSand = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Sand");
	public Tile tileWater1 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_0");
	public Tile tileWater2 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_1");
	public Tile tileWater3 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_2");
	public Tile tileWater4 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_3");


	// grass details
	public Tile [] grassDetails = { Resources.Load<Tile> ("Sprites/Map/Tiles/detail_1"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_2"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_4"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_5"),
											Resources.Load<Tile> ("Sprites/Map/Tiles/detail_3")
											};


}
