using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using System.Threading;
using System;


public class Chunk
{
	// components
	public static int chunkSize = 16;
	public Vector2Int chunkPos;
	public Vector2Int chunkCoord;

	bool isLoaded = false;
	bool dataRecieved = false;
	bool isGenerated = false;
	GameObject treeParent;       // parent for this particular chunk
	

	// deet stuff
	Tilemap detailTilemap;
	TilemapRenderer detailRenderer;
	
	// position arrays
	Vector3Int [] tilePositions;
	Vector3Int [] tilePositionsWorld;
	Vector3Int [] deetPositions;

	// tile arrays
	public Tile [] tileArray = new Tile [chunkSize * chunkSize];
	public RuleTile [] sandTileArray = new RuleTile [chunkSize * chunkSize];
	public Tile [] waterTileArray = new Tile [chunkSize * chunkSize];
	public Tile [] deetArray;

	// bools to supposedly save time
	bool containsNoWater = true;
	bool containsNoGrass = true;
	bool containsNoSand = true;


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
		chunkPos = new Vector2Int (pos.x * chunkSize, pos.y * chunkSize);
		chunkCoord = new Vector2Int (pos.x, pos.y);
		this.prng = prng;
		treeParent = new GameObject();
		treeParent.transform.SetParent (TreeParent.gameObject.transform);
		Tilemap tilemapObj = chunkManager.tilemapObj;

		// reference grid objects
		GameObject detailGrid = bCalc.detailGrid;

		// set up tilemap gameobject
		detailTilemap = UnityEngine.Object.Instantiate (tilemapObj, new Vector3Int (chunkPos.x, chunkPos.y, 200), Quaternion.identity);
		detailRenderer = detailTilemap.GetComponent<TilemapRenderer> ();
		detailTilemap.transform.SetParent (detailGrid.gameObject.transform);

