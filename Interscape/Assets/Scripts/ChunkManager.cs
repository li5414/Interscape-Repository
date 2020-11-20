using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
	
	// important values
	public static int seed = 130;
	public int renderDist = 4;                 // no. chunks
	public static int mapDimension = 5000;     // no. tiles
	public static int chunkSize = 16;          // no. tiles

	// references / objects
	public Transform playerTrans;          // player reference
	public Tilemap tilemapObj;             // used as empty tilemap to instantiate
	public Tilemap sandTilemap;
	public Tilemap waterTilemap;

	// coordinate variables
	public Vector2 viewerPosition; //?? not sure why static
	int currentChunkCoordX;
	int currentChunkCoordY;
	int lastChunkCoordX;
	int lastChunkCoordY;

	// number generator
	public System.Random prng = new System.Random (seed);
	
	// chunk dictionary
	public Dictionary<Vector2, MyChunkClass> terrainChunkDictionary = new Dictionary<Vector2, MyChunkClass> ();
	List<MyChunkClass> terrainChunksVisibleLastUpdate = new List<MyChunkClass> ();

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	private void Start ()
	{
		UpdateVisibleChunks ();
	}

	void Update ()
	{
		// get viewer and chunk position
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoordX = (Mathf.RoundToInt (viewerPosition.x / chunkSize)) * chunkSize;
		currentChunkCoordY = (Mathf.RoundToInt (viewerPosition.y / chunkSize)) * chunkSize;

		// if player moved to a different chunk, update chunks
		if ((currentChunkCoordX != lastChunkCoordX) ||  (currentChunkCoordY != lastChunkCoordY)){
			UpdateVisibleChunks ();
			lastChunkCoordX = currentChunkCoordX;
			lastChunkCoordY = currentChunkCoordY;
		}
	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	void UpdateVisibleChunks ()
	{
		// clear all visible chunks
		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate[i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		// go through neighbouring chunks that need to be rendered
		for (int x = -(renderDist+1) * chunkSize; x <= renderDist * chunkSize; x += chunkSize) {
			for (int y = -renderDist * chunkSize; y <= renderDist * chunkSize; y += chunkSize) {
				Vector2Int viewedChunkCoord = new Vector2Int (currentChunkCoordX + x, currentChunkCoordY + y);

				// if chunk has been encountered before, just switch it to visible
				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					if (terrainChunkDictionary [viewedChunkCoord].IsVisible () == false) {
						terrainChunkDictionary [viewedChunkCoord].SetVisible (true);
					}
					terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
				}
				else {
					// add chunks coordinates to dictionary and generate new
					Vector3Int pos = new Vector3Int (currentChunkCoordX + x, currentChunkCoordY + y, 200);
					terrainChunkDictionary.Add (viewedChunkCoord, new MyChunkClass (prng, pos, tilemapObj, sandTilemap, waterTilemap));
					terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
				}

			}
		}
	}

	
}
