using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetailGeneration : MonoBehaviour {
	public int width = 10;
	public int height = 10;

	// tile stuff
	public TileBase detail1;
	public TileBase detail2;
	public TileBase detail3;
	public TileBase detail4;
	public TileBase detail5;

	void Start ()
	{
		GenerateMap ();
	}

	void GenerateMap ()
	{
		Tilemap tilemap = GetComponent<Tilemap> ();
		int randNum;
		width = width * 5;
		height = height * 5;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				randNum = Random.Range(0, 15);
				Vector3Int detailPos = new Vector3Int (x - (width / 2), y - (width / 2), 199);

				// generate grass details
				if (randNum == 1)
					tilemap.SetTile (detailPos, detail1);
				else if (randNum == 2)
					tilemap.SetTile (detailPos, detail2);
				else if (randNum == 3)
					tilemap.SetTile (detailPos, detail4);
				else if (randNum == 4)
					tilemap.SetTile (detailPos, detail5);

				// flower detail generation
				else if (randNum == 5 & Random.value < 0.2)
					tilemap.SetTile (detailPos, detail3);
				else { }

			}
		}
	}
}
