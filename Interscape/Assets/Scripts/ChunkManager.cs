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
	List<Chunk> chunksVisible = new List<Chunk> ();

	/*List<Chunk> chunksLoading = new List<Chunk> ();*/

	 Queue<Chunk> chunksToGenerate = new Queue<Chunk> ();
	public Queue<Chunk> chunksToLoad = new Queue<Chunk> ();
	 Queue<Chunk> chunksToUnload = new Queue<Chunk> ();

	int distToUpdate = 1 * chunkSize;

	public static TileResources tileResources;
	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	private void Start ()
	{
		tileResources = new TileResources ();

		UpdateVisibleChunks ();

		// initalise positions to avoid duplicate chunk loading :(
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoord = ToChunkCoord (viewerPosition);
		lastChunkCoord = ToChunkCoord (viewerPosition);

	}

	void Update ()
	{
		// get viewer and chunk position
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoord = ToChunkCoord (viewerPosition);

		// if player moved a few chunks, update chunks
		if (Mathf.Abs(currentChunkCoord.x - lastChunkCoord.x) > distToUpdate ||
			Mathf.Abs(currentChunkCoord.y - lastChunkCoord.y) > distToUpdate) {
			UpdateVisibleChunks ();
			//Debug.Log ("Before: " + lastChunkCoord.ToString () + " After: " + currentChunkCoord.ToString ());
			lastChunkCoord.x = currentChunkCoord.x;
			lastChunkCoord.y = currentChunkCoord.y;
			
		}

		// finishing generating chunks that need to be generated
		if (chunksToGenerate.Count > 0) {
			if (chunksToGenerate.Peek().isReady()) {
				Chunk chunk;
				chunk = chunksToGenerate.Dequeue ();
				chunk.FinishGenerating ();
				chunksVisible.Add (chunk);
			}
		}

		// fininish loading (rendering) chunks that need to be loaded
		if (chunksToLoad.Count > 0) {
			Chunk chunk = chunksToLoad.Dequeue ();
			chunk.LoadChunk ();
			chunksVisible.Add (chunk);
		}

		// fininish unloading chunks
		if (chunksToUnload.Count > 0) {
			Chunk chunk = chunksToUnload.Dequeue ();
			chunk.UnloadChunk ();
			chunksVisible.Remove (chunk);
		}


	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	public Vector2Int ToChunkCoord(Vector2 position)
	{
		Vector2Int chunkCoord = new Vector2Int ();
		chunkCoord.x = Mathf.FloorToInt (viewerPosition.x / chunkSize) * chunkSize;
		chunkCoord.y = Mathf.FloorToInt (viewerPosition.y / chunkSize) * chunkSize;
		return chunkCoord;
	}

	void UpdateVisibleChunks ()
	{
		Debug.Log ("Generating " + chunksToGenerate.Count + " chunks...");
		Debug.Log ("Loading " + chunksToLoad.Count + " chunks...");
		Debug.Log ("Unloading " + chunksToUnload.Count + " chunks...");
		
		// unload unneeded chunks
		for (int i = 0; i < chunksVisible.Count; i++) {
			Chunk chunk = chunksVisible [i];

			if (!isWithinRenderDistance(chunk) && chunk.IsLoaded()) {
				chunksToUnload.Enqueue (chunk);
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
						chunksToLoad.Enqueue (chunkDictionary [chunkCoord]);
					}
				}
				else {
					// add chunks coordinates to dictionary and generate new chunk
					chunkDictionary.Add (chunkCoord, new Chunk (prng, chunkCoord));
					chunksToGenerate.Enqueue (chunkDictionary [chunkCoord]);
				}
			}
		}
		Debug.Log ("Generating " + chunksToGenerate.Count + " chunks...");
		Debug.Log ("Loading " + chunksToLoad.Count + " chunks...");
		Debug.Log ("Unloading " + chunksToUnload.Count + " chunks...");
		Debug.Log ("--------------");
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
			yield return null;
		}
		//chunk.createThread ();
		chunk.GenerateChunkData ();
	}
}
