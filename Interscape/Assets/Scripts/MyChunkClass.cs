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
	Vector3Int [,] tilePositions; // probably dont need?
	Vector3Int [] deetPositions; // probably dont need?
	Tile [,] tileArray; // can make 2d
	Tile [] deetArray; // can make 2d
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	Color32 [,] colors;
	GameObject [,] entities;
	BiomeCalculations.BiomeType [,] biomes;
	
	// reference other script
	ChunkManager chunkManager = GameObject.Find ("System Placeholder").GetComponent<ChunkManager> ();
	BiomeCalculations bCalc = GameObject.Find ("System Placeholder").GetComponent<BiomeCalculations> ();

	GreeneryGeneration gen = GameObject.Find ("System Placeholder").GetComponent<GreeneryGeneration> ();
	GameObject TreeParent = GameObject.Find ("TreeParent");

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public MyChunkClass (Vector3Int pos, Tilemap tilemapObj, Tilemap sandTilemapObj, Tilemap waterTilemapObj)
	{
		// get/initialise some important things
		System.Random prng = ChunkManager.prng;
		treeParent = new GameObject();
		treeParent.transform.SetParent (TreeParent.gameObject.transform);
		sandTilemap = sandTilemapObj;
		waterTilemap = waterTilemapObj;

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
		//sandTilemap = Object.Instantiate (tilemapObj, pos, Quaternion.identity);
		//waterTilemap = Object.Instantiate (tilemapObj, new Vector3Int(pos.x, pos.y, 197), Quaternion.identity);

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
		tilePositions = new Vector3Int [chunkSize, chunkSize];
		tileArray = new Tile [chunkSize, chunkSize];
		colors = new Color32 [chunkSize, chunkSize];
		BiomeCalculations.BiomeType biome;
		Vector2 biomePos;
		Color32 secondary;
		float distThreshold = 0.25f;
		float heightVal;
		float temp;
		float humidity;
		float [,] overlapArray = new float [chunkSize + 1, chunkSize + 1];

		// variables for neighbouring chunks and tiles
		MyChunkClass chunkU;
		MyChunkClass chunkR;
		MyChunkClass chunkL;
		MyChunkClass chunkD;
		MyChunkClass chunkUR;
		MyChunkClass chunkDR;
		MyChunkClass chunkDL;
		MyChunkClass chunkUL;
		bool neighbourU;
		bool neighbourUR;
		bool neighbourR;
		bool neighbourDR;
		bool neighbourD;
		bool neighbourDL;
		bool neighbourL;
		bool neighbourUL;

		// get neighbouring chunks
		/*if (chunkManager.terrainChunkDictionary.ContainsKey (new Vector2 (chunkPos.x, chunkPos.y + chunkSize)))
			chunkU = chunkManager.terrainChunkDictionary [new Vector2 (chunkPos.x, chunkPos.y + chunkSize)];
		else
			chunkU = null;
		if (chunkManager.terrainChunkDictionary.ContainsKey (new Vector2 (chunkPos.x, chunkPos.y - chunkSize)))
			chunkD = chunkManager.terrainChunkDictionary [new Vector2 (chunkPos.x, chunkPos.y - chunkSize)];
		else
			chunkD = null;
		if (chunkManager.terrainChunkDictionary.ContainsKey (new Vector2 (chunkPos.x - chunkSize, chunkPos.y)))
			chunkL = chunkManager.terrainChunkDictionary [new Vector2 (chunkPos.x - chunkSize, chunkPos.y)];
		else
			chunkL = null;
		if (chunkManager.terrainChunkDictionary.ContainsKey (new Vector2 (chunkPos.x + chunkSize, chunkPos.y)))
			chunkR = chunkManager.terrainChunkDictionary [new Vector2 (chunkPos.x + chunkSize, chunkPos.y)];
		else
			chunkR = null;


		// create a larger overlap array with the heightmap
		if (chunkU != null) {
			for ()
		}
		for (int i = 1; i < chunkSize + 1; i++) {
			for (int j = 1; j < chunkSize + 1; j++) {
				overlapArray [i, j] = heights [i, j];
			}
		}
		*/
		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < chunkSize; i++) {
			for (int j = 0; j < chunkSize; j++) {
				
				tilePositions [i, j] = new Vector3Int (i, j, 200);
				Color32 tileColor = new Color32 (117, 173, 141, 255); // standard foresty colour

				// get features for this tile
				heightVal = heights [i, j];
				temp = temps [i, j];
				humidity = humidities [i, j];
				biome = biomes [i, j];

				// set tile sprite 
				tileArray [i, j] = TileResources.tileGrass;
				if (biome == BiomeCalculations.BiomeType.Desert)
					tileArray [i, j] = TileResources.tileSand;
				//if (biome == BiomeCalculations.BiomeType.Water || biome == BiomeCalculations.BiomeType.DeepWater)
				//	tileArray [index] = tileWater;


				// set sand layer
				if (heightVal < -0.26) {
					Vector3Int sandPos = new Vector3Int (tilePositions [i,j].x + chunkPos.x, tilePositions [i,j].y + chunkPos.y, 198);
					sandTilemap.SetTile (sandPos, TileResources.tileSandRule);
				}

				/***********************************************/
				// choosing colour for grassy biome types
				if (heightVal >= -0.3 && biome != BiomeCalculations.BiomeType.Ice) {

					// get temp in range for lookup (assuming the max and min possible temperatures)
					temp = Mathf.InverseLerp (-80f, 80f, temp);
					temp *= BiomeCalculations.tableSize;
					biomePos = new Vector2 (humidity, temp);

					// loop through coordinates to determine which biome colours to use
					for (int z = 0; z < BiomeCalculations.coords.Length; z++) {
						distances [z] = Vector2.Distance (BiomeCalculations.coords [z], biomePos);

						// normalise distance to 0-1
						distances [z] = Mathf.InverseLerp (0f, Mathf.Sqrt (BiomeCalculations.tableSize * BiomeCalculations.tableSize +
							BiomeCalculations.tableSize * BiomeCalculations.tableSize), distances [z]); // diagonal length of array is the upper range

						// only take into account the nearest biome colours, making sure its not ice cuz i dont want everything tinted white
						if (distances [z] < distThreshold) {
							if (BiomeCalculations.BiomeTable [BiomeCalculations.coords [z].x, BiomeCalculations.coords [z].y] != BiomeCalculations.BiomeType.Ice) {
								secondary = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeTable [BiomeCalculations.coords [z].x,
															BiomeCalculations.coords [z].y]];
								float weighting = 1 - Mathf.InverseLerp (0f, distThreshold, distances [z]); // lerp creates larger range of weightings

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
					colors [i, j] = tileColor;
					tileArray [i,j].color = tileColor;
					tilemap.SetTileFlags (tilePositions [i,j], TileFlags.None);
					tilemap.SetColor (tilePositions [i,j], tileColor);
					tilemap.SetTile (tilePositions [i,j], tileArray [i,j]);
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
					colors [i, j] = tileColor;
					tileArray [i,j].color = tileColor;
					tilemap.SetTileFlags (tilePositions [i,j], TileFlags.None);
					tilemap.SetColor (tilePositions [i,j], tileColor);
					tilemap.SetTile (tilePositions [i,j], tileArray [i,j]);
				}

				/***********************************************/
				// water special case
				else if (heightVal < -0.3f) {
					tileColor = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeType.Water];
					Tile water;

					Vector3Int waterPos = new Vector3Int (tilePositions [i,j].x + chunkPos.x, tilePositions [i,j].y + chunkPos.y, 198);


					// if x is odd
					if ((i) % 2 == 1) {
						if (j % 2 == 1) // y odd
							water = TileResources.tileWater1;
						else                            // y even
							water = TileResources.tileWater3;
					}
					// if x is even
					else {
						if (j % 2 == 1) // y odd
							water = TileResources.tileWater2;
						else                            // y even
							water = TileResources.tileWater4;
					}

					float darkness = Mathf.InverseLerp (-3f, -0.2f, heightVal);

					// set water tile
					tileColor.r = ReturnColourWithinBound ((int)(tileColor.r * darkness * 0.95), 40, 250);
					tileColor.g = ReturnColourWithinBound ((int)(tileColor.g * darkness * 0.95), 40, 250);
					tileColor.b = ReturnColourWithinBound ((int)(tileColor.b * darkness), 40, 250);
					tileColor.a = (byte)(255 * (1 - Mathf.InverseLerp (-0.6f, -0.2f, heightVal)));

					colors [i, j] = tileColor;
					water.color = tileColor;
					waterTilemap.SetTileFlags (waterPos, TileFlags.None);
					waterTilemap.SetColor (waterPos, tileColor);
					waterTilemap.SetTile (waterPos, water);
				}
			}

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
				if (randNum < 4)
					deetArray [i] = TileResources.grassDetails[randNum];

				// flower generation less likely
				else if (randNum == 5 & prng.Next (0, 15) == 1)
					deetArray [i] = TileResources.grassDetails[4];
				else
					isNotNull = false;

				// set colour according to biome and some extra perlin noise for variation
				if (isNotNull == true && deetArray [i] != TileResources.grassDetails [4]) {
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

