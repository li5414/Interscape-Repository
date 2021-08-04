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

	bool isLoaded;
	bool dataRecieved;
	bool isGenerated;
	GameObject treeParent;       // parent for this particular chunk
	
	// position arrays
	Vector3Int [] tilePositionsWorld;

	// tile arrays
	//public Tile [] tileArray = new Tile [chunkSize * chunkSize];
	public RuleTile [] sandTileArray = new RuleTile [chunkSize * chunkSize];
	public Tile [] waterTileArray = new Tile [chunkSize * chunkSize];
	public Tile deetChunk;

	public Texture2D terrainColours = new Texture2D (chunkSize, chunkSize);

	// bools to supposedly save time
	bool containsWater;
	bool containsSand;
	bool containsGrass;


	// biome information
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	GameObject [,] entities;
	BiomeCalculations.BiomeType [,] biomes;
	GameObject terrainChunk;
	
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

	public void GenerateColourTexture()
	{
		// assumes chunkSize does not exxeed the terrainColours width;
		for (int i = 0; i < chunkSize; i++) {
			for (int j = 0; j < chunkSize; j++) {
				float height = heights[i, j];

				float temp = temps[i, j];
				temp = Mathf.InverseLerp (-80f, 80f, temp);
				temp *= bCalc.biomeColourMap.width;

				float humidity = humidities[i, j];
				humidity = 1 - humidity;
				humidity *= bCalc.biomeColourMap.width;
				Color color = bCalc.biomeColourMap.GetPixel ((int)temp, (int)humidity);

				if (height < -0.29f) {
					color = BiomeCalculations.BiomeColours [BiomeCalculations.BiomeType.Beach];
				}

				color.a = Mathf.Clamp01 (Mathf.InverseLerp (-1f, 1f, height)); // lower alpha is deeper

				terrainColours.SetPixel (i, j, color);
			}
		}
		terrainColours.Apply ();
	}

	public Material GenerateMaterial()
	{
		Material newMat = new Material (Shader.Find ("Unlit/ChunkTerrainShader"));
		//Material newMat = new Material (Shader.Find ("Sprites/Default"));
		newMat.CopyPropertiesFromMaterial (bCalc.chunkTerrainMaterial);
		//newMat.mainTexture = terrainColours;
		//newMat.SetTexture ("_TileColors", terrainColours);

		return newMat;
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

		GenerateColourTexture ();

		treeParent.SetActive (true);

		if (containsSand)
			chunkManager.sandTilemap.SetTiles (tilePositionsWorld, sandTileArray);

		if (containsGrass) {
			terrainChunk = UnityEngine.Object.Instantiate (bCalc.chunkTerrainPrefab, new Vector3Int (chunkPos.x + chunkSize/2, chunkPos.y + chunkSize/2, 199), Quaternion.identity);
			terrainChunk.transform.SetParent (bCalc.chunkTerrainParent.gameObject.transform);

			Material newMat = GenerateMaterial ();
			terrainChunk.GetComponent<SpriteRenderer> ().material = newMat;
			terrainChunk.GetComponent<SpriteRenderer> ().material.SetTexture ("_TileColours", terrainColours);
		}

		if (containsWater) {
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
			terrainChunk.SetActive(false);
		}

		// unload water if there is some
		if (containsWater) {
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

