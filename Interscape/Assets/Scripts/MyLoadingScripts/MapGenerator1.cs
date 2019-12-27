using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator1 : MonoBehaviour {

	public enum DrawMode { NoiseMap, ColourMap};
	public DrawMode drawMode;

	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public int octaves;
	public float persistance; // in range 0 to 1
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public TerrainType [] regions;

	public void GenerateMap()
	{
		float [,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed,
			noiseScale, octaves, persistance, lacunarity, offset);

		Color [] colourMap = new Color [mapWidth * mapHeight];

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions[i].height) {
						colourMap [y * mapWidth + x] = regions [i].colour;
						break;
					}
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay> ();

		if ( drawMode == DrawMode.NoiseMap) {
			display.DrawNoiseMap (noiseMap);
		}
		else if (drawMode == DrawMode.ColourMap) {

		}
		
	}
}

[System.Serializable] // shows up in inspector
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}