using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using System;

public class ChunkManager : MonoBehaviour
{
	
	// important values
	public static int seed = 130;
	public int renderDist = 5;                 // no. chunks
	public static int mapDimension = 5000;     // no. tiles
	public static int chunkSize = 16;          // no. tiles

	// references / objects
	public Transform playerTrans;          // player reference
	public Tilemap tilemapObj;             // used as empty tilemap to instantiate
	public Tilemap grassTilemap;
	public Tilemap sandTilemap;
	public Tilemap waterTilemap;

	// coordinate variables
	public Vector2 viewerPosition;
	Vector2Int currentChunkCoord;
	Vector2Int lastChunkCoord;

	// number generator
	public System.Random prng = new System.Random (seed);
	
	// chunk dictionary
	public Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk> ();
	List<Chunk> chunksVisibleLastUpdate = new List<Chunk> ();

	List<Chunk> chunksLoading = new List<Chunk> ();

	int distToUpdate = 1 * chunkSize;

	TileResources tileResources;
	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	private void Start ()
	{
		tileResources = new TileResources ();

		UpdateVisibleChunks ();
	}

	void Update ()
	{
		// get viewer and chunk position
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoord.x = Mathf.RoundToInt (viewerPosition.x / chunkSize) * chunkSize;
		currentChunkCoord.y = Mathf.RoundToInt (viewerPosition.y / chunkSize) * chunkSize;

		// if player moved a few chunks, update chunks
		if (Mathf.Abs(currentChunkCoord.x - lastChunkCoord.x) > distToUpdate ||
			Mathf.Abs(currentChunkCoord.y - lastChunkCoord.y) > distToUpdate) {
			UpdateVisibleChunks ();
			lastChunkCoord.x = currentChunkCoord.x;
			lastChunkCoord.y = currentChunkCoord.y;
		}

		//Debug.Log ("pepee");
		List<Chunk> readyChunks = new List<Chunk> (); 
		foreach (Chunk chunk in chunksLoading) {
			if (chunk.isReady()) {
				readyChunks.Add (chunk);
			}
		}
		//Debug.Log ("Size " + readyChunks.Count);
		foreach (Chunk chunk in readyChunks) {
			chunk.finishLoading ();
			chunksLoading.Remove (chunk);
		}
		

	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	void UpdateVisibleChunks ()
	{
		// unload unneeded chunks
		for (int i = 0; i < chunksVisibleLastUpdate.Count; i++) {
			Chunk chunk = chunksVisibleLastUpdate [i];

			if (!isWithinRenderDistance(chunk) && chunk.IsLoaded()) {
				chunk.UnloadChunk();
				chunksVisibleLastUpdate.RemoveAt (i);
			}
		}

		int xRadius = renderDist * chunkSize;
		int yRadius = xRadius - chunkSize;

		// go through neighbouring chunks that need to be rendered
		for (int x = -xRadius; x < xRadius; x += chunkSize) {
			for (int y = -yRadius; y < yRadius; y += chunkSize) {
				Vector2Int chunkCoord = new Vector2Int (currentChunkCoord.x + x, currentChunkCoord.y + y);

				// if chunk has been encountered before, just switch it to visible
				if (chunkDictionary.ContainsKey (chunkCoord)) {
					if (!chunkDictionary [chunkCoord].IsLoaded ()) {
						chunkDictionary [chunkCoord].LoadChunk();
						chunksVisibleLastUpdate.Add (chunkDictionary [chunkCoord]);
					}
				}
				else {
					// add chunks coordinates to dictionary and generate new
					chunkDictionary.Add (chunkCoord, new Chunk (prng, chunkCoord, tileResources));
					chunksLoading.Add (chunkDictionary [chunkCoord]);
					chunksVisibleLastUpdate.Add (chunkDictionary [chunkCoord]);
				}

			}
		}
	}

	bool isWithinRenderDistance(Chunk chunk)
	{
		if ((Mathf.Abs (currentChunkCoord.x - chunk.chunkPos.x) < renderDist*chunkSize) &&
			(Mathf.Abs (currentChunkCoord.y - chunk.chunkPos.y) < renderDist*chunkSize)) {
			return true;
		}
		return false;
	}

	public void allocate(Chunk chunk)
	{
		StartCoroutine (allocateMemory(chunk));
	}

	IEnumerator allocateMemory (Chunk chunk)
	{
		for (int i = 0; i < chunk.tileArray.Length; i++) {
			chunk.tileArray [i] = ScriptableObject.CreateInstance<Tile> ();
			chunk.waterTileArray [i] = ScriptableObject.CreateInstance<Tile> ();
			//chunk.sandTileArray [i] = ScriptableObject.CreateInstance<RuleTile> ();
			yield return null;
		}
		chunk.createThread ();
		//chunk.GenerateChunkData ();
	}
}
