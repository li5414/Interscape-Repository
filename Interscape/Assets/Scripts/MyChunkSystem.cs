using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyChunkSystem : MonoBehaviour
{
	public static int seed = 40;
	static int chunkSize = 16;
	public static Vector2 viewerPosition;
	public Transform playerTrans;
	public Tilemap tilemapObj;

	// coordinates
	int currentChunkCoordX;
	int currentChunkCoordY;
	int lastChunkCoordX;
	int lastChunkCoordY;
	int renderDist = 4;

	// number generator
	System.Random prng = new System.Random (seed);
	static int octaves = 4;
	Vector2 [] octaveOffsets = new Vector2 [octaves]; // we want each octave to come from different location

	// chunk dictionary
	Dictionary<Vector2, MyChunkClass> terrainChunkDictionary = new Dictionary<Vector2, MyChunkClass> ();
	List<MyChunkClass> terrainChunksVisibleLastUpdate = new List<MyChunkClass> ();

	public void InitialiseOctavesForHeight ()
	{
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-10000, 10000); // too high numbers returns same value
			float offsetY = prng.Next (-10000, 10000);
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
			Debug.Log (offsetX);
			Debug.Log (offsetY);
		}
	}

	public float GetHeightValue (int x, int y, float scale, float persistance, float lacunarity)
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

	public float [,] GetHeightValues (int chunkX, int chunkY, float scale, float persistance, float lacunarity)
	{
		float [,] heights = new float [chunkSize, chunkSize];

		for (int x = 0)

		float height = 0;
		float amplitude = 1;
		float frequency = 1;

		// scale results in non-integer value
		for (int i = 0; i < octaves; i++) {
			float sampleX = chunkX / (scale / frequency) + octaveOffsets [i].x + 0.1f;
			float sampleY = chunkY + YieldInstruction) / (scale / frequency) + octaveOffsets [i].y + 0.1f;

			float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1; // make in range -1 to 1

			height += perlinValue * amplitude; // increase noise height each time
			amplitude *= persistance; // decreases each octave as persistance below 1
			frequency *= lacunarity; // increases each octave
		}
		return height;
	}

	public float GetTemperature (int x, int y, int mapDimension, float heightVal)
	{
		float temp;
		int lat = Mathf.Abs(y - (mapDimension / 2));
		float freq = 0.1f;
		int poleThresh = 100; // dist from equator
		int coldThresh = 80;
		int midThresh = 50;
		int warmThresh = 20;
		int equatorThresh = 10;

		// get noise based on seed
		float perlinValue = Mathf.PerlinNoise (x + octaveOffsets [1].x * freq + 0.1f, y + octaveOffsets [1].y * freq + 0.1f);
		perlinValue = perlinValue * 15; // make value larger so can subtract it

		// choose value based on latitude
		if (lat > poleThresh) {
			temp = -20 - perlinValue;
		}
		else if (lat <= poleThresh && lat > coldThresh) {
			temp = 0 - perlinValue;
		}
		else if (lat <= coldThresh && lat > midThresh) {
			temp = 20 - perlinValue;
		}
		else if (lat <= midThresh && lat > warmThresh) {
			temp = 40 - perlinValue;
		}
		else if (lat <= warmThresh && lat > equatorThresh) {
			temp = 50 - perlinValue;
		}
		else {
			temp = 55 - (perlinValue/2);
		}

		// choose value based on height
		if (heightVal > 0.2 || heightVal < -0.2) {
			temp -= 10;
		} else if (heightVal > 0.1 || heightVal < -0.1) {
			temp -= 5;
		}

		//Debug.Log ("Temp is " + temp + " degrees");
		return temp;
	}

	public float GetMoisture (int x, int y, float heightVal)
	{
		float freq = 0.1f;

		// get noise based on seed
		float offsetX = prng.Next (-100000, 100000); // too high numbers returns same value
		float offsetY = prng.Next (-100000, 100000);
		float perlinValue = Mathf.PerlinNoise (x + offsetX * freq, y + offsetY * freq);
		float moisture = perlinValue * 10; // in range 1 to 10
		moisture = moisture - heightVal;
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
		//InvokeRepeating ("GetTemp", 1.0f, 0.5f);
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

	void GetTemp ()
	{
		Vector2 pos = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		float heightVal = GetHeightValue ((int)pos.x, (int)pos.y, 61.4f, 0.5f, 2);
		Debug.Log ("Height is " + heightVal);
		GetTemperature ((int)pos.x, (int)pos.y, 200, heightVal);
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
					terrainChunkDictionary.Add (viewedChunkCoord, new MyChunkClass (pos, transform, seed, tilemapObj));
					// initialise/generate
					terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
				}

			}
		}
	}

	
}
