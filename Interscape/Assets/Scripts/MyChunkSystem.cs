using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyChunkSystem : MonoBehaviour
{
	public enum BiomeType {
		Desert,
		Savanna,
		Rainforest,
		Swamp,
		Grassland,
		SeasonalForest,
		Taiga,
		Tundra,
		Ice,
		Water,
		DeepWater,
		Beach
	}
	// MAKE ARRAY BIGGER AND MORE DETAILED FOR BETTER BIOME BLENDING
	public static BiomeType [,] BiomeTable = {   
    //                 <--Colder                                                      // Hotter -->            
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland, BiomeType.Grassland,      BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert},     // Dryest
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland, BiomeType.SeasonalForest, BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert},     
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland, BiomeType.SeasonalForest, BiomeType.Savanna,    BiomeType.Savanna,    BiomeType.Desert },    
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,     BiomeType.SeasonalForest, BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },   
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,     BiomeType.SeasonalForest, BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert }, 
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,     BiomeType.Rainforest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },       // Wettest
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,     BiomeType.Rainforest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,     BiomeType.Rainforest,     BiomeType.Swamp,      BiomeType.Savanna,    BiomeType.Desert }
	};

	public static int tableSize = 8;
	public static Vector2Int [] specialCoords = new Vector2Int [tableSize * tableSize]; /*= {
		new Vector2Int (0, 5), new Vector2Int (1, 0),

		new Vector2Int (1, 2), new Vector2Int (2, 1),
		new Vector2Int (2, 5), new Vector2Int (3, 3),
		new Vector2Int (4, 0), new Vector2Int (4, 2),
		new Vector2Int (5, 4), new Vector2Int (5, 5)};
		*/
	//    0 1 2 3 4 5
	// 0   | | | | |X
	// 1  X| |X| | |
	// 2   |X| | | |X
	// 3   | | |X| |
	// 4  X| |X| | |
	// 5   | | | |X|X

	// important values
	public static int seed = 29;
	public int renderDist = 8;            // no. chunks
	public static int mapDimension = 5000; // no. tiles
	static int chunkSize = 16;            // no. tiles

	// references / objects
	public Transform playerTrans;         // player reference
	public Tilemap tilemapObj;            // used as empty tilemap to instantiate

	// coordinate variables
	public static Vector2 viewerPosition;
	int currentChunkCoordX;
	int currentChunkCoordY;
	int lastChunkCoordX;
	int lastChunkCoordY;

	// number generator and perlin noise stuff
	public static System.Random prng = new System.Random (seed);
	Vector2 [] octaveOffsets = new Vector2 [octaves]; // we want each octave to come from different 'location' in the perlin noise
	static int octaves = 4;               // number of noise layers
	public float scale = 361.4f;          // the higher the number, the more 'zoomed in'. Needs to be likely to result in non-integer
	float persistance = 0.5f;             // the higher the octave, the less of an effect
	float lacunarity = 2.5f;              // value that decreases scale each octave

	// chunk dictionary
	Dictionary<Vector2, MyChunkClass> terrainChunkDictionary = new Dictionary<Vector2, MyChunkClass> ();
	List<MyChunkClass> terrainChunksVisibleLastUpdate = new List<MyChunkClass> ();

	// colour dict
	public static Dictionary<BiomeType, Color32> BiomeColours = new Dictionary<BiomeType, Color32> ();

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public void InitialiseOctavesForHeight ()
	{
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-10000, 10000); // too high numbers returns same value
			float offsetY = prng.Next (-10000, 10000);
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}
	}

	public float GetHeightValue (int x, int y)
	{
		float height = 0;
		float amplitude = 1;
		float frequency = 1;

		// scale results in non-integer value
		for (int i = 0; i < octaves; i++) {
			float sampleX = x / (scale / frequency) + octaveOffsets [i].x + 0.1f;
			float sampleY = y / (scale / frequency) + octaveOffsets [i].y + 0.1f;
			
			float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1; // make in range -1 to 1
																				
			height += perlinValue * amplitude; // increase noise height each time
			amplitude *= persistance; // decreases each octave as persistance below 1
			frequency *= lacunarity; // increases each octave
		}
		return height;
	}

	public float [,] GetHeightValues (int chunkX, int chunkY)
	{
		float [,] heights = new float [chunkSize, chunkSize];

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {

				// reset values each layer
				float height = 0;
				float amplitude = 1;
				float frequency = 1;

				// scale results in non-integer value
				for (int i = 0; i < octaves; i++) {
					float sampleX = (chunkX + x) / (scale / frequency) + octaveOffsets [i].x + 0.1f;
					float sampleY = (chunkY + y) / (scale / frequency) + octaveOffsets [i].y + 0.1f;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1; // make in range -1 to 1

					height += perlinValue * amplitude; // increase noise height each time
					amplitude *= persistance; // decreases each octave as persistance below 1
					frequency *= lacunarity; // increases each octave
				}
				heights [x, y] = height;
			}
		}
		return heights;
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public float GetTemperature (int x, int y, float heightVal)
	{
		int lat = Mathf.Abs(y - (mapDimension / 2)); // positive latitude at position given
		float temp;

		// putting a cap on latitude
		if (lat > (mapDimension / 2)) {
			lat = (mapDimension / 2);
		}

		// get noise based on seed
		float perlinValue = Mathf.PerlinNoise ((x / scale) + octaveOffsets [1].x,
			(y / scale) + octaveOffsets [1].y) * 2 - 1; // make in range -1 to 1;
		perlinValue = perlinValue * 20; // make value larger so can directly subtract it

		// choose value based on latitude and height
		temp = 60 - perlinValue - (lat / 20f);
		if (heightVal >= 0)
			temp -= 20 * (1 - heightVal);
		else
			temp -= 10 * (1 - Mathf.Abs (heightVal)) / 2;

		return temp;
	}

	public float [,] GetTemperatures (int chunkX, int chunkY, float [,] heights)
	{
		float [,] temps = new float [chunkSize, chunkSize];
		int lat;
		float temp;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {

				lat = Mathf.Abs ((chunkY + y) - (mapDimension / 2)); // positive latitude at tile position

				// putting a cap on latitude
				if (lat > (mapDimension / 2)) {
					lat = (mapDimension / 2);
				}

				// get noise based on seed
				float perlinValue = Mathf.PerlinNoise ((chunkX + x) / scale + octaveOffsets [1].x,
					(chunkY + y) / scale + octaveOffsets [1].y) * 2 - 1; // make in range -1 to 1;
				perlinValue = perlinValue * 20; // make value larger so can directly subtract it

				// choose value based on latitude and height
				temp = 60 - perlinValue - (lat / 20f);
				if (heights [x, y] >= 0)
					temp -= 20 * (1 - heights [x, y]);
				else
					temp -= 10 * (1 - Mathf.Abs (heights [x, y])) / 2;

				temps [x, y] = temp;
			}
		}
		return temps;
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	// IMPLEMENT NOISE LAYERS


	public float GetHumidity (int x, int y, float heightVal)
	{
		float perlinValue = Mathf.PerlinNoise ((x / (scale/2)) + octaveOffsets [2].x, (y / (scale/2)) + octaveOffsets [2].y);
		float moisture = perlinValue * tableSize; // in range for array lookup

		float height = Mathf.InverseLerp (-1f, 1f, heightVal);
		moisture *= (1-height);
		return moisture;
	}

	public float [,] GetHumidityArray (int chunkX, int chunkY, float [,] heights)
	{
		float [,] moistures = new float [chunkSize, chunkSize];
		float moisture;
		float perlinValue;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {
				perlinValue = Mathf.PerlinNoise ((chunkX + x) / (scale/2) + octaveOffsets [2].x, (chunkY + y) / (scale/2) + octaveOffsets [2].y);
				moisture = perlinValue * tableSize; // in range for array lookup

				float height = Mathf.InverseLerp (-1f, 1f, heights[x, y]);
				moisture *= (1-height);
				moistures [x, y] = moisture;
			}
		}
		return moistures;
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	
	public BiomeType [,] GetBiomes (float [,] heights, float [,] temperatures, float [,] humidities)
	{
		BiomeType [,] biomeTypes = new BiomeType [chunkSize, chunkSize];
		int temp;
		int humidity;
		float height;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {
				height = heights [x, y];

				// get temperature as a value in the range 0-5 for easy lookup in array
				if (temperatures [x, y] > 40)
					temp = 5;
				else if (temperatures [x, y] > 30)
					temp = 4;
				else if (temperatures [x, y] > 20)
					temp = 3;
				else if (temperatures [x, y] > 5)
					temp = 2;
				else if (temperatures [x, y] > -15)
					temp = 1;
				else {
					temp = 0;
				}

				// putting a cap on vales so as not to over-index array
				humidity = Mathf.FloorToInt (humidities [x, y]);
				if (humidity < 0)
					humidity = 0;
				if (humidity > 5)
					humidity = 5;

				biomeTypes [x, y] = BiomeTable [humidity, temp];

				// water and beach biomes
				if (height < -0.6 && biomeTypes [x, y] != MyChunkSystem.BiomeType.Ice)
					biomeTypes [x, y] = BiomeType.DeepWater;
				else if (height < -0.3 && biomeTypes [x, y] != MyChunkSystem.BiomeType.Ice)
					biomeTypes [x, y] = BiomeType.Water;
				else if (height < -0.26 && biomeTypes [x, y] != MyChunkSystem.BiomeType.Ice)
					biomeTypes [x, y] = BiomeType.Beach;
			}
		}

		return biomeTypes;
	}

	public BiomeType GetBiome (float height, float temperature, float humidity)
	{

		int temp;
		int humid;
		BiomeType biome;

		// get temperature as a value in the range 0-5 for easy lookup in array
		if (temperature > 40)
			temp = 5;
		else if (temperature > 30)
			temp = 4;
		else if (temperature > 20)
			temp = 3;
		else if (temperature > 5)
			temp = 2;
		else if (temperature > -15)
			temp = 1;
		else {
			temp = 0;
		}

		// putting a cap on vales so as not to over-index array
		humid = Mathf.FloorToInt (humidity);
		if (humid < 0)
			humid = 0;
		if (humid > 5)
			humid = 5;

		biome = BiomeTable [humid, temp];

		// water and beach biomes
		if (height < -0.6 && biome != MyChunkSystem.BiomeType.Ice)
			biome = BiomeType.DeepWater;
		else if (height < -0.3 && biome != MyChunkSystem.BiomeType.Ice)
			biome = BiomeType.Water;
		else if (height < -0.26 && biome != MyChunkSystem.BiomeType.Ice)
			biome = BiomeType.Beach;
			
		return biome;
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	private void Start ()
	{
		// initialise biome colours dictionary
		BiomeColours.Add (BiomeType.Water, new Color32 (116, 144, 183, 255));
		BiomeColours.Add (BiomeType.DeepWater, new Color32 (88, 115, 159, 255));
		BiomeColours.Add (BiomeType.Beach, new Color32 (231, 213, 173, 255));
		//BiomeColours.Add (BiomeType.Desert, new Color32 (254, 234, 184, 255));
		BiomeColours.Add (BiomeType.Desert, new Color32 (255, 169, 100, 255));
		//BiomeColours.Add (BiomeType.Savanna, new Color32 (206, 206, 158, 255));
		BiomeColours.Add (BiomeType.Savanna, new Color32 (254, 234, 184, 255));
		BiomeColours.Add (BiomeType.Rainforest, new Color32 (94, 185, 141, 255));
		BiomeColours.Add (BiomeType.Swamp, new Color32 (124, 145, 116, 255));
		BiomeColours.Add (BiomeType.Grassland, new Color32 (189, 210, 151, 255));
		BiomeColours.Add (BiomeType.SeasonalForest, new Color32 (117, 173, 141, 255));
		BiomeColours.Add (BiomeType.Taiga, new Color32 (112, 168, 155, 255));
		BiomeColours.Add (BiomeType.Tundra, new Color32 (151, 183, 171, 255));
		//BiomeColours.Add (BiomeType.Ice, new Color32 (214, 227, 231, 255));
		BiomeColours.Add (BiomeType.Ice, new Color32 (255, 255, 255, 255));

		int count = 0;
		for (int i = 0; i < tableSize; i++) {
			for (int j = 0; j < tableSize; j++) {
				specialCoords[count] = new Vector2Int (i, j);
				count++;
			}

		}

		InitialiseOctavesForHeight ();
		UpdateVisibleChunks ();

		InvokeRepeating ("PrintAtPos", 1.0f, 0.5f);
	}

	void Update ()
	{
		// get viewer and chunk position
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoordX = (Mathf.RoundToInt (viewerPosition.x / chunkSize)) * chunkSize;
		currentChunkCoordY = (Mathf.RoundToInt (viewerPosition.y / chunkSize)) * chunkSize;

		// if player moved to a different chunk, update chunks
		if ((currentChunkCoordX != lastChunkCoordX) ||  (currentChunkCoordY != lastChunkCoordY)){
			UpdateVisibleChunks ();
			lastChunkCoordX = currentChunkCoordX;
			lastChunkCoordY = currentChunkCoordY;
		}
		
	}

	// for testing purposes
	void PrintAtPos ()
	{
		Vector2 pos = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		float heightVal = GetHeightValue ((int)pos.x, (int)pos.y);
		//Debug.Log ("Height is " + heightVal);
		float temp = GetTemperature ((int)pos.x, (int)pos.y, heightVal);
		//Debug.Log ("Temp is " + temp);
		float humidity = GetHumidity ((int)pos.x, (int)pos.y, heightVal);
		Debug.Log ("Humidity is " + humidity);
		BiomeType biome = GetBiome (heightVal, temp, humidity);
		Debug.Log ("You are in a " + biome);
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	void UpdateVisibleChunks ()
	{
		// clear all visible chunks
		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate[i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		// go through neighbouring chunks that need to be rendered
		for (int x = -renderDist * chunkSize; x <= renderDist * chunkSize; x += chunkSize) {
			for (int y = -renderDist * chunkSize; y <= renderDist * chunkSize; y += chunkSize) {
				Vector2Int viewedChunkCoord = new Vector2Int (currentChunkCoordX + x, currentChunkCoordY + y);

				// if chunk has been encountered before
				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					if (terrainChunkDictionary [viewedChunkCoord].IsVisible () == false) {
						terrainChunkDictionary [viewedChunkCoord].SetVisible (true);
					}
					terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
				}
				else {
					// add chunks coordinates to dictionary
					Vector3Int pos = new Vector3Int (currentChunkCoordX + x, currentChunkCoordY + y, 200);
					terrainChunkDictionary.Add (viewedChunkCoord, new MyChunkClass (pos, seed, tilemapObj));
					terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
				}

			}
		}
	}

	
}
