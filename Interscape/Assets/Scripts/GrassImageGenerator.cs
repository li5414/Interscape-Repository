using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GrassImageGenerator : MonoBehaviour
{
	private Texture2D texture;
	private Sprite sprite;
	private int imageSize = 256;
	private System.Random prng;
	private Color32 lightColor = new Color32(245, 244, 245, 255);
	private Color32 darkColor = new Color32 (234, 232, 235, 255);
	private int count = 0;


	// Start is called before the first frame update
	void Start()
    {
		prng = new System.Random ();
		Setup ();
		DrawAllGrass ();
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
			Setup ();
			DrawAllGrass ();
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			SavePNG ();
		}
	}

	void SavePNG ()
	{
		byte [] bytes = texture.EncodeToPNG ();
		var dirPath = Application.dataPath + "/GrassImages/";
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		File.WriteAllBytes (dirPath + "GrassChunk" + count + ".png", bytes);
		count++;
	}

	void Setup ()
	{
		texture = new Texture2D (imageSize, imageSize, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		sprite = Sprite.Create (texture, new Rect (0, 0, imageSize, imageSize), Vector2.zero);
		GetComponent<SpriteRenderer> ().sprite = sprite;

		Color fillColor = Color.clear;
		Color [] fillPixels = new Color [texture.width * texture.height];

		for (int i = 0; i < fillPixels.Length; i++) {
			fillPixels [i] = fillColor;
		}

		texture.SetPixels (fillPixels);


		texture.Apply ();
	}

	void DrawAllGrass ()
	{
		int sizeFactor = 4;
		for (int j = imageSize - 1; j >= 3; j -= 1/*sizeFactor*/) {
			for (int i = 0; i < imageSize - 2; i += 1/*sizeFactor*/) { 
				int randNum = prng.Next (0, 90);

				switch (randNum) {
					case 0:
						DrawType1(i, j);
						break;
					case 1:
						DrawType2 (i, j);
						break;
					case 3:
						DrawType3 (i, j);
						break;
					case 4:
						DrawType4 (i, j);
						break;
				}

			}
		}
		texture.Apply ();
	}

	void DrawType1(int atX, int atY)
	{
		texture.SetPixel (atX, atY, lightColor);
		texture.SetPixel (atX, atY - 1, darkColor);
		texture.SetPixel (atX, atY - 2, darkColor);
	}

	void DrawType2 (int atX, int atY)
	{
		texture.SetPixel (atX + 1, atY, lightColor);
		texture.SetPixel (atX + 1, atY - 1, darkColor);
		//texture.SetPixel (atX, atY - 1, darkColor);
		texture.SetPixel (atX, atY - 2, darkColor);
	}

	void DrawType3 (int atX, int atY)
	{
		texture.SetPixel (atX + 2, atY, lightColor);
		texture.SetPixel (atX + 2, atY - 1, lightColor);
		texture.SetPixel (atX, atY - 1, lightColor);

		texture.SetPixel (atX + 1, atY - 2, darkColor);
		texture.SetPixel (atX + 2, atY - 2, darkColor);
	}

	void DrawType4 (int atX, int atY)
	{
		texture.SetPixel (atX, atY, lightColor);
		texture.SetPixel (atX + 3, atY - 1, lightColor);

		texture.SetPixel (atX + 1, atY - 1, darkColor);
		texture.SetPixel (atX + 1, atY - 2, darkColor);
		texture.SetPixel (atX + 2, atY - 2, darkColor);
	}
}
