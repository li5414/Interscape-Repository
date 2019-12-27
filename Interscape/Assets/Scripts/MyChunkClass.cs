using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;


public class MyChunkClass
{
	public enum BiomeType {
		Desert,
		Savanna,
		TropicalRainforest,
		Grassland,
		Woodland,
		SeasonalForest,
		TemperateRainforest,
		BorealForest,
		Tundra,
		Ice
	}

	// components
	public static int chunkSize = 16;
	Tilemap tilemap;
	TilemapRenderer trender;
	Tilemap detailTilemap;
	TilemapRenderer drender;

	// arrays
	Vector3Int [] tilePositions;
	TileBase [] tileArray;
	Vector3Int [] deetPositions;
	TileBase [] deetArray;

	// reference other script
	MyChunkSystem sys = GameObject.Find ("System Placeholder").GetComponent<MyChunkSystem>();
	

	// tiles
	Tile tileA = Resources.Load<Tile> ("Sprites/Map/Tiles/TileBase_0");
	Tile tileB = Resources.Load<Tile> ("Sprites/Map/Tiles/TileBase_1");
	Tile tileC = Resources.Load<Tile> ("Sprites/Map/Tiles/TileBase_2");
	Tile tileD = Resources.Load<Tile> ("Sprites/Map/Tiles/TileBase_3");

	// grass details
	Tile detail1 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_1");
	Tile detail2 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_2");
	Tile detail3 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_3");
	Tile detail4 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_4");
	Tile detail5 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_5");


	public MyChunkClass (Vector3Int pos, Transform parent, int seed,
		Tilemap tilemapObj)
	{
		// create rand num generator from seed to use throughout
		System.Random prng = new System.Random (seed);

		// creates biome array
		BiomeType [,] biomes = new BiomeType [chunkSize, chunkSize];

		// set up tilemap components
		var grid = GameObject.FindGameObjectsWithTag ("Grid")[0];
		tilemap = Object.Instantiate (tilemapObj, pos, Quaternion.identity);
		trender = tilemap.GetComponent<TilemapRenderer> ();
		tilemap.transform.SetParent (grid.gameObject.transform);

		// set up detail tilemap components
		var detailGrid = GameObject.FindGameObjectsWithTag ("Grid")[1];
		Vector3Int position = new Vector3Int (pos.x, pos.y, 199); // move further forward
		detailTilemap = Object.Instantiate (tilemapObj, pos, Quaternion.identity);
		drender = detailTilemap.GetComponent<TilemapRenderer> ();
		detailTilemap.transform.SetParent (detailGrid.gameObject.transform);

		// creates and sets tiles in tilearray to positions in position array
		GenerateTiles (prng, pos);
		tilemap.SetTiles (tilePositions, tileArray);

		

		// generate grass details
		GenerateDetails (prng);
		detailTilemap.SetTiles (deetPositions, deetArray);

		// *list* of gameobject NEEDS IMPLEMENTING
		GameObject [,] entities = new GameObject [chunkSize, chunkSize];
	}

	public void GenerateTiles (System.Random prng, Vector3Int chunkPos) {
		int size = chunkSize * chunkSize;
		tilePositions = new Vector3Int [size];
		tileArray = new Tile [size];
		float randNum;
		float heightVal;
		float temp;

		// in loop, enter all positions and tiles in arrays
		for (int index = 0; index < size; index++) {
			randNum = prng.Next (0, 10);
			tilePositions [index] = new Vector3Int (index % chunkSize, index / chunkSize, 200);
			tileArray [index] = randNum < 5 ? tileA : tileB;

			heightVal = sys.GetHeightValue ((index % chunkSize) + chunkPos.x, (index / chunkSize) + chunkPos.y, 361.4f, 0.5f, 2.5f);
			temp = sys.GetTemperature ((index % chunkSize) + chunkPos.x, (index / chunkSize) + chunkPos.y, 200, heightVal);
			Debug.Log ("Temp:" + temp);

			if (heightVal < -0.1) {
				tileArray [index] = tileD;
			}
			/*else if (heightVal > 0) {
				tileArray [index] = tileC;
			}*/
		}
	}

	public void GenerateDetails (System.Random prng)
	{
		int size = chunkSize * chunkSize * 16; // 4x4 = 16
		deetPositions = new Vector3Int [size];
		deetArray = new Tile [size];
		int randNum;

		// in loop, enter all positions and tiles in arrays, skipping null spots
		for (int i = 0; i < size; i++) {
			randNum = prng.Next (0, 15);
			deetPositions [i] = new Vector3Int (i % (chunkSize*4), i / (chunkSize*4), 199);

			// generate grass details
			if (randNum == 1)
				deetArray [i] = detail1;
			else if (randNum == 2)
				deetArray [i] = detail2;
			else if (randNum == 3)
				deetArray [i] = detail4;
			else if (randNum == 4)
				deetArray [i] = detail5;

			// flower generation less likely
			else if (randNum == 5 & prng.Next(0, 15) == 1)
				deetArray [i] = detail3;
		}
	}

	public void SetVisible (bool visible)
	{
		trender.enabled = visible;
		drender.enabled = visible;
	}

	public bool IsVisible ()
	{
		return trender.enabled;
	}
}

