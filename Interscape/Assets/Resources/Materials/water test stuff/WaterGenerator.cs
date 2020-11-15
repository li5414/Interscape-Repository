using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WaterGenerator : MonoBehaviour
{
	private int imageSize = 128;
	public Texture2D noise1;
	public Texture2D noise2;
	public float tick = 0.02f;
	public float timeUntilNextTick = 0.02f;
	private Texture2D texture;
	private Sprite sprite;

	public int offset1 = 1; //pixels
	public int offset2 = -1; //pixels
	public int currentIterations = 0;
	int nsaves = 0;

	[Space]
	public SpriteRenderer spr1;
	public SpriteRenderer spr2;
	public SpriteRenderer spr3;
	public SpriteRenderer spr4;
	public SpriteRenderer spr5;
	public SpriteRenderer spr6;
	public SpriteRenderer spr7;
	public SpriteRenderer spr8;

	// Start is called before the first frame update
	void Start()
    {
		texture = new Texture2D (imageSize, imageSize);
		texture.filterMode = FilterMode.Point;
		sprite = Sprite.Create (texture, new Rect (0, 0, imageSize, imageSize), Vector2.zero);
		GetComponent<SpriteRenderer> ().sprite = sprite;

		//for (int i = 0; i < 128; i++) {
			resetPixels ();
		//	SaveFile (i);
		//}
		

		
	}

	public Color Posturize(Color inputColour)
	{
		// posturize by even distance
		/*float levels = 8f;
		float value = ((int)(inputColour.r * levels))/levels;
		value = Mathf.Lerp (0.1f, 1, value);
		return new Color (value, value, value, 1);*/

		// lerp input colour
		float col = Mathf.Lerp (0f, 1, inputColour.r);


		float returnVal;
		if (col > 0.67)
			return new Color (0.9f, 0.95f, 1, 0.5f);
		//else if (col > 0.6f)
		//	returnVal = 0f;
		//else if (col > 0.55)
		//	returnVal = 0.4f - adjustment;
		else if (col > 0.6)
			returnVal = 0f;
		else if (col > 0.5)
			returnVal = 0.04f;
		else if (col > 0.47)
			returnVal = 0.10f;
		else if (col > 0.4f)
			returnVal = 0.01f;
		else
			returnVal = 0f;


		return new Color (0.75f, 0.9f, 1, returnVal);

	}

	public Color GetAverage(int x, int y, Texture2D tex)
	{
		float sum = tex.GetPixel (x, y).r;

		// left side case
		int left;
		if (x > 0)
			left = x-1;
		else
			left = tex.width;
		sum += tex.GetPixel (left, y).r;

		// right side case
		int right;
		if (x < tex.width)
			right = x + 1;
		else
			right = 0;
		sum += tex.GetPixel (right, y).r;

		// up case
		int up;
		if (y < tex.height)
			up = y + 1;
		else
			up = 0;
		sum += tex.GetPixel (x, up).r;

		// down case
		int down;
		if (y > 0)
			down = y - 1;
		else
			down = tex.height;
		sum += tex.GetPixel (x, down).r;

		sum += tex.GetPixel (left, up).r;
		sum += tex.GetPixel (left, down).r;
		sum += tex.GetPixel (right, up).r;
		sum += tex.GetPixel (right, down).r;

		sum = sum / 9;
		return new Color (sum, sum, sum, 1);
	}

	// Update is called once per frame
	void Update()
    {
        //if (timeUntilNextTick > 0) {
		//	timeUntilNextTick -= Time.deltaTime;
		//} else {
			
			resetPixels ();
		//	timeUntilNextTick = tick;
		//}
    }

	public void resetPixels()
	{

		// Goes through each pixel
		for (int y = 0; y < texture.height; y++) {

			// Choose start of sample x coordinate
			int sampleX1 = offset1 * currentIterations;
			int sampleX2 = offset2 * currentIterations; //assuming this is negative
			if (sampleX2 < 0) {
				sampleX2 = texture.height + sampleX2;
			}
			Color [] colors = new Color [texture.width];
			
			for (int x = 0; x < texture.width; x++) {

				// clamp x values to loop around
				if (sampleX1 >= texture.height) {
					sampleX1 = 0;
				}
				if (sampleX2 >= texture.height) {
					sampleX2 = 0;
				}



				// pick colours
				Color pixelColour;
				//pixelColour = GetAverage (sampleX1, y, noise1) + GetAverage (sampleX2, y, noise1);
				pixelColour = noise1.GetPixel (sampleX1, y) + noise2.GetPixel (sampleX2, y);
				pixelColour = pixelColour / 2f;
				pixelColour = Posturize (pixelColour);

				//texture.SetPixel (x, y, pixelColour);
				colors [x] = pixelColour;
				sampleX1++;
				sampleX2++;
			}
			texture.SetPixels (0, y, texture.width, 1, colors);
		}
		currentIterations++;

		if (currentIterations > texture.width / offset1)
			currentIterations = 0;

		texture.Apply ();

		spr1.sprite = sprite;
		spr2.sprite = sprite;
		spr3.sprite = sprite;
		spr4.sprite = sprite;
		spr5.sprite = sprite;
		spr6.sprite = sprite;
		spr7.sprite = sprite;
		spr8.sprite = sprite;
	}

	public void SaveFile(int n)
	{
		byte [] bytes = texture.EncodeToPNG ();
		var dirPath = Application.dataPath + "/WaterImageTest/";
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		File.WriteAllBytes (dirPath + "Image" + n + ".png", bytes);
	}
}