		chunkManager.allocate (this);

	}

	

	public void createThread()
	{
		var thread = new Thread (() => { this.GenerateChunkData (); });
		thread.Start ();
	}

	public void FinishGenerating()
	{
		// array of gameobjects (use dict/list instead?)
		entities = gen.GeneratePlants (chunkPos, biomes, heights, treeParent);

		// load in the chunk
		isGenerated = true;
		chunkManager.chunksToLoad.Enqueue (this);
	}

	public void GenerateChunkData() 
	{
		// initialise arrays
		heights = bCalc.GetHeightValues (chunkPos.x, chunkPos.y);
		temps = bCalc.GetTemperatures (chunkPos.x, chunkPos.y, heights);
		humidities = bCalc.GetHumidityArray (chunkPos.x, chunkPos.y, heights);
		biomes = bCalc.GetBiomes (heights, temps, humidities);

		// initalise arrays to be be used for settiles()
		tilePositions = new Vector3Int [chunkSize * chunkSize];
		tilePositionsWorld = new Vector3Int [chunkSize * chunkSize];
		

		// creates and sets tiles in tilearray to positions in position array
		GenerateTiles ();

		// generate grass details
		//GenerateDetails ();

		dataRecieved = true;
	}

	
	public bool IsReadyToFinishGeneration()
	{
		return dataRecieved;
	}

	public bool IsGenerated ()
	{
		return isGenerated;
	}


	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void GenerateTiles () {
		float [] distances = new float [BiomeCalculations.coords.Length];

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
				Color32 tileColor = new Color32 (139, 179, 148, 255); // standard foresty colour

				// get features for this tile
				heightVal = heights [i, j];
				temp = temps [i, j];
				humidity = humidities [i, j];
				biome = biomes [i, j];

				// set tile sprite 
				if (biome == BiomeCalculations.BiomeType.Desert)
					tileArray [at (i, j)].sprite = ChunkManager.tileResources.tileSand.sprite;
				else
					tileArray [at (i, j)].sprite = ChunkManager.tileResources.tileGrass.sprite;

				// set sand layer
				if (heightVal < -0.26) {
					sandTileArray [at (i, j)] = ChunkManager.tileResources.tileSandRule;
					containsNoSand = false;
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
					tileArray [at (i, j)].color = tileColor;
					containsNoGrass = false;
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
					tileArray [at (i, j)].color = tileColor;
				}

				/***********************************************/
				// water special case
				else if (heightVal < -0.3f) {
					tileColor = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeType.Water];
					Tile water = waterTileArray[at(i, j)];

					// if x is odd
					if ((i) % 2 == 1) {
						if (j % 2 == 1) // y odd
							water.sprite = ChunkManager.tileResources.tileWater1.sprite;
						else                            // y even
							water.sprite = ChunkManager.tileResources.tileWater3.sprite;
					}
					// if x is even
					else {
						if (j % 2 == 1) // y odd
							water.sprite = ChunkManager.tileResources.tileWater2.sprite;
						else                            // y even
							water.sprite = ChunkManager.tileResources.tileWater4.sprite;
					}

					float darkness = Mathf.InverseLerp (-3f, -0.2f, heightVal);

					// set water tile
					tileColor.r = ReturnColourWithinBound ((int)(tileColor.r * darkness * 0.95), 40, 250);
					tileColor.g = ReturnColourWithinBound ((int)(tileColor.g * darkness * 0.95), 40, 250);
					tileColor.b = ReturnColourWithinBound ((int)(tileColor.b * darkness), 40, 250);
					tileColor.a = (byte)(255 * (1 - Mathf.InverseLerp (-0.6f, -0.2f, heightVal)));

					colors [i, j] = tileColor;
					water.color = tileColor;
					containsNoWater = false;
					tileArray [at (i, j)] = null; // removing tiles under ocean
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
				int yIndex = (int)(y / sizeFactor);
				biome = biomes [xIndex, yIndex];
				isNotNull = true;
	
				// temporary grass spawning system
				if (biome != BiomeCalculations.BiomeType.Ice && biome != BiomeCalculations.BiomeType.Desert
					&& heights [xIndex, yIndex] > -0.3) {
					randNum = prng.Next (0, 15);
					
					// generate grass details using random numbers
					if (randNum < 4)
						deetArray [at64 (x, y)] = ChunkManager.tileResources.grassDetails [randNum];

					// flower generation less likely
					else if (randNum == 5 & prng.Next (0, 15) == 1)
						deetArray [at64 (x, y)] = ChunkManager.tileResources.grassDetails [4];
					else
						isNotNull = false;

					
					deetPositions [at64 (x, y)] = new Vector3Int (x, y, 199);

					// alter colour according to biome and some extra perlin noise for variation
					if (isNotNull && deetArray [at64 (x, y)] != ChunkManager.tileResources.grassDetails [4]) {
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

	public void ColourWithinBounds (Color32 color, int min, int max)
	{
		// only works if you provide min and max within 0-255

		color.r = (byte)Mathf.Clamp (color.r, min, max);
		color.g = (byte)Mathf.Clamp (color.r, min, max);
		color.b = (byte)Mathf.Clamp (color.r, min, max);
	}

	public void LoadChunk () {
		if (isLoaded)
			return;

		Assert.IsTrue (isGenerated);

		// enable things
		detailTilemap.enabled = true;
		treeParent.SetActive (true);

		if (!containsNoSand)
			chunkManager.sandTilemap.SetTiles (tilePositionsWorld, sandTileArray);

		if (!containsNoGrass)
			chunkManager.grassTilemap.SetTiles (tilePositionsWorld, tileArray);

		if (!containsNoWater)
			chunkManager.waterTilemap.SetTiles (tilePositionsWorld, waterTileArray);

		isLoaded = true;
	}

	public void UnloadChunk () {
		if (!isLoaded)
			return;

		Assert.IsTrue(isGenerated);

		// disable things
		detailTilemap.enabled = false;
		treeParent.SetActive (false);

		// unload sand if there is some
		if (!containsNoSand) {
			for (int i = 0; i < tilePositionsWorld.Length; i++) {
				chunkManager.sandTilemap.SetTile (tilePositionsWorld [i], null);
			}
		}

		// unload grass if there is some
		if (!containsNoGrass) {
			for (int i = 0; i < tilePositionsWorld.Length; i++) {
				chunkManager.grassTilemap.SetTile (tilePositionsWorld [i], null);
			}
		}

		// unload water if there is some
		if (!containsNoWater) {
			for (int i = 0; i < tilePositionsWorld.Length; i++) {
				chunkManager.waterTilemap.SetTile (tilePositionsWorld [i], null);
			}
		}
		
		isLoaded = false;
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

