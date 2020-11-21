using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;


public class Chunk
{
	// components
	public static int chunkSize = 16;
	bool isLoaded;
	GameObject treeParent;       // parent for this particular chunk
	public Vector2Int chunkPos;

	// deet stuff
	Tilemap detailTilemap;
	TilemapRenderer detailRenderer;
	
	// position arrays
	Vector3Int [] tilePositions;
	Vector3Int [] tilePositionsWorld;
	Vector3Int [] deetPositions;

	// tile arrays
	Tile [] tileArray;
	RuleTile [] sandTileArray;
	Tile [] waterTileArray;
	Tile [] deetArray;

	// biome information
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	Color32 [,] colors;
	GameObject [,] entities;
	BiomeCalculations.BiomeType [,] biomes;
	
	// reference other scripts
	static ChunkManager chunkManager = GameObject.Find ("System Placeholder").GetComponent<ChunkManager> ();
	static BiomeCalculations bCalc = GameObject.Find ("System Placeholder").GetComponent<BiomeCalculations> ();
	static GreeneryGeneration gen = GameObject.Find ("System Placeholder").GetComponent<GreeneryGeneration> ();

	static GameObject TreeParent = GameObject.Find ("TreeParent");

	// random number generator
	System.Random prng;

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public Chunk (System.Random prng, Vector2Int pos)
	{
		// get/initialise some important things
		chunkPos = new Vector2Int (pos.x, pos.y);
		this.prng = prng;
		treeParent = new GameObject();
		treeParent.transform.SetParent (TreeParent.gameObject.transform);
		Tilemap tilemapObj = chunkManager.tilemapObj;

		// initialise arrays
		heights = bCalc.GetHeightValues (chunkPos.x, chunkPos.y);
		temps = bCalc.GetTemperatures (chunkPos.x, chunkPos.y, heights);
		humidities = bCalc.GetHumidityArray (chunkPos.x, chunkPos.y, heights);
		biomes = bCalc.GetBiomes (heights, temps, humidities);

		// reference grid objects
		GameObject grid = bCalc.grid;
		GameObject detailGrid = bCalc.detailGrid;
		GameObject sandGrid = bCalc.sandGrid;
		GameObject waterGrid = bCalc.waterGrid;

		// set up tilemap gameobject
		detailTilemap = Object.Instantiate (tilemapObj, new Vector3Int (chunkPos.x, chunkPos.y, 200), Quaternion.identity);
		detailRenderer = detailTilemap.GetComponent<TilemapRenderer> ();
		detailTilemap.transform.SetParent (detailGrid.gameObject.transform);

		// creates and sets tiles in tilearray to positions in position array
		GenerateTiles ();

		// generate grass details
		GenerateDetails ();

		// array of gameobjects (use dict/list instead?)
		entities = gen.GeneratePlants (chunkPos, biomes, heights, treeParent);

		// load in the chunk
		isLoaded = false;
		LoadChunk ();
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateTiles () {
		float [] distances = new float [BiomeCalculations.coords.Length];

		// initalise arrays to be be used for settiles()
		tilePositions = new Vector3Int [chunkSize * chunkSize];
		tilePositionsWorld = new Vector3Int [chunkSize * chunkSize];
		tileArray = new Tile [chunkSize * chunkSize];
		sandTileArray = new RuleTile [chunkSize * chunkSize];
		waterTileArray = new Tile [chunkSize * chunkSize];


		// other information
		colors = new Color32 [chunkSize, chunkSize];
		BiomeCalculations.BiomeType biome;
		Vector2 biomePos;
		Color32 secondary;
		float distThreshold = 0.25f;
		float heightVal;
		float temp;
		float humidity;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < chunkSize; i++) {
			for (int j = 0; j < chunkSize; j++) {

				// fill in tile position arrays
				tilePositions [at(i, j)] = new Vector3Int (i, j, 200);
				tilePositionsWorld [at (i, j)] = new Vector3Int (i + chunkPos.x, j + chunkPos.y, 0);

				// colour variable used to set tile colour
				//Color32 tileColor = new Color32 (117, 173, 141, 255); // standard foresty colour
				//Color32 tileColor = new Color32 (127, 181, 139, 255);
				Color32 tileColor = new Color32 (139, 179, 148, 255);

				// get features for this tile
				heightVal = heights [i, j];
				temp = temps [i, j];
				humidity = humidities [i, j];
				biome = biomes [i, j];

				// set tile sprite 
				if (biome == BiomeCalculations.BiomeType.Desert)
					tileArray [at (i, j)] = TileResources.tileSand;
				else
					tileArray [at (i, j)] = TileResources.tileGrass;

				// set sand layer
				if (heightVal < -0.26) {
					sandTileArray [at(i, j)] = TileResources.tileSandRule;
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
					tileArray [at (i, j)] = createNewTile (tileArray [at (i, j)].sprite, tileColor);
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
					tileArray [at (i, j)] = createNewTile (tileArray [at (i, j)].sprite, tileColor);
				}

				/***********************************************/
				// water special case
				else if (heightVal < -0.3f) {
					tileColor = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeType.Water];
					Tile water;

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
					waterTileArray [at (i, j)] = createNewTile (water.sprite, tileColor);
				}
			}
		}
	}

	

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateDetails ()
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
		for (int x = 0; x < chunkSize * sizeFactor; x++) {
			for (int y = 0; y < chunkSize * sizeFactor; y++) {
				int xIndex = (int)(x / sizeFactor);
				int yIndex = (int)(y / sizeFactor);//potential integer division error idk??
				biome = biomes [xIndex, yIndex];
				isNotNull = true;
	
				// temporary grass spawning system
				if (biome != BiomeCalculations.BiomeType.Ice && biome != BiomeCalculations.BiomeType.Desert
					&& heights [xIndex, yIndex] > -0.3) {
					randNum = prng.Next (0, 15);
					
					// generate grass details using random numbers
					if (randNum < 4)
						deetArray [at64 (x, y)] = TileResources.grassDetails [randNum];

					// flower generation less likely
					else if (randNum == 5 & prng.Next (0, 15) == 1)
						deetArray [at64 (x, y)] = TileResources.grassDetails [4];
					else
						isNotNull = false;

					
					deetPositions [at64 (x, y)] = new Vector3Int (x, y, 199);

					// alter colour according to biome and some extra perlin noise for variation
					if (isNotNull && deetArray [at64 (x, y)] != TileResources.grassDetails [4]) {
						float perlinNoise = Mathf.PerlinNoise ((chunkPos.x + xIndex +
							bCalc.octaveOffsets [3].x / 3.5f) * 0.1f, (chunkPos.y + yIndex +
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

						

						//alter colour
						deetArray [at64 (x, y)] = createNewTile (deetArray [at64 (x, y)].sprite, tileColor);
					}
				}
			}
			
		}
		detailTilemap.SetTiles (deetPositions, deetArray);
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public byte ReturnColourWithinBound (int val, int min, int max) {
		// only works if you provide min and max within 0-255
		if (val < min)
			return (byte)min;
		if (val > max)
			return (byte)max;
		return (byte)val;
	}

	public Color32 ReturnColourWithinBiome (Color32 color, BiomeCalculations.BiomeType biome)
	{
		if (color.r > BiomeCalculations.BiomeColours [biome].r)
			color.r = BiomeCalculations.BiomeColours [biome].r;
		if (color.g > BiomeCalculations.BiomeColours [biome].g)
			color.g = BiomeCalculations.BiomeColours [biome].g;
		if (color.b > BiomeCalculations.BiomeColours [biome].b)
			color.b = BiomeCalculations.BiomeColours [biome].b;
		return color;
	}

	public void LoadChunk () {
		if (isLoaded)
			return;

		// enable things
		detailRenderer.enabled = true;
		treeParent.SetActive (true);
		isLoaded = true;

		chunkManager.sandTilemap.SetTiles (tilePositionsWorld, sandTileArray);
		chunkManager.grassTilemap.SetTiles (tilePositionsWorld, tileArray);
		chunkManager.waterTilemap.SetTiles (tilePositionsWorld, waterTileArray);
	}

	public void UnloadChunk () {
		if (!isLoaded)
			return;

		// disable things
		detailRenderer.enabled = false;
		treeParent.SetActive (false);
		isLoaded = false;

		for (int i = 0; i < tilePositionsWorld.Length; i++) {
			chunkManager.sandTilemap.SetTile (tilePositionsWorld [i], null);
			chunkManager.grassTilemap.SetTile (tilePositionsWorld [i], null);
			chunkManager.waterTilemap.SetTile (tilePositionsWorld [i], null);
		}
	}

	public bool IsLoaded () {
		return isLoaded;
	}

	public Tile createNewTile (Sprite sprite, Color color) {
		Tile newTile = ScriptableObject.CreateInstance<Tile> ();
		newTile.sprite = sprite;
		newTile.color = color;
		return newTile;
	}

	// lookup index of 16x16 2D array condensed to 1D array
	public int at (int x, int y)
	{
		return (x * 16 + y);
	}

	// lookup index of 64x64 2D array condensed to 1D array
	public int at64 (int x, int y)
	{
		return (x * 64 + y);
	}
}

