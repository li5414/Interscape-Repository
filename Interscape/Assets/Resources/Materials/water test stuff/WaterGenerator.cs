using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterGenerator : MonoBehaviour
{
	private int imageSize = 128;
	public Texture2D noise1;
	public Texture2D noise2;
	public float [] colArray = new float [] {0.2f,  0.3f, 0.425f, 0.75f, 0.95f};

	// Start is called before the first frame update
	void Start()
    {
		Texture2D texture = new Texture2D (imageSize, imageSize);
		texture.filterMode = FilterMode.Point;
		Sprite sprite = Sprite.Create (texture, new Rect (0, 0, imageSize, imageSize), Vector2.zero);
		GetComponent<SpriteRenderer> ().sprite = sprite;

		//Goes through each pixel
		for (int y = 0; y < texture.height; y++) {
			for (int x = 0; x < texture.width; x++) 
			{
				Color pixelColour;
				/*if (Random.Range (0, 2) == 1)
				 {
					pixelColour = new Color (0, 0, 0, 1);
				} else {
					pixelColour = new Color (1, 1, 1, 1);
				}*/

				pixelColour = GetAverage (x, y, noise1) + GetAverage (x, y, noise1);
				//pixelColour = noise1.GetPixel (x, y) + noise2.GetPixel (x, y);
				pixelColour = pixelColour / 2f;
				//pixelColour = RemoveBright (pixelColour);
				pixelColour = Posturize (pixelColour);

				texture.SetPixel (x, y, pixelColour);
			}
		}
		texture.Apply ();
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

		// posturize by finsding closest colour
		/*float returnVal = colArray [0];
		float curMin = 2;
		for (int i = 0; i < colArray.Length; i++) {
			if (Mathf.Abs(colArray[i] - col) < curMin) {
				curMin = Mathf.Abs (colArray [i] - col);
				returnVal = colArray [i];
			}
		}
		*/
		float returnVal;
		if (col > 0.73)
			returnVal = 0.95f;
		else if (col > 0.6)
			returnVal = 0.2f;
		else if (col > 0.55)
			returnVal = 0.9f;
		else if (col > 0.5)
			returnVal = 0.8f;
		else if (col > 0.4)
			returnVal = 0.6f;
		else if (col > 0.3)
			returnVal = 0.4f;
		else
			returnVal = 0.2f;


		return new Color (returnVal, returnVal, returnVal, 1);
	}

	public Color RemoveBright (Color inputColour)
	{
		float threshold = 0.7f;

		if (inputColour.r < threshold) {
			return inputColour;
		} else {
			return Color.black;
		}

		
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
        
    }
}
