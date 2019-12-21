using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour {
	public static int width = 150, height = 150;
	public GameObject treeParent;
	public GameObject shrubParent;
	private float randNum;
	SpriteRenderer sprender;

	[Space (10)]
	public TileBase tile1;
	public TileBase tile2;
	

	// perlin noise stuff
	[Space(10)]
	public float colourThreshold = 2.3f;
	public float treeThreshold1 = 0.03f;
	public float treeThreshold2 = 0.001f; // very small value for spawning trees on light tiles
	public float refinement = 0.1f;
	public float multiplier = 5f;
	private float perlinNoise = 0f;

	// tree objects
	[Space (10)]
	public GameObject treeFab1;
	public GameObject treeFab2;
	public GameObject treeFab3;
	public GameObject treeFab4;
	private bool[,] isTreeArray = new bool[width+2,height+2];

	// shrub objects
	[Space (10)]
	public GameObject ShrubFab3;
	public GameObject ShrubFab5;
	public GameObject ShrubFab7;
	public GameObject ShrubFab8;
	public GameObject ShrubFab9;
	public GameObject ShrubFab10;

	void Start ()
	{
		GenerateMap ();
	}

	void GenerateMap ()
	{
		Tilemap tilemap = GetComponent<Tilemap> ();
		float diff;
		GameObject tree;
		GameObject shrub;

		// generating base tiles
		for (int x = 1; x < width; x++) {
			for (int y = 1; y < height; y++) {

				isTreeArray [x, y] = false;

				// generates colour shift using noise
				perlinNoise = Mathf.PerlinNoise(x * refinement, y * refinement);
				diff = perlinNoise * multiplier;

				// position vector uses values from loops
				Vector3Int pos = new Vector3Int (x - (width / 2), y - (width / 2), 200);

				// tile stuff
				randNum = Random.value;
				if (randNum < 0.5)
					tilemap.SetTile (pos, tile1);
				else
					tilemap.SetTile (pos, tile2);

				// sets the new colour (brightness) onto tile
				//sprender = tile.GetComponent<SpriteRenderer>();
				//sprender.color = newColour;
				
				// generate trees (more likely to generate trees on "dark patches")
				// check nearby coordinates for trees first
				if (((isTreeArray[x - 1, y] == false && isTreeArray [x + 1, y] == false) &&
					(isTreeArray [x, y - 1] == false && isTreeArray [x, y + 1] == false)) &&
					((isTreeArray [x - 1, y - 1] == false && isTreeArray [x + 1, y + 1] == false) &&
					(isTreeArray [x - 1, y + 1] == false && isTreeArray [x + 1, y - 1] == false))) {

					// regular tree generation
					if (diff > colourThreshold) {
						if (randNum < treeThreshold1) {
							if (Random.value < 0.33)
								tree = Instantiate (treeFab1, pos, Quaternion.identity);
							else if (Random.value > 0.66)
								tree = Instantiate (treeFab2, pos, Quaternion.identity);
							else
								tree = Instantiate (treeFab3, pos, Quaternion.identity);
							tree.transform.SetParent (treeParent.transform, true);

							// chance to flip the tree
							if (Random.value < 0.5) {
								sprender = tree.GetComponent<SpriteRenderer> ();
								sprender.flipX = true;
							}
							isTreeArray [x, y] = true;
						}
					}

					// big tree generation
					else {
						if (randNum < treeThreshold2) {
							tree = Instantiate (treeFab4, pos, Quaternion.identity);
							tree.transform.SetParent (treeParent.transform, true);

							// chance to flip the tree
							if (Random.value < 0.5) {
								sprender = tree.GetComponent<SpriteRenderer> ();
								sprender.flipX = true;
							}
							isTreeArray [x, y] = true;
						}
					}
				}
				

				// generate the shrubs
				randNum = Random.Range (0, 60);
				if (isTreeArray [x, y] == false) {
					if (randNum == 0) {
						shrub = Instantiate (ShrubFab3, pos, Quaternion.identity);
						shrub.transform.SetParent (shrubParent.transform, true);
					}
					else if (randNum == 1) {
						shrub = Instantiate (ShrubFab5, pos, Quaternion.identity);
						shrub.transform.SetParent (shrubParent.transform, true);
					}


					// golden grass
					else if (diff < 0.5) {
						if (randNum < 30) {
							shrub = Instantiate (ShrubFab7, pos, Quaternion.identity);
							shrub.transform.SetParent (shrubParent.transform, true);
						}
						else {
							shrub = Instantiate (ShrubFab8, pos, Quaternion.identity);
							shrub.transform.SetParent (shrubParent.transform, true);
						}
					}

					// green grass
					else if (diff > 4.3) {
						if (randNum < 30) {
							shrub = Instantiate (ShrubFab9, pos, Quaternion.identity);
							shrub.transform.SetParent (shrubParent.transform, true);
						}
						else {
							shrub = Instantiate (ShrubFab10, pos, Quaternion.identity);
							shrub.transform.SetParent (shrubParent.transform, true);
						}
					}
				}
			}
		}
	}
}
