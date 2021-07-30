using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
	public int width, height;
	public float refinement = 0f;
	public float multiplier = 0f;
	public float colourThreshold = 3f;
	public float treeThreshold1 = 0.03f; 
	public float treeThreshold2 = 0.05f; // very small value for spawning trees on light tiles
	private float randNum;
	private float perlinNoise = 0f;

	// gameobjects
	public GameObject treeFab1;
	public GameObject treeFab2;
	public GameObject detailFab1;
	public GameObject detailFab2;
	public GameObject detailFab3;
	public GameObject detailFab4;
	public GameObject detailFab5;

	// tile stuff
	SpriteRenderer sprender;
	public GameObject tile1;
	public GameObject tile2;

	void Start() {
		GenerateMap();
	}
	
	void GenerateMap() {
		Tilemap tilemap = GetComponent<Tilemap> ();
		GameObject tree;
		GameObject tile;
		Color newColour;
		float diff;

		// generating base tiles
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				// generates colour shift using noise
				perlinNoise = Mathf.PerlinNoise(x * refinement, y * refinement);
				diff = perlinNoise * multiplier;
				newColour = new Color(1-diff, 1-diff, 1-diff);

				// position vector uses values from loops
				Vector3Int pos = new Vector3Int(x-(width/2), y-(width/2), 200);

				// tile stuff
				randNum = Random.value;
				if (randNum < 0.5)
					tile = Instantiate (tile1, pos, Quaternion.identity);
				else
					tile = Instantiate (tile2, pos, Quaternion.identity);

				// sets the new colour (brightness) onto tile
				//sprender = tile.GetComponent<SpriteRenderer>();
				//sprender.color = newColour;

				// generate grass details
				Vector3 detailPos = new Vector3(x - Random.value - (width / 2), y - Random.value - (width / 2), 199);
				Instantiate(detailFab1, detailPos, Quaternion.identity);
				detailPos = new Vector3(x - Random.value - (width / 2), y - Random.value - (width / 2), 199);
				Instantiate(detailFab2, detailPos, Quaternion.identity);
				detailPos = new Vector3(x - Random.value - (width / 2), y - Random.value - (width / 2), 199);
				Instantiate(detailFab4, detailPos, Quaternion.identity);
				detailPos = new Vector3(x - Random.value - (width / 2), y - Random.value - (width / 2), 199);
				Instantiate(detailFab5, detailPos, Quaternion.identity);

				// flower detail generation
				if (Random.value < 0.5) {
					detailPos = new Vector3(x - Random.value - (width / 2), y - Random.value - (width / 2), 199);
					Instantiate(detailFab3, detailPos, Quaternion.identity);
					if (Random.value < 0.1) {
						detailPos = new Vector3(x - Random.value - (width / 2), y - Random.value - (width / 2), 199);
						Instantiate(detailFab3, detailPos, Quaternion.identity);
					}
				}

				// generate trees (more likely to generate trees on dark patches)
				if (diff > colourThreshold) {
					if (randNum > treeThreshold1) {
						tree = Instantiate (treeFab1, pos, Quaternion.identity);
						if (Random.value < 0.5) {
							sprender = tree.GetComponent<SpriteRenderer> ();
							sprender.flipX = true;
						}
					}
				} else {
					if (randNum < treeThreshold2) {
						tree = Instantiate (treeFab2, pos, Quaternion.identity);
						if (Random.value < 0.5) {
							sprender = tree.GetComponent<SpriteRenderer> ();
							sprender.flipX = true;
						}
					}
				}
			}
		}
    }
}
