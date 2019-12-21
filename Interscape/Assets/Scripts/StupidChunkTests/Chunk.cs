using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	// this script was made following a tutorial online with some modifications that don't work

	public static int size = 10;
	Tile1 [,] tiles;
	int [,] tileData;
	GameObject [,] treeData;

	public GameObject data;

	public Sprite grass1;
	public Sprite grass2;

	public GameObject treeFab1;
	public GameObject treeFab2;
	public GameObject treeFab3;
	public GameObject treeFab4;

	private void Start ()
	{
		
		CrappyLoadingSystem cScript = data.GetComponent<CrappyLoadingSystem> ();
		tileData = new int [cScript.mapWidth, cScript.mapHeight];
		tileData = cScript.tiles;

		// create tile array for the chunk
		tiles = new Tile1 [size, size];
		Debug.Log (tileData == null);

		// try to access array created in other script :(
		treeData = data.GetComponent<CrappyLoadingSystem> ().trees;
		int xPos = (int)transform.position.x;
		int yPos = (int)transform.position.y;


		for (int i = 0; i < size; i++) {
			for (int j= 0; j < size; j++) {

				// each tile position offset by chunk position
				tiles [i, j] = new Tile1 (i + xPos, j + yPos, 0);

				// create the tile gameobject with position inside name
				GameObject tileGO = new GameObject ("Tile_" +
					tiles[i, j].x + "_" + tiles[i, j].y);

				// set its position and parent
				tileGO.transform.position = new Vector3 (tiles
					[i, j].x, tiles [i, j].y, tiles [i, j].z);
				tileGO.transform.SetParent (this.transform, true);

				// set its sprite
				SpriteRenderer spriteRenderer = tileGO.AddComponent<SpriteRenderer> ();
				if (tileData[(tiles [i, j].x), (tiles [i, j].y)] == 1) {
					spriteRenderer.sprite = grass1;
				}
				else {
					spriteRenderer.sprite = grass2;
				}

				// instantiate trees
				if (treeData [(tiles [i, j].x), (tiles [i, j].y)] != null) {
					GameObject tree = Instantiate (treeData [(tiles [i, j].x), (tiles [i, j].y)]);
					tree.SetActive (true);
				}
				
			}
		}
	}
}
