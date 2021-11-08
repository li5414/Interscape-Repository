using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomeCalculations : MonoBehaviour
{
	public Texture2D biomeColourMap;
	public Material chunkGrassMaterial;
	public Material chunkTerrainMaterial;
	public GameObject chunkTerrainPrefab;
	public GameObject chunkTerrainParent;
	public Material chunkWaterMaterial;
	public GameObject chunkWaterPrefab;
	public GameObject chunkWaterParent;

    // values relating to biome type table
	public static int BIOME_TABLE_SIZE = Consts.BIOME_TYPE_TABLE.GetLength(0);
	public static Vector2Int[] biomeTableCoords = new Vector2Int[BIOME_TABLE_SIZE * BIOME_TABLE_SIZE];

	// reference world settings script
	static WorldSettings worldSettings;

	// references / objects
	public Transform playerTrans;         // player reference
	public Tilemap tilemapObj;            // used as empty tilemap to instantiate
	public GameObject grid;
	public GameObject sandGrid;
	public GameObject waterGrid;
	public GameObject detailGrid;

	// perlin noise stuff
	public Vector2[] octaveOffsets; // we want each octave to come from different 'location' in the perlin noise
	static int octaves = 4;               // number of noise layers
	float scale = 361.4f;                 // the higher the number, the more 'zoomed in'. Needs to be likely to result in non-integer
	float persistance = 0.5f;             // the higher the octave, the less of an effect
	float lacunarity = 2.5f;              // value that decreases scale each octave

	// this needs to be in awake() or else bad stuff happens
	private void Awake ()
	{
		worldSettings = GameObject.Find ("System Placeholder").GetComponent<WorldSettings> ();

		//initialise biome table coords
		int count = 0;
		for (int i = 0; i < BIOME_TABLE_SIZE; i++) {
			for (int j = 0; j < BIOME_TABLE_SIZE; j++) {
				biomeTableCoords[count] = new Vector2Int(i, j);
			}
			count++;
		}
		initialiseOctavesForHeight();

		//InvokeRepeating ("PrintAtPos", 2.0f, 2.0f); //do not delete - is for testing!
	}

	void initialiseOctavesForHeight()
	{
		octaveOffsets = new Vector2 [octaves];
		for (int i = 0; i < octaves; i++)
		{
			float offsetX = worldSettings.PRNG.Next(-10000, 10000); // too high numbers returns same value
			float offsetY = worldSettings.PRNG.Next(-10000, 10000);
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}
	}

	public float GetHeightValue(int x, int y)
	{
		float height = 0;
		float amplitude = 1;
		float frequency = 1;

		// scale results in non-integer value
		for (int i = 0; i < octaves; i++)
		{
			float sampleX = x / (scale / frequency) + octaveOffsets[i].x + 0.1f;
			float sampleY = y / (scale / frequency) + octaveOffsets[i].y + 0.1f;

			float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // make in range -1 to 1

			height += perlinValue * amplitude; // increase noise height each time
			amplitude *= persistance; // decreases each octave as persistance below 1
			frequency *= lacunarity; // increases each octave
		}
		return height;
	}

	public float[,] GetHeightValues(int chunkX, int chunkY)
	{
		float[,] heights = new float[Consts.CHUNK_SIZE, Consts.CHUNK_SIZE];
		
		// loop through all tiles in chunk
		for (int x = 0; x < Consts.CHUNK_SIZE; x++) {
			for (int y = 0; y < Consts.CHUNK_SIZE; y++) {

				// reset values each layer
				float height = 0;
				float amplitude = 1;
				float frequency = 1;

				// scale results in non-integer value
				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (chunkX + x) / (scale / frequency) + octaveOffsets[i].x + 0.1f;
					float sampleY = (chunkY + y) / (scale / frequency) + octaveOffsets[i].y + 0.1f;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // make in range -1 to 1

					height += perlinValue * amplitude; // increase noise height each time
					amplitude *= persistance; // decreases each octave as persistance below 1
					frequency *= lacunarity; // increases each octave
				}
				heights[x, y] = height;
			}
		}
		return heights;
	}

	public float GetTemperature(int x, int y, float heightVal)
	{
		int lat = Mathf.Abs(y - (Consts.MAP_DIMENSION / 2)); // positive latitude at position given
		float temp;

		// putting a cap on latitude
		if (lat > (Consts.MAP_DIMENSION / 2)) {
			lat = (Consts.MAP_DIMENSION / 2);
		}

		// get noise based on seed
		float perlinValue = Mathf.PerlinNoise((x / scale) + octaveOffsets[1].x,
			(y / scale) + octaveOffsets[1].y) * 2 - 1; // make in range -1 to 1;
		perlinValue = perlinValue * 20; // make value larger so can directly subtract it

		// choose value based on latitude and height
		temp = 60 - perlinValue - (lat / 20f);
		temp -= 20 * (1 - heightVal);

		return temp;
	}

	public float[,] GetTemperatures(int chunkX, int chunkY, float[,] heights)
	{
		float[,] temps = new float[Consts.CHUNK_SIZE, Consts.CHUNK_SIZE];
		int lat;
		float temp;

		// loop through all tiles in chunk
		for (int x = 0; x < Consts.CHUNK_SIZE; x++) {
			for (int y = 0; y < Consts.CHUNK_SIZE; y++) {
				lat = Mathf.Abs((chunkY + y) - (Consts.MAP_DIMENSION / 2)); // positive latitude at tile position

				// putting a cap on latitude
				if (lat > (Consts.MAP_DIMENSION / 2)) {
					lat = (Consts.MAP_DIMENSION / 2);
				}

				// get noise based on seed
				float perlinValue = Mathf.PerlinNoise((chunkX + x) / scale + octaveOffsets[1].x,
					(chunkY + y) / scale + octaveOffsets[1].y) * 2 - 1; // make in range -1 to 1;
				perlinValue = perlinValue * 20; // make value larger so can directly subtract it

				// choose value based on latitude and height
				temp = 60 - perlinValue - (lat / 20f);
				temp -= 20 * (1 - heights[x, y]);

				temps[x, y] = temp;
			}
		}
		return temps;
	}

	// note: there is currently no noise layers for humidity
	public float GetHumidity(int x, int y, float heightVal)
	{
		float perlinValue = Mathf.PerlinNoise((x / (scale / 2)) + octaveOffsets[2].x, (y / (scale / 2)) + octaveOffsets[2].y);
		float moisture = perlinValue;

		float height = Mathf.InverseLerp(-1f, 1f, heightVal);
		moisture -= height / 3;
		moisture += 0.2f;
		return Mathf.Clamp01(moisture);
	}

	public float[,] GetHumidityArray(int chunkX, int chunkY, float[,] heights)
	{
		float[,] moistures = new float[Consts.CHUNK_SIZE, Consts.CHUNK_SIZE];
		float moisture;
		float perlinValue;

		// loop through all tiles in chunk
		for (int x = 0; x < Consts.CHUNK_SIZE; x++) {
			for (int y = 0; y < Consts.CHUNK_SIZE; y++) {
				perlinValue = Mathf.PerlinNoise((chunkX + x) / (scale / 2) + octaveOffsets[2].x, (chunkY + y) / (scale / 2) + octaveOffsets[2].y);
				moisture = perlinValue;

				float height = Mathf.InverseLerp(-1f, 1f, heights[x, y]); // get height in range 0-1
																		  //moisture *= (1 - height);
				moisture -= height / 3;
				moisture += 0.2f;
				moistures [x, y] = Mathf.Clamp01 (moisture);
			}
		}
		return moistures;
	}

	public BiomeType[,] GetBiomes(float[,] heights, float[,] temperatures, float[,] humidities)
	{
		BiomeType[,] biomeTypes = new BiomeType[Consts.CHUNK_SIZE, Consts.CHUNK_SIZE];
		float temp;
		int humidity;
		float height;

		// loop through all tiles in chunk
		for (int x = 0; x < Consts.CHUNK_SIZE; x++)
		{
			for (int y = 0; y < Consts.CHUNK_SIZE; y++)
			{
				height = heights[x, y];
				if (height >= -0.26) {
					// get temperature as an integer for easy lookup in biome array
					temp = Mathf.InverseLerp (-70f, 70f, temperatures [x, y]);
					temp = Mathf.Clamp01 (temp);
					temp *= BIOME_TABLE_SIZE;
					temp = Mathf.FloorToInt (temp);

					// putting a cap on vales so as not to over-index array
					humidity = Mathf.FloorToInt (humidities [x, y] * BIOME_TABLE_SIZE);

					biomeTypes [x, y] = Consts.BIOME_TYPE_TABLE [humidity, (int)temp];
				}
				else if (height < -0.3)
					biomeTypes [x, y] = BiomeType.Water;
				else
					biomeTypes [x, y] = BiomeType.Beach;
			}
		}
		return biomeTypes;
	}

	public BiomeType GetBiome(float height, float temperature, float humidity)
	{
		float temp;
		int humid;
		BiomeType biome;

		// get temperature as an integer for easy lookup in biome array
		temp = Mathf.InverseLerp(-70f, 70f, temperature);
		temp = Mathf.Clamp01 (temp);
		temp *= BIOME_TABLE_SIZE;
		temp = Mathf.FloorToInt(temp);

		humid = Mathf.FloorToInt (humidity * BIOME_TABLE_SIZE);
		biome = Consts.BIOME_TYPE_TABLE[humid, (int)temp];

		// water and beach biomes
		if (height < -0.6 && biome != BiomeType.Ice)
			biome = BiomeType.DeepWater;
		else if (height < -0.3 && biome != BiomeType.Ice)
			biome = BiomeType.Water;
		else if (height < -0.26 && biome != BiomeType.Ice)
			biome = BiomeType.Beach;
		return biome;
	}

	/*public Texture2D GenerateColourMap()
	{
		Debug.Log ("Generating colourmap for shader...");
		Texture2D tex = new Texture2D (5000, 5000);
		StartCoroutine (GeneratePartialColourMap (tex));
		return tex;
	}

	IEnumerator GeneratePartialColourMap (Texture2D tex)
	{
		for (int i = 0; i < tex.width; i++) {
			for (int j = 0; j < tex.width; j++) {
				float height = GetHeightValue (i, j);

				float temp = GetTemperature (i, j, height);
				temp = Mathf.InverseLerp (-80f, 80f, temp);
				temp *= biomeColourMap.width;

				float humidity = GetHumidity (i, j, height);
				humidity = 1 - humidity;
				humidity *= biomeColourMap.width;
				Color color = biomeColourMap.GetPixel ((int)temp, (int)humidity);

				if (height < -0.29f) {
					color = Consts.BIOME_COLOUR_DICT [BiomeType.Beach];
					// could add water color here for minimap?
				}

				color.a = Mathf.Clamp01 (Mathf.InverseLerp (-1f, 1f, height)); // lower alpha is deeper
				tex.SetPixel (i, j, color);
			}
			yield return null;
		}

		Debug.Log ("Finished generating colourmap. Saving...");
		SavePNG (tex);
		Debug.Log ("Saved!");
	}

	void SavePNG (Texture2D tex)
	{
		byte [] bytes = tex.EncodeToPNG ();
		var dirPath = Application.dataPath + "/Resources/Sprites/Map/";
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		File.WriteAllBytes (dirPath + "GiantColourMap.png", bytes);
	}*/

	// for testing purposes
	void PrintAtPos()
	{
		Vector2 pos = new Vector2(playerTrans.position.x, playerTrans.position.y);
		float heightVal = GetHeightValue((int)pos.x, (int)pos.y);
		Debug.Log ("Height is " + heightVal);
		float temp = GetTemperature((int)pos.x, (int)pos.y, heightVal);
		Debug.Log ("Temp is " + temp);
		float humidity = GetHumidity((int)pos.x, (int)pos.y, heightVal);
		Debug.Log ("Humidity is " + humidity + "%");
		BiomeType biome = GetBiome(heightVal, temp, humidity);
		Debug.Log("You are in a " + biome + "biome");
	}
}
