using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomeCalculations : MonoBehaviour
{
	public enum BiomeType
	{
		Desert,
		Savanna,
		Rainforest,
		Grassland,
		SeasonalForest,
		Taiga,
		Tundra,
		Ice,
		Water,
		DeepWater,
		Beach
	}

	public Texture2D biomeColourMap;

	public static BiomeType[,] BiomeTable = {   
    //                                               <--Colder      Hotter -->            
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert},   // Dryest
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert},
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert },
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Savanna,    BiomeType.Desert },
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Grassland,  BiomeType.Savanna,    BiomeType.Desert },
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },  // Wettest
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert }
	};

    // currently unused colours
	public static BiomeType[] BiomeTable2 =  
    //                 <--Lower                    Higher -->            
    { BiomeType.DeepWater, BiomeType.DeepWater, BiomeType.Water, BiomeType.Water, BiomeType.Water, BiomeType.Beach};

    // values relating to BiomeTable
	public static int tableSize = 8;
	public static Vector2Int[] coords = new Vector2Int[tableSize * tableSize];

	// reference other scripts
	ChunkManager chunkManager;

	// important numbers
	static int mapDimension = ChunkManager.mapDimension;
	static int chunkSize = ChunkManager.chunkSize;

	// references / objects
	public Transform playerTrans;         // player reference
	public Tilemap tilemapObj;            // used as empty tilemap to instantiate
	public GameObject grid;
	public GameObject sandGrid;
	public GameObject waterGrid;
	public GameObject detailGrid;

	// number generator and perlin noise stuff
	System.Random prng;
	public Vector2[] octaveOffsets = new Vector2[octaves]; // we want each octave to come from different 'location' in the perlin noise
	static int octaves = 4;               // number of noise layers
	float scale = 361.4f;                 // the higher the number, the more 'zoomed in'. Needs to be likely to result in non-integer
	float persistance = 0.5f;             // the higher the octave, the less of an effect
	float lacunarity = 2.5f;              // value that decreases scale each octave

	// colour dict
	public static Dictionary<BiomeType, Color32> BiomeColours = new Dictionary<BiomeType, Color32>();

	// color texture
	public Texture2D GiantColourMap;


	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	private void Awake()
	{
		chunkManager = GameObject.Find ("System Placeholder").GetComponent<ChunkManager> ();
		prng = chunkManager.prng;

		// initialise biome colours dictionary
		BiomeColours.Add(BiomeType.Water, new Color32(116, 144, 183, 255));
		BiomeColours.Add(BiomeType.DeepWater, new Color32(88, 115, 159, 255));
		BiomeColours.Add(BiomeType.Beach, new Color32(229, 209, 168, 255));

		BiomeColours.Add (BiomeType.Desert, new Color32 (229, 204, 159, 255));
		BiomeColours.Add(BiomeType.Savanna, new Color32(246, 226, 176, 255));
		//BiomeColours.Add(BiomeType.Rainforest, new Color32(90, 181, 137, 255));
		BiomeColours.Add (BiomeType.Rainforest, new Color32 (69, 163, 117, 255));
		BiomeColours.Add(BiomeType.Grassland, new Color32(185, 205, 147, 255));
		//BiomeColours.Add(BiomeType.SeasonalForest, new Color32(117, 173, 141, 255)); //75ad8d
		BiomeColours.Add (BiomeType.SeasonalForest, new Color32 (130, 181, 146, 255)); //82b592
		//BiomeColours.Add(BiomeType.Taiga, new Color32(112, 168, 155, 255));
		BiomeColours.Add(BiomeType.Taiga, new Color32(126, 166, 142, 255));
		BiomeColours.Add(BiomeType.Tundra, new Color32(144, 179, 164, 255));
		BiomeColours.Add (BiomeType.Ice, new Color32 (218, 231, 235, 255));

		int count = 0;
		for (int i = 0; i < tableSize; i++) {
			for (int j = 0; j < tableSize; j++) {
				coords[count] = new Vector2Int(i, j);
			}
			count++;
		}

		InitialiseOctavesForHeight();
		
	}

	private void Start ()
	{
		GiantColourMap = Resources.Load<Texture2D> ("Sprites/Map/GiantColourMap");
		if (GiantColourMap == null) {
			GenerateColourMap ();
			GiantColourMap = Resources.Load<Texture2D> ("Sprites/Map/GiantColourMap");
			if (GiantColourMap == null) {
				Debug.Log ("error: colours not found??");
			}
		}


		//InvokeRepeating ("PrintAtPos", 2.0f, 2.0f); //do not delete - is for testing!
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void InitialiseOctavesForHeight()
	{
		octaveOffsets = new Vector2 [octaves];
		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-10000, 10000); // too high numbers returns same value
			float offsetY = prng.Next(-10000, 10000);
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
		float[,] heights = new float[chunkSize, chunkSize];

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{

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

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public float GetTemperature(int x, int y, float heightVal)
	{
		int lat = Mathf.Abs(y - (mapDimension / 2)); // positive latitude at position given
		float temp;

		// putting a cap on latitude
		if (lat > (mapDimension / 2)) {
			lat = (mapDimension / 2);
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
		float[,] temps = new float[chunkSize, chunkSize];
		int lat;
		float temp;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {
				lat = Mathf.Abs((chunkY + y) - (mapDimension / 2)); // positive latitude at tile position

				// putting a cap on latitude
				if (lat > (mapDimension / 2)) {
					lat = (mapDimension / 2);
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

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	// IMPLEMENT NOISE LAYERS??

	public float GetHumidity(int x, int y, float heightVal)
	{
		float perlinValue = Mathf.PerlinNoise((x / (scale / 2)) + octaveOffsets[2].x, (y / (scale / 2)) + octaveOffsets[2].y);
		float moisture = perlinValue;
		//float moisture = perlinValue * tableSize; // in range for array lookup

		float height = Mathf.InverseLerp(-1f, 1f, heightVal);
		//moisture *= (1 - height);
		moisture -= height / 3;
		moisture += 0.2f;
		return Mathf.Clamp01(moisture);
	}

	public float[,] GetHumidityArray(int chunkX, int chunkY, float[,] heights)
	{
		float[,] moistures = new float[chunkSize, chunkSize];
		float moisture;
		float perlinValue;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {
				perlinValue = Mathf.PerlinNoise((chunkX + x) / (scale / 2) + octaveOffsets[2].x, (chunkY + y) / (scale / 2) + octaveOffsets[2].y);
				moisture = perlinValue;
				//moisture = perlinValue * tableSize; // in range for array lookup

				float height = Mathf.InverseLerp(-1f, 1f, heights[x, y]); // get height in range 0-1
																		  //moisture *= (1 - height);
				moisture -= height / 3;
				moisture += 0.2f;
				moistures [x, y] = Mathf.Clamp01 (moisture);
			}
		}
		return moistures;
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public BiomeType[,] GetBiomes(float[,] heights, float[,] temperatures, float[,] humidities)
	{
		BiomeType[,] biomeTypes = new BiomeType[chunkSize, chunkSize];
		float temp;
		int humidity;
		float height;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				height = heights[x, y];

				
				if (height >= -0.26) {
					// get temperature as an integer for easy lookup in biome array
					temp = Mathf.InverseLerp (-70f, 70f, temperatures [x, y]);
					temp = Mathf.Clamp01 (temp);
					temp *= tableSize;
					temp = Mathf.FloorToInt (temp);

					// putting a cap on vales so as not to over-index array
					humidity = Mathf.FloorToInt (humidities [x, y] * tableSize);

					biomeTypes [x, y] = BiomeTable [humidity, (int)temp];
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
		temp *= tableSize;
		temp = Mathf.FloorToInt(temp);

		
		humid = Mathf.FloorToInt (humidity * tableSize);

		biome = BiomeTable[humid, (int)temp];

		// water and beach biomes
		if (height < -0.6 && biome != BiomeType.Ice)
			biome = BiomeType.DeepWater;
		else if (height < -0.3 && biome != BiomeType.Ice)
			biome = BiomeType.Water;
		else if (height < -0.26 && biome != BiomeType.Ice)
			biome = BiomeType.Beach;

		return biome;
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public Texture2D GenerateColourMap()
	{
		Texture2D tex = new Texture2D (5000, 5000);
		for (int i = 0; i < tex.width; i ++) {
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
					color = BiomeColours [BiomeType.Beach];
					// could add water color here for minimap?
				}

				color.a = Mathf.Clamp01 (Mathf.InverseLerp (-1f, 1f, height)); // lower alpha is deeper

				tex.SetPixel (i, j, color);
			}
		}
		SavePNG (tex);
		return tex;
	}

	void SavePNG (Texture2D tex)
	{
		byte [] bytes = tex.EncodeToPNG ();
		var dirPath = Application.dataPath + "/Resources/Sprites/Map/";
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		File.WriteAllBytes (dirPath + "GiantColourMap.png", bytes);
	}

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
