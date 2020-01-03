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
	Tilemap detailTilemap;
	TilemapRenderer trender;
	TilemapRenderer drender;
	GameObject treeParent;       // parent for this particular chunk

	// arrays
	Vector3Int [] tilePositions; // probably dont need?
	Vector3Int [] deetPositions; // probably dont need?
	Tile [] tileArray; // can make 2d
	Tile [] deetArray; // can make 2d
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	Color32 [,] colors;
	GameObject [,] entities;
	MyChunkSystem.BiomeType [,] biomes;
	
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
		GenerateDetails (prng, pos);
		//detailTilemap.SetTiles (deetPositions, deetArray);

		// *list* of gameobject NEEDS IMPLEMENTING
		entities = gen.GeneratePlants (prng, pos, biomes, treeParent);
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateTiles (System.Random prng, Vector3Int chunkPos) {
		float [] distances = new float [MyChunkSystem.specialCoords.Length];
		int size = chunkSize * chunkSize;
		tilePositions = new Vector3Int [size];
		tileArray = new Tile [size];
		colors = new Color32 [chunkSize, chunkSize];
		MyChunkSystem.BiomeType biome;
		Vector2 biomePos;
		Color32 secondary;
		float distThreshold = 0.3f;
		float heightVal;
		float temp;
		float humidity;

		// in loop, enter all positions and tiles in arrays
		for (int index = 0; index < size; index++) {
			tilePositions [index] = new Vector3Int (index % chunkSize, index / chunkSize, 200);
			Color32 tileColor = new Color32 (117, 173, 141, 255); // standard foresty colour

			// get features for this tile
			heightVal = heights[index % chunkSize, index / chunkSize];
			temp = temps[index % chunkSize, index / chunkSize];
			humidity = humidities [index % chunkSize, index / chunkSize];
			biome = biomes [index % chunkSize, index / chunkSize];
			
			// set tile sprite
			tileArray [index] = tile1;

			// choosing colour for certain biomes
			if (biome != MyChunkSystem.BiomeType.Beach && biome != MyChunkSystem.BiomeType.Water
				&& biome != MyChunkSystem.BiomeType.DeepWater && biome != MyChunkSystem.BiomeType.Ice) {

				// get temp in range for lookup (assuming the max and min possible temperatures)
				temp = Mathf.InverseLerp (-80f, 80f, temp);
				temp *= MyChunkSystem.tableSize;
				biomePos = new Vector2 (humidity, temp);

				// loop through coordinates to determine which biome colours to use
				for (int i = 0; i < MyChunkSystem.specialCoords.Length; i++) {
					distances [i] = Vector2.Distance (MyChunkSystem.specialCoords [i], biomePos);

					// normalise distance to 0-1
					distances [i] = Mathf.InverseLerp (0f, Mathf.Sqrt(MyChunkSystem.tableSize * MyChunkSystem.tableSize +
						MyChunkSystem.tableSize * MyChunkSystem.tableSize), distances [i]); // diagonal length of array is the upper range

					// only take into account the nearest biome colours
					if (distances [i] < distThreshold) {
						secondary = MyChunkSystem.BiomeColours [MyChunkSystem.BiomeTable [MyChunkSystem.specialCoords [i].x,
							MyChunkSystem.specialCoords [i].y]];
						float weighting = 1 - Mathf.InverseLerp (0f, distThreshold, distances [i]); // lerp creates larger range of weightings

						//get difference, multiply by weighting value and add to original
						int r = tileColor.r + Mathf.FloorToInt ((secondary.r - tileColor.r) * weighting);
						int g = tileColor.g + Mathf.FloorToInt ((secondary.g - tileColor.g) * weighting);
						int b = tileColor.b + Mathf.FloorToInt ((secondary.b - tileColor.b) * weighting);

						// check we dont have any extreme colours
						tileColor.r = ReturnColourWithinBound (r, 40, 250);
						tileColor.g = ReturnColourWithinBound (g, 40, 250);
						tileColor.b = ReturnColourWithinBound (b, 40, 250);
					}
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

			// water special case
			else if (biome == MyChunkSystem.BiomeType.Water || biome == MyChunkSystem.BiomeType.DeepWater) {
				tileColor = MyChunkSystem.BiomeColours [MyChunkSystem.BiomeType.Water];
				heightVal = Mathf.InverseLerp (-3f, -0.2f, heightVal);

				tileColor.r = ReturnColourWithinBound ((int)(tileColor.r * heightVal * 0.95), 40, 250);
				tileColor.g = ReturnColourWithinBound ((int)(tileColor.g * heightVal * 0.95), 40, 250);
				tileColor.b = ReturnColourWithinBound ((int)(tileColor.b * heightVal), 40, 250);
			}

			else {
				tileColor = MyChunkSystem.BiomeColours [biome];
			}

			// and finally... we can set the new tile colour
			colors [index % chunkSize, index / chunkSize] = tileColor;
			tileArray [index].color = tileColor;
			tilemap.SetTileFlags (tilePositions [index], TileFlags.None);
			tilemap.SetColor (tilePositions [index], tileColor);
			tilemap.SetTile (tilePositions [index], tileArray [index]);
		}
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	// IMPLEMENT FLOWERS SPAWNING IN PATCHES
	public void GenerateDetails (System.Random prng, Vector3Int chunkPos)
	{
		int sizeFactor = 4; // the cell size is 0.25x the normal cell size
		int size = chunkSize * chunkSize * sizeFactor * sizeFactor;
		deetPositions = new Vector3Int [size];
		deetArray = new Tile [size];
		int randNum;
		MyChunkSystem.BiomeType biome;
		Color32 tileColor;
		bool isNotNull;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < size; i++) {
			int xIndex = Mathf.FloorToInt (i % (chunkSize * sizeFactor)) / sizeFactor;
			int yIndex = Mathf.FloorToInt (i / (chunkSize * sizeFactor)) / sizeFactor;
			biome = biomes [xIndex, yIndex];
			isNotNull = true;

			// temporary grass spawning system
			if ((biome !=  MyChunkSystem.BiomeType.Ice && biome != MyChunkSystem.BiomeType.Water
				&& biome != MyChunkSystem.BiomeType.Desert && biome != MyChunkSystem.BiomeType.Beach)
				&& biome != MyChunkSystem.BiomeType.DeepWater) {

				randNum = prng.Next (0, 15);
				deetPositions [i] = new Vector3Int (i % (chunkSize * sizeFactor),
					i / (chunkSize * sizeFactor), 199);

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

				// set colour according to biome and some extra perlin noise for variation
				if (isNotNull == true && deetArray [i] != detail3) {
					float perlinNoise = Mathf.PerlinNoise ((chunkPos.x + xIndex +
						sys.octaveOffsets[3].x / 3.5f) * 0.1f,(chunkPos.y + yIndex +
						sys.octaveOffsets [3].y / 3.5f) * 0.1f);
					tileColor = colors [xIndex, yIndex];

					// store new values in ints
					int b = tileColor.b - (byte)(25 * perlinNoise);
					int r = tileColor.r + (byte)(25 * perlinNoise);
					int g = tileColor.g + (byte)(25 * perlinNoise);

					// check for overflow while casting to byte
					tileColor.r = ReturnColourWithinBound (r, 160, 254);
					tileColor.g = ReturnColourWithinBound (g, 160, 254);
					tileColor.b = ReturnColourWithinBound (b, 160, 254);

					deetArray [i].color = tileColor;
					detailTilemap.SetTileFlags (deetPositions [i], TileFlags.None);
					detailTilemap.SetColor (deetPositions [i], tileColor);
				}

				detailTilemap.SetTile (deetPositions [i], deetArray [i]);
			}
		}
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
	public byte ReturnColourWithinBound (int val, int min, int max)
	{
		// only works if you provide min and max within 0-255
		if (val < min)
			return (byte)min;
		if (val > max)
			return (byte)max;
		return (byte)val;
	}

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

