using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using System.Threading;
using System;
using System.IO;

public class Chunk
{
	// components
	public static int chunkSize = 16;
	public Vector2Int chunkPos;
	public Vector2Int chunkCoord;
	public static int sizeFactor = 2; // the cell size is 0.25x the normal cell size

	bool isLoaded = false;
	bool dataRecieved = false;
	bool isGenerated = false;
	GameObject treeParent;       // parent for this particular chunk
	
	// position arrays
	Vector3Int [] tilePositionsWorld;
	//Vector3Int [] deetPositions;

	// tile arrays
	//public Tile [] tileArray = new Tile [chunkSize * chunkSize];
	public RuleTile [] sandTileArray = new RuleTile [chunkSize * chunkSize];
	public Tile [] waterTileArray = new Tile [chunkSize * chunkSize];
	//public Tile [] deetArray = new Tile [chunkSize * chunkSize * sizeFactor * sizeFactor];
	public Tile deetChunk;

	// bools to supposedly save time
	bool containsWater;
	bool containsSand;
	bool containsGrass;


	// biome information
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	//Color32 [,] colors;
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


		//chunkManager.allocate (this);
		//GenerateChunkData();
		createThread ();

	}

	

	public void createThread()
	{
		Thread thread = new Thread (() => { this.GenerateChunkData (); });
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
		tilePositionsWorld = new Vector3Int [chunkSize * chunkSize];
		

		// creates and sets tiles in tilearray to positions in position array
		GenerateTilesChunked ();

		// generate grass details
		GenerateDetailsChunk ();

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

	/*public void GenerateTiles () {
		colors = new Color32 [chunkSize, chunkSize];
		BiomeCalculations.BiomeType biome;
		float heightVal;
		float temp;
		float humidity;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < chunkSize; i++) {
			for (int j = 0; j < chunkSize; j++) {

				// fill in tile position arrays
				//tilePositions [at(i, j)] = new Vector3Int (i, j, 200);
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


				// choosing colour for grassy biome types
				if (heightVal >= -0.3) {

					tileColor = NewBiomeColorAlgorithm (temp, humidity);

					// and finally... we can set the new tile colour
					colors [i, j] = tileColor;
					tileArray [at (i, j)].color = tileColor;
					containsGrass = true;
				}

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
	}*/

	/*public Color NewBiomeColorAlgorithm (float temp, float humidity)
	{
		temp = Mathf.InverseLerp (-80f, 80f, temp);
		temp *= bCalc.biomeColourMap.width;

		humidity = 1 - humidity;
		humidity *= bCalc.biomeColourMap.width;
		Color color = bCalc.biomeColourMap.GetPixel ((int)temp, (int)humidity);

		return color;

	}*/

	/*public void GenerateDetails ()
	{
		
		int size = chunkSize * chunkSize * sizeFactor * sizeFactor;
		int nTilesWidth = chunkSize * sizeFactor;
		deetPositions = new Vector3Int [size];
		int randNum;
		BiomeCalculations.BiomeType biome;
		Color32 tileColor;
		bool isNotNull;
		int newChunkPosX = chunkPos.x * sizeFactor;
		int newChunkPosY = chunkPos.y * sizeFactor;

		// in loop, enter all positions and tiles in arrays
		for (int x = 0; x < nTilesWidth; x++) {
			for (int y = 0; y < nTilesWidth; y++) {
				int xIndex = (int)(x / sizeFactor);
				int yIndex = (int)(y / sizeFactor);
				biome = biomes [xIndex, yIndex];
				tileColor = Color.white;
				isNotNull = true;

				deetPositions [at64 (x, y)] = new Vector3Int (x + newChunkPosX, y + newChunkPosY, 199);

				// temporary grass spawning system
				if (biome != BiomeCalculations.BiomeType.Ice && biome != BiomeCalculations.BiomeType.Desert
					&& heights [xIndex, yIndex] > -0.3) {
					randNum = prng.Next (0, 50);

					// generate grass details using random numbers
					if (randNum < 28) {
						if (randNum >= 8 && prng.Next (0, 50) < 30) {
							randNum %= 8;
						}

						deetArray [at64 (x, y)].sprite = ChunkManager.tileResources.grassDetails [randNum];
					} else
						isNotNull = false;


					// alter colour according to biome and some extra perlin noise for variation
					if (isNotNull) {
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

						deetArray [at64 (x, y)].color = tileColor;
					} else
						deetArray [at64 (x, y)] = null;

				}
			}
			
		}
		
	}*/

	public void GenerateTilesChunked ()
	{
		float heightVal;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < chunkSize; i++) {
			for (int j = 0; j < chunkSize; j++) {

				// fill in tile position arrays
				tilePositionsWorld [at (i, j)] = new Vector3Int (i + chunkPos.x, j + chunkPos.y, 0);


				// get features for this tile
				heightVal = heights [i, j];


				// sand layer
				if (heightVal < -0.26) {
					sandTileArray [at (i, j)] = ChunkManager.tileResources.tileSandRule;
					containsSand = true;
				}

				// grass layer
				if (heightVal >= -0.4) {
					containsGrass = true;
				}

				// water layer
				if (heightVal < -0.26f) { // used to be -0.3, but im doing some overlapping
					containsWater = true;
				}
			}
		}
	}

	public void GenerateDetailsChunk ()
	{
		int randNum = prng.Next (0, 7);
		deetChunk = ChunkManager.tileResources.grassDetailsChunk [randNum];
	
	}



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
		treeParent.SetActive (true);

		if (containsSand)
			chunkManager.sandTilemap.SetTiles (tilePositionsWorld, sandTileArray);

		if (containsGrass) {
			chunkManager.grassTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), ChunkManager.tileResources.tileGrassBig);
			chunkManager.detailTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), deetChunk);
		}

		if (containsWater) {
			//chunkManager.waterTilemap.SetTiles (tilePositionsWorld, waterTileArray);
			chunkManager.waterTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), ChunkManager.tileResources.plainChunk);
		}


		isLoaded = true;
	}

	public void UnloadChunk () {
		if (!isLoaded)
			return;

		Assert.IsTrue(isGenerated);

		// disable things
		treeParent.SetActive (false);

		// unload sand if there is some
		if (containsSand) {
			for (int i = 0; i < tilePositionsWorld.Length; i++) {
				chunkManager.sandTilemap.SetTile (tilePositionsWorld [i], null);
			}
		}

		// unload grass if there is some
		if (containsGrass) {

			chunkManager.grassTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), null);
			chunkManager.detailTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), null);
		}

		// unload water if there is some
		if (containsWater) {
			/*for (int i = 0; i < tilePositionsWorld.Length; i++) {
				chunkManager.waterTilemap.SetTile (tilePositionsWorld [i], null);
			}*/
			chunkManager.waterTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), null);
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
		return (x * 32 + y);
	}
}

