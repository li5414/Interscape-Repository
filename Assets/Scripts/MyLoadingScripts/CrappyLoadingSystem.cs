using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrappyLoadingSystem : MonoBehaviour {
	// public variables
	public int mapWidth = 200;         // measured in no. of tiles
	public int mapHeight = 200;        // measured in no. of tiles
	public int radius = 20;
	private int multiplyVar = 5;
	public GameObject treeParent;
	public GameObject shrubParent;

	[Space (10)]
	public GameObject player;
	public GameObject BaseTilemap;
	public GameObject DetailTilemap;
	Tilemap baseTilemap;
	Tilemap detailTilemap;

	[Space (10)]
	public TileBase tile1;
	public TileBase tile2;

	// tile stuff
	[Space (10)]
	public TileBase detail1;
	public TileBase detail2;
	public TileBase detail3;
	public TileBase detail4;
	public TileBase detail5;

	// perlin noise stuff
	private float colourThreshold = 2.3f;
	private float treeThreshold1 = 0.03f;
	private float treeThreshold2 = 0.001f; // very small value for spawning those big ass trees
	private float refinement = 0.1f;
	private float multiplier = 5f;
	private float perlinNoise = 0f;

	// tree objects
	[Space (10)]
	public GameObject treeFab1;
	public GameObject treeFab2;
	public GameObject treeFab3;
	public GameObject treeFab4;

	// arrays
	public int [,] tiles { get; set; }
	public int [,] details;
	public GameObject [,] trees;

	private Vector2Int lastUpdatePos;

	private void Awake ()
	{
		// create arrays
		tiles = new int [mapWidth, mapHeight];
		details = new int [mapWidth * multiplyVar, mapHeight * multiplyVar]; // the detail tilemap is smaller, there are more tiles per unit
		trees = new GameObject [mapWidth, mapHeight];

		// get gameobjects
		baseTilemap = BaseTilemap.GetComponent<Tilemap> ();
		detailTilemap = DetailTilemap.GetComponent<Tilemap> ();
	}

	private void Start ()
	{
		SetBaseTiles ();
		SetDetailTiles ();
		SetTrees ();

		UpdateMap ();
	}

	private void Update ()
	{
		int xPos = (int)player.transform.position.x;
		int yPos = (int)player.transform.position.y;

		if (Mathf.Abs(xPos - lastUpdatePos.x) > 20 ||
			Mathf.Abs (yPos - lastUpdatePos.y) > 20)
		{
			UpdateMap ();
		}
	}

	void UpdateMap ()
	{
		UpdateArea ();
		UpdateDetails ();
	}

	// removes and adds tiles and trees in one loop
	void UpdateArea ()
	{
		Vector3Int pos;
		int xPos = (int)player.transform.position.x;
		int yPos = (int)player.transform.position.y;
		lastUpdatePos = new Vector2Int(xPos, yPos);

		for (int i = xPos - (int)(radius * 2); i < xPos + (int)(radius * 2); i++) {
			for (int j = yPos - (int)(radius * 2); j < yPos + (int)(radius * 2); j++) {

				// check within map bounds
				if ((i < mapWidth && i >= 0) && (j < mapHeight && j >= 0)) {
					pos = new Vector3Int (i, j, 1000);

					// DELETING OLD
					if (!((i >= xPos - radius && i < xPos + radius) &&
							(j >= yPos - radius && j < yPos + radius))) {

						// set tile to null if not already
						if (baseTilemap.GetTile (pos) != null) {
							baseTilemap.SetTile (pos, null);
						}

						// potentially unload tree
						if (trees [i, j] != null && trees [i, j].activeSelf == true) {
							trees [i, j].SetActive (false);
						}
					}

					// LOADING NEW
					else {
						// only set tile if it is null
						if (baseTilemap.GetTile (pos) == null) {
							if (tiles [i, j] == 1)
								baseTilemap.SetTile (pos, tile1);
							else
								baseTilemap.SetTile (pos, tile2);
						}

						// only set tree if it has not been set yet but there should be one
						if (trees [i, j] != null && trees [i, j].activeSelf == false) {
							trees [i, j].SetActive (true);
						}
					}
						
				}
			}
		}

	}

	private void SetTrees () {
		float diff;
		float randNum;

		// indicate trees in array (more likely to generate when noise value is low)
		for (int x = 1; x < mapWidth-1; x++) {
			for (int y = 1; y < mapHeight-1; y++) {

				// generate noise values
				perlinNoise = Mathf.PerlinNoise (x * refinement, y * refinement);
				diff = perlinNoise * multiplier;

				// check nearby coordinates for trees
				if (diff > colourThreshold) {
					if (((trees [x - 1, y] == null && trees [x + 1, y] == null) &&
					(trees [x, y - 1] == null && trees [x, y + 1] == null)) &&
					((trees [x - 1, y - 1] == null && trees [x + 1, y + 1] == null) &&
					(trees [x - 1, y + 1] == null && trees [x + 1, y - 1] == null))) {

						Vector3Int pos = new Vector3Int (x, y, 198);
						randNum = Random.value;

						if (Random.value < treeThreshold1) {
							if (randNum < 0.34 && randNum >= treeThreshold2) {
								trees [x, y] = Instantiate (treeFab1, pos, Quaternion.identity);
							}
							else if (randNum > 0.66) {
								trees [x, y] = Instantiate (treeFab2, pos, Quaternion.identity);
							}
							else if (randNum < treeThreshold2) {
								trees [x, y] = Instantiate (treeFab4, pos, Quaternion.identity);
							}
							else {
								trees [x, y] = Instantiate (treeFab3, pos, Quaternion.identity);
							}
							trees [x, y].SetActive (false);
							trees [x, y].transform.SetParent (treeParent.transform, true);
						}
					}
				}
			}
		}
	}

	private void SetBaseTiles ()
	{
		// randomly set base tiles to be either 1 or 2
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				if (Random.value < 0.5) {
					tiles [x, y] = 1;
				} else {
					tiles [x, y] = 2;
				}

			}
		}
		Debug.Log ("base tiles set");
	}

	private void SetDetailTiles ()
	{
		int randNum;
		int width = mapWidth * multiplyVar;
		int height = mapHeight * multiplyVar;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				randNum = Random.Range (0, 15);

				// generate grass details
				if (randNum == 1)
					details [x, y] = 1;
				else if (randNum == 2)
					details [x, y] = 2;
				else if (randNum == 3)
					details [x, y] = 3;
				else if (randNum == 4)
					details [x, y] = 4;

				// flower detail generation
				else if (randNum == 5 & Random.value < 0.2)
					details [x, y] = 5;
				else {
					details [x, y] = 0;
				}
			}
		}
	}

	void UpdateDetails ()
	{
		int xPos = (int)player.transform.position.x * multiplyVar;
		int yPos = (int)player.transform.position.y * multiplyVar;
		int newRad = 50 * multiplyVar;
		Vector3Int pos;

		// screen size testing
		for (int i = xPos - newRad * 2; i < xPos + newRad * 2; i++) {
			for (int j = yPos - newRad * 2; j < yPos + newRad * 2; j++) {

				// check within map bounds
				if ((i < mapWidth * multiplyVar && i >= 0) &&
					(j < mapHeight * multiplyVar && j >= 0)) {
					pos = new Vector3Int (i, j, 199);

					// check within screen
					if ((i >= xPos - newRad && i < xPos + newRad) &&
						(j >= yPos - newRad && j < yPos + newRad)) {

						// only update tile if it is null
						if (detailTilemap.GetTile (pos) == null) {

							// set tile according to value
							if (details [i, j] == 0)
								detailTilemap.SetTile (pos, null);
							else if (details [i, j] == 1)
								detailTilemap.SetTile (pos, detail1);
							else if (details [i, j] == 2)
								detailTilemap.SetTile (pos, detail2);
							else if (details [i, j] == 3)
								detailTilemap.SetTile (pos, detail3);
							else if (details [i, j] == 4)
								detailTilemap.SetTile (pos, detail4);

							else if (details [i, j] == 5)
								detailTilemap.SetTile (pos, detail5);
							else
								detailTilemap.SetTile (pos, null);
						}
					}

					// if not within radius, remove
					else {
						// only update tile if it isnt alreay null
						if (detailTilemap.GetTile (pos) != null) {
							detailTilemap.SetTile (pos, null);
						}
					}
				}
			}
		}
	}
}

