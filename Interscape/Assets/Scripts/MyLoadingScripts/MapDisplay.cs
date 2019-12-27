using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
	public Renderer textureRender;

	public void DrawNoiseMap(float[,] noiseMap)
	{
		int width = noiseMap.GetLength (0);
		int height = noiseMap.GetLength (1);

		Texture2D texture = new Texture2D (width, height);

		// create and assign values to colour array
		Color [] colourMap = new Color [width * height];
		for(int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
			}
		}

		// set pixels
		texture.SetPixels (colourMap);
		texture.Apply ();

		// renders texture outside of game mode
		textureRender.sharedMaterial.mainTexture = texture;

		// set texture the same size as map size
		textureRender.transform.localScale = new Vector3 (width, 1, height);
	}
}
