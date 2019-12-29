using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyChunkSystem : MonoBehaviour
{
	public static int seed = 2;
	public int renderDist = 4;            // no. chunks
	public static int mapDimension = 5000; // no. tiles
	static int chunkSize = 16;            // no. tiles
	public Transform playerTrans;         // player reference
	public Tilemap tilemapObj;            // used as empty tilemap to instantiate

	// coordinate variables
	public static Vector2 viewerPosition;
	int currentChunkCoordX;
	int currentChunkCoordY;
	int lastChunkCoordX;
	int lastChunkCoordY;

	// number generator and perlin noise stuff
	System.Random prng = new System.Random (seed);
	Vector2 [] octaveOffsets = new Vector2 [octaves]; // we want each octave to come from different 'location' in the perlin noise
	static int octaves = 4;               // number of noise layers
	public float scale = 361.4f;          // the higher the number, the more 'zoomed in'. Needs to be likely to result in non-integer
	float persistance = 0.5f;             // the higher the octave, the less of an effect
	float lacunarity = 2.5f;              // value that decreases scale each octave

	// chunk dictionary
	Dictionary<Vector2, MyChunkClass> terrainChunkDictionary = new Dictionary<Vector2, MyChunkClass> ();
	List<MyChunkClass> terrainChunksVisibleLastUpdate = new List<MyChunkClass> ();

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

	public float GetTemperature (int x, int y, float heightVal)
	{
		int lat = Mathf.Abs(y - (mapDimension / 2)); // positive latitude at position given
		float temp;
		float freq = 0.1f;

		// putting a cap on latitude
		if (lat > (mapDimension / 2)) {
			lat = (mapDimension / 2);
		}

		// get noise based on seed
		float perlinValue = Mathf.PerlinNoise ((x / scale) + octaveOffsets [1].x + freq,
			(y / scale) + octaveOffsets [1].y + freq) * 2 - 1; // make in range -1 to 1;
		perlinValue = perlinValue * 20; // make value larger so can directly subtract it

		// choose value based on latitude and height
		temp = 60 - perlinValue - (lat / 20f);
		temp -= 20 * (1 - Mathf.Abs (heightVal));

		return temp;
	}

	public float [,] GetTemperatures (int chunkX, int chunkY, float [,] heights)
	{
		float [,] temps = new float [chunkSize, chunkSize];
		int lat;
		float temp;
		float freq = 0.1f;

		// loop through all tiles in chunk
		for (int x = 0; x < chunkSize; x++) {
			for (int y = 0; y < chunkSize; y++) {

				lat = Mathf.Abs ((chunkY + y) - (mapDimension / 2)); // positive latitude at tile position

				// putting a cap on latitude
				if (lat > (mapDimension / 2)) {
					lat = (mapDimension / 2);
				}

				// get noise based on seed
				float perlinValue = Mathf.PerlinNoise ((chunkX + x / scale) + octaveOffsets [1].x + freq,
					(chunkY + y / scale) + octaveOffsets [1].y + freq) * 2 - 1; // make in range -1 to 1;
				perlinValue = perlinValue * 20; // make value larger so can directly subtract it

				// choose value based on latitude and height
				temp = 60 - perlinValue - (lat / 20f);
				temp -= 20 * (1 - Mathf.Abs (heights[x, y]));
				temps [x, y] = temp;
			}
		}
		return temps;
	}

	public float GetMoisture (int x, int y, float heightVal)
	{
		float freq = 0.1f;

		// get noise based on offset, which is based on seed
		float offsetX = prng.Next (-10000, 10000); 
		float offsetY = prng.Next (-10000, 10000);
		float perlinValue = Mathf.PerlinNoise (x / scale + offsetX + freq, y / scale + offsetY + freq);
		float moisture = perlinValue * 6; // in range 0 to 6

		moisture *= 1 - Mathf.Abs(heightVal); // should be more humid at sea level
		return moisture;
	}

	public enum BiomeType {
		Desert,
		Savanna,
		TropicalRainforest,
		Grassland,
		Woodland,
		SeasonalForest,
		TemperateRainforest,
		BorealForest,
		Tundra,
		Ice
	}

	BiomeType [,] BiomeTable = new BiomeType [6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
	};

	/*public BiomeType GetBiomeType (int x, int y)
	{
		float height = GetHeightValue (x, y, seed, 27.6f, 4, 0.5f, 2, new Vector2(0, 0));
		float temp = GetTemperature (x, y, seed, 200, height);
		float moisture = GetMoisture (x, y, seed, height);
		return BiomeTable [(int)moisture, (int)temp];
	}*/

	private void Start ()
	{
		InitialiseOctavesForHeight ();
		UpdateVisibleChunks ();
		InvokeRepeating ("PrintTempAtPos", 1.0f, 0.5f);
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
	void PrintTempAtPos ()
	{
		Vector2 pos = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		float heightVal = GetHeightValue ((int)pos.x, (int)pos.y);
		//Debug.Log ("Height is " + heightVal);
		float temp = GetTemperature ((int)pos.x, (int)pos.y, heightVal);
		Debug.Log ("Temp is " + temp);
	}

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
