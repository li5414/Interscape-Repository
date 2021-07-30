using UnityEngine;
using System.Collections;

public static class Noise {

	public static float [,] GenerateNoiseMap (int mapWidth, int mapHeight, int seed,
		float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float [,] noiseMap = new float [mapWidth, mapHeight];

		System.Random prng = new System.Random (seed); // pseudo random num generator
		Vector2 [] octaveOffsets = new Vector2 [octaves]; // we want each octave to come from different location
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x; // too high numbers returns same value
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		// avoiding erronus values for scale, clamping to minimum value
		if (scale <= 0) {
			scale = 0.0001f;
		}

		// to keep track of max and mins
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		// to 'center' the noise
		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				// scale results in non-ineger value
				for (int i = 0; i < octaves; i++) {
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets [i].y; 

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1; // make in range -1 to 1
					// increase noise height each time
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance; // decreases each octave as persistance below 1
					frequency *= lacunarity; // increases each octave
				}

				// keep track of max for normalisation later
				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				noiseMap [x, y] = noiseHeight;
			}
		}

		// normalise values
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}

		return noiseMap;
	}

}