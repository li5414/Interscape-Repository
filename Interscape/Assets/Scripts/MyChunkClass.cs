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
	Tilemap sandTilemap;
	Tilemap waterTilemap;
	TilemapRenderer trender;
	TilemapRenderer drender;
	TilemapRenderer srender;
	TilemapRenderer wrender;
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
	BiomeCalculations.BiomeType [,] biomes;
	
	// tiles
	Tile tileGrass = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Grass");
	RuleTile tileSandRule = Resources.Load<RuleTile> ("Sprites/Map/Tiles/Sand_Rule");
	Tile tileSand = Resources.Load<Tile> ("Sprites/Map/Tiles/Tile_Sand");
	Tile tileWater1 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_0");
	Tile tileWater2 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_1");
	Tile tileWater3 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_2");
	Tile tileWater4 = Resources.Load<Tile> ("Sprites/Map/Tiles/Water_3");

	// grass details
	Tile detail1 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_1");
	Tile detail2 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_2");
	Tile detail3 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_3");
	Tile detail4 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_4");
	Tile detail5 = Resources.Load<Tile> ("Sprites/Map/Tiles/detail_5");



	// reference other script
	ChunkManager chunkManager = GameObject.Find ("System Placeholder").GetComponent<ChunkManager> ();
	BiomeCalculations bCalc = GameObject.Find ("System Placeholder").GetComponent<BiomeCalculations> ();

	GreeneryGeneration gen = GameObject.Find ("System Placeholder").GetComponent<GreeneryGeneration> ();
	GameObject TreeParent = GameObject.Find ("TreeParent");

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public MyChunkClass (Vector3Int pos, int seed, Tilemap tilemapObj, Tilemap sandTilemapObj)
	{
		// create rand num generator from seed to use throughout
		System.Random prng = ChunkManager.prng;
		treeParent = new GameObject();
		treeParent.transform.SetParent (TreeParent.gameObject.transform);
		//sandTilemap = sandTilemapObj;

		// initialise arrays
		heights = bCalc.GetHeightValues (pos.x, pos.y);
		temps = bCalc.GetTemperatures (pos.x, pos.y, heights);
		humidities = bCalc.GetHumidityArray (pos.x, pos.y, heights);
		biomes = bCalc.GetBiomes (heights, temps, humidities);

		// reference grid objects
		GameObject grid = bCalc.grid;
		GameObject detailGrid = bCalc.detailGrid;
		GameObject sandGrid = bCalc.sandGrid;
		GameObject waterGrid = bCalc.waterGrid;

		// set up tilemap gameobject
		tilemap = Object.Instantiate (tilemapObj, pos, Quaternion.identity);
		detailTilemap = Object.Instantiate (tilemapObj, pos, Quaternion.identity);
		sandTilemap = Object.Instantiate (tilemapObj, pos, Quaternion.identity);
		waterTilemap = Object.Instantiate (tilemapObj, new Vector3Int(pos.x, pos.y, 197), Quaternion.identity);

		// locate the renderer component for each
		trender = tilemap.GetComponent<TilemapRenderer> ();
		drender = detailTilemap.GetComponent<TilemapRenderer> ();
		srender = sandTilemap.GetComponent<TilemapRenderer> ();
		wrender = waterTilemap.GetComponent<TilemapRenderer> ();

		// set the tilemaps to the correct grid parents, grid determines layout
		tilemap.transform.SetParent (grid.gameObject.transform);
		detailTilemap.transform.SetParent (detailGrid.gameObject.transform);
		sandTilemap.transform.SetParent (sandGrid.gameObject.transform);
		waterTilemap.transform.SetParent (waterGrid.gameObject.transform);

		// creates and sets tiles in tilearray to positions in position array
		GenerateTiles (prng, pos);

		// generate grass details
		GenerateDetails (prng, pos);

		// array of gameobjects (use list instead?)
		entities = gen.GeneratePlants (prng, pos, biomes, heights, treeParent);
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateTiles (System.Random prng, Vector3Int chunkPos) {
		float [] distances = new float [BiomeCalculations.coords.Length];
		int size = chunkSize * chunkSize;
		tilePositions = new Vector3Int [size];
		tileArray = new Tile [size];
		colors = new Color32 [chunkSize, chunkSize];
		BiomeCalculations.BiomeType biome;
		Vector2 biomePos;
		Color32 secondary;
		float distThreshold = 0.25f;
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
			tileArray [index] = tileGrass;
			if (biome == BiomeCalculations.BiomeType.Desert)
				tileArray [index] = tileSand;
			//if (biome == BiomeCalculations.BiomeType.Water || biome == BiomeCalculations.BiomeType.DeepWater)
			//	tileArray [index] = tileWater;


			// set sand layer
			if (heightVal < -0.26) {
				//tileSandRule.color = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeType.Beach];
				//sandTilemap.SetTileFlags (tilePositions [index], TileFlags.None);
				//sandTilemap.SetColor (tilePositions [index], tileColor);
				//Vector3Int sandPos = new Vector3Int (tilePositions [index].x + chunkPos.x, tilePositions [index].y + chunkPos.y, 198);
				Vector3Int sandPos = new Vector3Int (tilePositions [index].x, tilePositions [index].y, 200);

				sandTilemap.SetTile (sandPos, tileSandRule);
			}

			/***********************************************/
			// choosing colour for grassy biome types
			if (heightVal >= -0.3 && biome != BiomeCalculations.BiomeType.Ice) {

				// get temp in range for lookup (assuming the max and min possible temperatures)
				temp = Mathf.InverseLerp (-80f, 80f, temp);
				temp *= BiomeCalculations.tableSize;
				biomePos = new Vector2 (humidity, temp);

				// loop through coordinates to determine which biome colours to use
				for (int i = 0; i < BiomeCalculations.coords.Length; i++) {
					distances [i] = Vector2.Distance (BiomeCalculations.coords [i], biomePos);

					// normalise distance to 0-1
					distances [i] = Mathf.InverseLerp (0f, Mathf.Sqrt (BiomeCalculations.tableSize * BiomeCalculations.tableSize +
						BiomeCalculations.tableSize * BiomeCalculations.tableSize), distances [i]); // diagonal length of array is the upper range

					// only take into account the nearest biome colours, making sure its not ice cuz i dont want everything tinted white
					if (distances [i] < distThreshold) {
						if (BiomeCalculations.BiomeTable [BiomeCalculations.coords [i].x, BiomeCalculations.coords [i].y] != BiomeCalculations.BiomeType.Ice) {
							secondary = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeTable [BiomeCalculations.coords [i].x,
														BiomeCalculations.coords [i].y]];
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

				// and finally... we can set the new tile colour
				colors [index % chunkSize, index / chunkSize] = tileColor;
				tileArray [index].color = tileColor;
				tilemap.SetTileFlags (tilePositions [index], TileFlags.None);
				tilemap.SetColor (tilePositions [index], tileColor);
				tilemap.SetTile (tilePositions [index], tileArray [index]);
			}

			/***********************************************/
			// ice biome special case
			else if (biome == BiomeCalculations.BiomeType.Ice) {
				tileColor = BiomeCalculations.BiomeColours [biome];
				float darkness = Mathf.InverseLerp (-10f, 1f, heightVal);
				tileColor.r = (byte)(tileColor.r * darkness * 0.95);
				tileColor.g = (byte)(tileColor.g * darkness * 0.95);
				tileColor.b = (byte)(tileColor.b * darkness);

				// and finally... we can set the new tile colour
				colors [index % chunkSize, index / chunkSize] = tileColor;
				tileArray [index].color = tileColor;
				tilemap.SetTileFlags (tilePositions [index], TileFlags.None);
				tilemap.SetColor (tilePositions [index], tileColor);
				tilemap.SetTile (tilePositions [index], tileArray [index]);
			}

			/***********************************************/
			// water special case
			else if (heightVal < -0.3f) {
				tileColor = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeType.Water];
				Tile water;

				// if x is odd
				if ((index % chunkSize) % 2 == 1) { 
					if (index / chunkSize % 2 == 1) // y odd
						water = tileWater1;
					else                            // y even
						water = tileWater3;
				}
				// if x is even
				else {
					if (index / chunkSize % 2 == 1) // y odd
						water = tileWater2;
					else                            // y even
						water = tileWater4;
				}

				float darkness = Mathf.InverseLerp (-3f, -0.2f, heightVal);

				// set water tile
				tileColor.r = ReturnColourWithinBound ((int)(tileColor.r * darkness * 0.95), 40, 250);
				tileColor.g = ReturnColourWithinBound ((int)(tileColor.g * darkness * 0.95), 40, 250);
				tileColor.b = ReturnColourWithinBound ((int)(tileColor.b * darkness), 40, 250);
				tileColor.a = (byte)(255 * (1 - Mathf.InverseLerp (-0.6f, -0.2f, heightVal)));

				colors [index % chunkSize, index / chunkSize] = tileColor;
				water.color = tileColor;
				waterTilemap.SetTileFlags (tilePositions [index], TileFlags.None);
				waterTilemap.SetColor (tilePositions [index], tileColor);
				waterTilemap.SetTile (tilePositions [index], water);
			}

			
			/***********************************************/
			// beach biome case
			/*else {
				tileColor = BiomeCalculations.BiomeColours [biome];

				// and finally... we can set the new tile colour
				colors [index % chunkSize, index / chunkSize] = tileColor;
				tileArray [index].color = tileColor;
				tilemap.SetTileFlags (tilePositions [index], TileFlags.None);
				tilemap.SetColor (tilePositions [index], tileColor);
				tilemap.SetTile (tilePositions [index], tileArray [index]);
			}*/
		}
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateDetails (System.Random prng, Vector3Int chunkPos)
	{
		int sizeFactor = 4; // the cell size is 0.25x the normal cell size
		int size = chunkSize * chunkSize * sizeFactor * sizeFactor;
		deetPositions = new Vector3Int [size];
		deetArray = new Tile [size];
		int randNum;
		BiomeCalculations.BiomeType biome;
		Color32 tileColor;
		bool isNotNull;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < size; i++) {
			int xIndex = Mathf.FloorToInt (i % (chunkSize * sizeFactor)) / sizeFactor;
			int yIndex = Mathf.FloorToInt (i / (chunkSize * sizeFactor)) / sizeFactor;
			biome = biomes [xIndex, yIndex];
			isNotNull = true;

			// temporary grass spawning system
			if ((biome != BiomeCalculations.BiomeType.Ice && biome != BiomeCalculations.BiomeType.Desert)
				&& heights[xIndex, yIndex] > -0.3) {

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
						bCalc.octaveOffsets [3].x / 3.5f) * 0.1f,(chunkPos.y + yIndex +
						bCalc.octaveOffsets [3].y / 3.5f) * 0.1f);
					tileColor = colors [xIndex, yIndex];

					// store new values in ints
					int b = (int)(tileColor.b - (25 * perlinNoise));
					int r = (int)(tileColor.r + (25 * perlinNoise));
					int g = (int)(tileColor.g + (25 * perlinNoise));

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
		srender.enabled = visible;
		wrender.enabled = visible;
		treeParent.SetActive (visible);
	}

	public bool IsVisible ()
	{
		return trender.enabled;
	}
}

