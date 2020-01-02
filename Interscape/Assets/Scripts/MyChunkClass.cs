using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;


public class MyChunkClass
{
	// components
	public static int chunkSize = 16;
	Tilemap tilemap;
	TilemapRenderer trender;
	Tilemap detailTilemap;
	TilemapRenderer drender;

	// arrays
	Vector3Int [] tilePositions;
	Tile [] tileArray;
	Vector3Int [] deetPositions;
	Tile [] deetArray;
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	GameObject [,] entities;
	MyChunkSystem.BiomeType [,] biomes;
	GameObject treeParent;

	// tiles
	Tile tile1 = Resources.Load<Tile> ("Sprites/Map/Tiles/TileBase_1");
	Tile tile0 = Resources.Load<Tile> ("Sprites/Map/Tiles/TileBase_0");

	// grass details
	Tile detail1 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_1");
	Tile detail2 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_2");
	Tile detail3 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_3");
	Tile detail4 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_4");
	Tile detail5 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_5");

	

	// reference other script
	MyChunkSystem sys = GameObject.Find ("System Placeholder").GetComponent<MyChunkSystem> ();
	GreeneryGeneration gen = GameObject.Find ("System Placeholder").GetComponent<GreeneryGeneration> ();
	GameObject TreeParent = GameObject.Find ("TreeParent");

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public MyChunkClass (Vector3Int pos, int seed, Tilemap tilemapObj)
	{
		// create rand num generator from seed to use throughout
		System.Random prng = MyChunkSystem.prng;
		treeParent = new GameObject();
		treeParent.transform.SetParent (TreeParent.gameObject.transform);

		// initialise arrays
		heights = sys.GetHeightValues (pos.x, pos.y);
		temps = sys.GetTemperatures (pos.x, pos.y, heights);
		humidities = sys.GetHumidityArray (pos.x, pos.y, heights);
		biomes = sys.GetBiomes (heights, temps, humidities);

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
		//tilemap.SetTiles (tilePositions, tileArray); // can't find a way to use settiles while keeping each colour seperate

		// generate grass details
		GenerateDetails (prng);
		//detailTilemap.SetTiles (deetPositions, deetArray);

		// *list* of gameobject NEEDS IMPLEMENTING
		entities = gen.GeneratePlants (prng, pos, biomes, treeParent);
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateTiles (System.Random prng, Vector3Int chunkPos) {
		int size = chunkSize * chunkSize;
		tilePositions = new Vector3Int [size];
		tileArray = new Tile [size];
		float [] distances = new float [MyChunkSystem.specialCoords.Length];
		float randNum;
		float heightVal;
		float temp;
		float humidity;
		MyChunkSystem.BiomeType biome;
		Vector2 biomePos;
		Color32 secondary;
		int rDiff;
		int gDiff;
		int bDiff;

		// in loop, enter all positions and tiles in arrays
		for (int index = 0; index < size; index++) {
			randNum = prng.Next (0, 10);
			tilePositions [index] = new Vector3Int (index % chunkSize, index / chunkSize, 200);
			//tileArray [index] = randNum < 5 ? tile1 : tile2;

			heightVal = heights[index % chunkSize, index / chunkSize];
			temp = temps[index % chunkSize, index / chunkSize];
			humidity = humidities [index % chunkSize, index / chunkSize];
			biome = biomes [index % chunkSize, index / chunkSize];
			Color32 tileColor = new Color32 (117, 173, 141, 255);

			// set tile sprite
			tileArray [index] = tile1;
			
			if (biome != MyChunkSystem.BiomeType.Beach && biome != MyChunkSystem.BiomeType.Water && biome != MyChunkSystem.BiomeType.DeepWater
				&& biome != MyChunkSystem.BiomeType.Ice) {
				// get temp in range for lookup (assuming the max and min possible temperatures)
				temp = Mathf.InverseLerp (-90f, 90f, temp);
				temp *= MyChunkSystem.tableSize;
				biomePos = new Vector2 (humidity, temp);

				for (int i = 0; i < MyChunkSystem.specialCoords.Length; i++) {
					distances [i] = Vector2.Distance (MyChunkSystem.specialCoords [i], biomePos);

					// normalise distance to 0-1
					distances [i] = Mathf.InverseLerp (0f, Mathf.Sqrt(MyChunkSystem.tableSize * MyChunkSystem.tableSize +
						MyChunkSystem.tableSize * MyChunkSystem.tableSize), distances [i]); // diagonal length of array

					// only take into account colours within a certain range
					if (distances [i] < 0.3) {

						// get biome colour
						secondary = MyChunkSystem.BiomeColours [MyChunkSystem.BiomeTable [MyChunkSystem.specialCoords [i].x, MyChunkSystem.specialCoords [i].y]];

						// weight distance even further to create more drastic changes
						float weighting = 1 - Mathf.InverseLerp (0f, 0.3f, distances [i]);

						//get difference and multiply by weighting value
						rDiff = (int)((secondary.r - tileColor.r) * weighting);
						gDiff = (int)((secondary.g - tileColor.g) * weighting);
						bDiff = (int)((secondary.b - tileColor.b) * weighting);

						// grr need to account for negatives
						if (rDiff > 0)
							tileColor.r += (byte)rDiff;
						else {
							tileColor.r -= (byte)(Mathf.Abs (rDiff));
						}
						if (gDiff > 0)
							tileColor.g += (byte)gDiff;
						else
							tileColor.g -= (byte) (Mathf.Abs (gDiff));
						if (bDiff > 0)
							tileColor.b += (byte)bDiff;
						else
							tileColor.b -= (byte) (Mathf.Abs (bDiff));

					}

					//putting caps on the values
					if (tileColor.r < 40)
						tileColor.r = 40;
					if (tileColor.g < 40)
						tileColor.g = 40;
					if (tileColor.b < 40)
						tileColor.b = 40;

					if (tileColor.r > 230)
						tileColor.r = 230;
					if (tileColor.g > 230)
						tileColor.g = 230;
					if (tileColor.b > 230)
						tileColor.b = 230;

					/*if (Random.value < 0.001) {
						Debug.Log(tileColor.r);
					}*/
				}
			}

			// ice biome special case
			else if (biome == MyChunkSystem.BiomeType.Ice) {
				tileColor = MyChunkSystem.BiomeColours [biome];
				heightVal = Mathf.InverseLerp (-10f, 1f, heightVal);
				tileColor.r = (byte)(tileColor.r * heightVal * 0.95);
				tileColor.g = (byte)(tileColor.g * heightVal * 0.95);
				tileColor.b = (byte)(tileColor.b * heightVal);
			}

			else {
				tileColor = MyChunkSystem.BiomeColours [biome];
			}

			// set tile colour according to biome
			tileArray [index].color = tileColor;
			tilemap.SetTileFlags (tilePositions [index], TileFlags.None);
			tilemap.SetColor (tilePositions [index], tileColor);
			tilemap.SetTile (tilePositions [index], tileArray [index]);
		}
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */


	// - NEED TO FIX FLOWERS SPAWNING IN EXACT SAME SPOT EACH CHUNK
	// - NEED TO FIX COLOURS NOT SETTING CORRECTLY
	public void GenerateDetails (System.Random prng)
	{
		int size = chunkSize * chunkSize * 16; // 4x4 = 16
		deetPositions = new Vector3Int [size];
		deetArray = new Tile [size];
		int randNum;
		MyChunkSystem.BiomeType biome;
		Color32 tileColor = new Color32 (0, 0, 0, 255);
		bool isNotNull;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < size; i++) {
			biome = biomes [Mathf.FloorToInt(i % (chunkSize * 4))/4, Mathf.FloorToInt(i / (chunkSize * 4))/4];
			isNotNull = true;

			// temporary grass spawning system
			if ((biome !=  MyChunkSystem.BiomeType.Ice && biome != MyChunkSystem.BiomeType.Water
				&& biome != MyChunkSystem.BiomeType.Desert && biome != MyChunkSystem.BiomeType.Beach)
				&& biome != MyChunkSystem.BiomeType.DeepWater) {

				randNum = prng.Next (0, 15);
				deetPositions [i] = new Vector3Int (i % (chunkSize * 4), i / (chunkSize * 4), 199);

				// generate grass details using random numbers
				if (randNum == 1)
					deetArray [i] = detail1;
				else if (randNum == 2)
					deetArray [i] = detail2;
				else if (randNum == 3)
					deetArray [i] = detail4;
				else if (randNum == 4)
					deetArray [i] = detail5;

				// flower generation less likely
				else if (randNum == 5 & prng.Next (0, 15) == 1)
					deetArray [i] = detail3;
				else
					isNotNull = false;

				// set colour according to biome
				if (isNotNull == true && deetArray [i] != detail3) {
					//tileColor = MyChunkSystem.BiomeColours [biome];
					tileColor = tileArray [Mathf.FloorToInt(i / 16f)].color;
					tileColor.b -= 5; // adjust colour slightly for grass

					deetArray [i].color = tileColor;
					detailTilemap.SetTileFlags (deetPositions [i], TileFlags.None);
					detailTilemap.SetColor (deetPositions [i], tileColor);
				}

				detailTilemap.SetTile (deetPositions [i], deetArray [i]);
			}
		}
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void SetVisible (bool visible)
	{
		trender.enabled = visible;
		drender.enabled = visible;
		treeParent.SetActive (visible);
	}

	public bool IsVisible ()
	{
		return trender.enabled;
	}
}

