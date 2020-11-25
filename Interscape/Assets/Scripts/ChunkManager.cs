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


	Queue<Chunk> chunksToGenerate = new Queue<Chunk> ();
	public Queue<Chunk> chunksToLoad = new Queue<Chunk> ();
	Queue<Chunk> chunksToUnload = new Queue<Chunk> ();


	public static TileResources tileResources;
	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	private void Start ()
	{
		tileResources = new TileResources ();

		// initalise positions to avoid duplicate chunk loading :(
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoord = ToChunkCoord (viewerPosition);
		lastChunkCoord = ToChunkCoord (viewerPosition);

		UpdateVisibleChunks ();
	}

	void Update ()
	{
		// get viewer and chunk position
		viewerPosition = new Vector2 (playerTrans.position.x, playerTrans.position.y);
		currentChunkCoord = ToChunkCoord (viewerPosition);

		// finishing generating chunks that need to be generated
		if (chunksToGenerate.Count > 0) {
			if (chunksToGenerate.Peek().IsReadyToFinishGeneration()) {
				Chunk chunk = chunksToGenerate.Dequeue ();
				chunk.FinishGenerating ();
			}
		}

		// fininish loading (rendering) chunks that need to be loaded
		if (chunksToLoad.Count > 0) {
			if (chunksToLoad.Peek ().IsGenerated ()) {
				Chunk chunk = chunksToLoad.Dequeue ();
				chunk.LoadChunk ();
			}
		}

		// fininish unloading chunks
		if (chunksToUnload.Count > 0) {
			Chunk chunk = chunksToUnload.Dequeue ();
			chunk.UnloadChunk ();
		}

		// if player moved a chunks, update chunks
		if (currentChunkCoord.x != lastChunkCoord.x || currentChunkCoord.y != lastChunkCoord.y) {
			lastChunkCoord.x = currentChunkCoord.x;
			lastChunkCoord.y = currentChunkCoord.y;
			UpdateVisibleChunks ();
		}

	}

	/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	public Vector2Int ToChunkCoord(Vector2 position)
	{
		Vector2Int chunkCoord = new Vector2Int ();
		chunkCoord.x = Mathf.FloorToInt (viewerPosition.x / chunkSize);
		chunkCoord.y = Mathf.FloorToInt (viewerPosition.y / chunkSize);
		return chunkCoord;
	}

	void UpdateVisibleChunks ()
	{
		//Debug.Log ("chunk coord" + currentChunkCoord.ToString());
		CalculateChunksToUnload ();

		// go through neighbouring chunks that need to be rendered
		for (int x = -renderDist; x <= renderDist; x ++) {
			for (int y = -renderDist; y <= renderDist; y ++) {
				Vector2Int chunkCoord = new Vector2Int (currentChunkCoord.x + x, currentChunkCoord.y + y);

				// if chunk has been encountered before, just switch it to visible
				if (chunkDictionary.ContainsKey (chunkCoord)) {
					if (!chunkDictionary [chunkCoord].IsLoaded () &&
						chunkDictionary [chunkCoord].IsGenerated()) {
						chunksToLoad.Enqueue (chunkDictionary [chunkCoord]);
					}
				}
				else {
					// add chunks coordinates to dictionary and generate new chunk
					chunkDictionary.Add (chunkCoord, new Chunk (prng, chunkCoord));
					chunksToGenerate.Enqueue (chunkDictionary [chunkCoord]);
				}
				chunksVisible.Add (chunkDictionary [chunkCoord]);
			}
		}

		/*Debug.Log (chunksVisible.Count + " chunks visible");
		Debug.Log ("Generating " + chunksToGenerate.Count + " chunks...");
		Debug.Log ("Loading " + chunksToLoad.Count + " chunks...");
		Debug.Log ("Unloading " + chunksToUnload.Count + " chunks...");
		Debug.Log ("--------------");*/
	}

	// enqueues chunks to be unloaded
	void CalculateChunksToUnload ()
	{
		for (int i = 0; i < chunksVisible.Count; i++) {
			Chunk chunk = chunksVisible [i];

			if (!isWithinRenderDistance (chunk)) {
				chunksToUnload.Enqueue (chunk);
			}
		}

		chunksVisible.Clear();
	}

	bool isWithinRenderDistance(Chunk chunk)
	{
		if ((Mathf.Abs (currentChunkCoord.x - chunk.chunkCoord.x) > renderDist) ||
			(Mathf.Abs (currentChunkCoord.y - chunk.chunkCoord.y) > renderDist)) {
			return false;
		}
		return true;
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
