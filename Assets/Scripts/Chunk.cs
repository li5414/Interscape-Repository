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
	public Vector2Int chunkPos;
	public Vector2Int chunkCoord;
	
	bool isLoaded;
	bool isGenerated;
	bool containsWater;
	bool containsSand;
	bool containsGrass;

	// parent for this particular chunk
	GameObject treeParent;       
	
	// position arrays
	Vector3Int [] tilePositionsWorld;

	// tile arrays
	public RuleTile [] sandTileArray = new RuleTile [Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];
	public Tile [] waterTileArray = new Tile [Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];

	// tile to store the grass blade details (the size of the tile takes up the whole chunk)
	public Tile deetChunk;

	// biome information
	public Texture2D terrainColours = new Texture2D (Consts.CHUNK_SIZE, Consts.CHUNK_SIZE);
	float [,] heights;
	float [,] temps;
	float [,] humidities;
	GameObject [,] entities;
	BiomeType [,] biomes;
	GameObject terrainChunk;
	GameObject waterChunk;
	
	// reference other scripts
	static ChunkManager chunkManager = GameObject.Find ("System Placeholder").GetComponent<ChunkManager> ();
	static BiomeCalculations bCalc = GameObject.Find ("System Placeholder").GetComponent<BiomeCalculations> ();
	static GreeneryGeneration gen = GameObject.Find ("System Placeholder").GetComponent<GreeneryGeneration> ();
	static WorldSettings worldSettings = GameObject.Find ("System Placeholder").GetComponent<WorldSettings> ();

	static GameObject TreeParent = GameObject.Find ("TreeParent");
	
	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	public Chunk (Vector2Int pos)
	{
		// get/initialise some important things
		chunkPos = new Vector2Int (pos.x * Consts.CHUNK_SIZE, pos.y * Consts.CHUNK_SIZE);
		chunkCoord = new Vector2Int (pos.x, pos.y);
		treeParent = new GameObject();
		treeParent.transform.SetParent (TreeParent.gameObject.transform);
		GenerateChunkData();
	}

	public void GenerateChunkData() 
	{
		// initialise arrays
		heights = bCalc.GetHeightValues (chunkPos.x, chunkPos.y);
		temps = bCalc.GetTemperatures (chunkPos.x, chunkPos.y, heights);
		humidities = bCalc.GetHumidityArray (chunkPos.x, chunkPos.y, heights);
		biomes = bCalc.GetBiomes (heights, temps, humidities);
		
		// initalise arrays to be be used for settiles()
		tilePositionsWorld = new Vector3Int [Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];
		
		// creates and sets tiles in tilearray to positions in position array
		GenerateTilesChunked ();
		
		// generate the colour texture for that chunk (determines colour of grass)
		GenerateColourTexture ();

		// generate grass details
		GenerateDetailsChunk ();

		// array of gameobjects (use dict/list instead?)
		entities = gen.GeneratePlants (chunkPos, biomes, heights, treeParent);
		
		// load in the chunk
		isGenerated = true;
		chunkManager.chunksToLoad.Enqueue (this);
	}

	public void GenerateColourTexture()
	{
		for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
			for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
				float height = heights[i, j];

				float temp = temps[i, j];
				temp = Mathf.InverseLerp (-80f, 80f, temp);
				temp *= bCalc.biomeColourMap.width;

				float humidity = humidities[i, j];
				humidity = 1 - humidity;
				humidity *= bCalc.biomeColourMap.width;
				Color color = bCalc.biomeColourMap.GetPixel ((int)temp, (int)humidity);

				if (height < -0.29f) {
					color = Consts.BIOME_COLOUR_DICT [BiomeType.Beach];
				}
				color.a = Mathf.Clamp01 (Mathf.InverseLerp (-1f, 1f, height)); // lower alpha is deeper
				terrainColours.SetPixel (i, j, color);
			}
		}
		terrainColours.Apply ();

		// get rid of lines between textures :D
		terrainColours.wrapMode = TextureWrapMode.Clamp;
	}

	public void GenerateTilesChunked ()
	{
		float heightVal;

		// in loop, enter all positions and tiles in arrays
		for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
			for (int j = 0; j < Consts.CHUNK_SIZE; j++) {

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

	public void GenerateDetailsChunk () {
		int randNum = worldSettings.PRNG.Next (0, 7);
		deetChunk = ChunkManager.tileResources.grassDetailsChunk [randNum];
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	public void LoadChunk () {
		if (isLoaded)
			return;
		// the chunk must be generated in order to load
		Assert.IsTrue (isGenerated);

		// set parent gameobject for the plants
		treeParent.SetActive (true);

		if (containsSand)
			chunkManager.sandTilemap.SetTiles (tilePositionsWorld, sandTileArray);

		if (containsGrass) {
			terrainChunk = UnityEngine.Object.Instantiate (bCalc.chunkTerrainPrefab, new Vector3Int (chunkPos.x + Consts.CHUNK_SIZE/2, chunkPos.y + Consts.CHUNK_SIZE/2, 199), Quaternion.identity);
			terrainChunk.transform.SetParent (bCalc.chunkTerrainParent.gameObject.transform);

			// generate a new material and apply it to the chunk
			Material newMat = new Material (Shader.Find ("Unlit/ChunkTerrainShader"));
			newMat.CopyPropertiesFromMaterial (bCalc.chunkTerrainMaterial);
			terrainChunk.GetComponent<SpriteRenderer> ().material = newMat;
			terrainChunk.GetComponent<SpriteRenderer> ().material.SetTexture ("_TileColours", terrainColours);

			// generate a new material for the grass blades in the chunk
			newMat = new Material (Shader.Find ("Unlit/ChunkTerrainShader"));
			newMat.CopyPropertiesFromMaterial (bCalc.chunkGrassMaterial);
			terrainChunk.GetComponentsInChildren<SpriteRenderer> ()[1].material = newMat;
			terrainChunk.GetComponentsInChildren<SpriteRenderer> ()[1].material.SetTexture ("_TileColours", terrainColours);
		}

		if (containsWater) {
			// generate a new material for the water in the chunk
			waterChunk = UnityEngine.Object.Instantiate (bCalc.chunkWaterPrefab, new Vector3Int (chunkPos.x + Consts.CHUNK_SIZE/2, chunkPos.y + Consts.CHUNK_SIZE/2, 198), Quaternion.identity);
			waterChunk.transform.SetParent (bCalc.chunkWaterParent.gameObject.transform);

			// generate a new material and apply it to the chunk
			Material newWaterMaterial = new Material (Shader.Find ("Unlit/ChunkWaterShader"));
			newWaterMaterial.CopyPropertiesFromMaterial (bCalc.chunkWaterMaterial);
			waterChunk.GetComponent<SpriteRenderer> ().material = newWaterMaterial;
			waterChunk.GetComponent<SpriteRenderer> ().material.SetTexture ("_TileColours", terrainColours);
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
			// chunkManager.waterTilemapChunked.SetTile (new Vector3Int (chunkCoord.x, chunkCoord.y, 0), null);
			waterChunk.SetActive(false);
		}
		isLoaded = false;
	}

	public bool IsGenerated () {
		return isGenerated;
	}

	public bool IsLoaded () {
		return isLoaded;
	}

	// lookup index of 16x16 2D array condensed to 1D array
	public int at (int x, int y) {
		return (x * 16 + y);
	}

	// lookup index of 64x64 2D array condensed to 1D array
	public int at64 (int x, int y) {
		return (x * 32 + y);
	}
}

