using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using System;

public class ChunkManager : MonoBehaviour {
    // references / objects
    public Transform playerTrans;          // player reference
    public Tilemap sandTilemap;

    // coordinate variables
    Vector2 viewerPosition;
    Vector2Int currentChunkCoord;
    Vector2Int lastChunkCoord;

    // chunk management dict, list and queues
    public Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    List<Chunk> chunksVisible = new List<Chunk>();
    Queue<Chunk> chunksToGenerate = new Queue<Chunk>();
    public Queue<Chunk> chunksToLoad = new Queue<Chunk>();
    Queue<Chunk> chunksToUnload = new Queue<Chunk>();

    // reference to the tile files
    public static TileResources tileResources;

    static WorldSettings worldSettings;

    private void Start() {
        worldSettings = GameObject.FindWithTag("SystemPlaceholder").GetComponent<WorldSettings>();
        tileResources = new TileResources();

        // initalise positions
        viewerPosition = new Vector2(playerTrans.position.x, playerTrans.position.y);
        currentChunkCoord = ToChunkCoord(viewerPosition);
        lastChunkCoord = ToChunkCoord(viewerPosition);

        UpdateVisibleChunks();
    }

    void Update() {
        // get viewer and chunk position
        viewerPosition = new Vector2(playerTrans.position.x, playerTrans.position.y);
        currentChunkCoord = ToChunkCoord(viewerPosition);

        // finishing generating chunks that need to be generated
        if (chunksToGenerate.Count > 0) {
            Chunk chunk = chunksToGenerate.Dequeue();
            chunk.GenerateChunkData();
        }

        // fininish loading (rendering) chunks that need to be loaded
        if (chunksToLoad.Count > 0) {
            if (chunksToLoad.Peek().IsGenerated()) {
                Chunk chunk = chunksToLoad.Dequeue();
                chunk.LoadChunk();
            }
        }

        // fininish unloading chunks
        if (chunksToUnload.Count > 0) {
            if (chunksToUnload.Peek().IsGenerated()) {
                Chunk chunk = chunksToUnload.Dequeue();
                chunk.UnloadChunk();
            }
        }

        // if player moved a chunks, update chunks
        if (currentChunkCoord.x != lastChunkCoord.x || currentChunkCoord.y != lastChunkCoord.y) {
            UpdateVisibleChunks();
            lastChunkCoord = new Vector2Int(currentChunkCoord.x, currentChunkCoord.y);
        }
    }

    /*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    public Vector2Int ToChunkCoord(Vector2 position) {
        Vector2Int chunkCoord = new Vector2Int();
        chunkCoord.x = Mathf.FloorToInt(viewerPosition.x / Consts.CHUNK_SIZE);
        chunkCoord.y = Mathf.FloorToInt(viewerPosition.y / Consts.CHUNK_SIZE);
        return chunkCoord;
    }

    void UpdateVisibleChunks() {
        //Debug.Log ("chunk coord" + currentChunkCoord.ToString());
        CalculateChunksToUnload();
        int renderDist = worldSettings.RENDER_DIST;

        // go through neighbouring chunks that need to be rendered
        for (int x = -renderDist; x <= renderDist; x++) {
            for (int y = -renderDist; y <= renderDist; y++) {
                Vector2Int chunkCoord = new Vector2Int(currentChunkCoord.x + x, currentChunkCoord.y + y);

                // if chunk has been encountered before, just switch it to visible
                if (chunkDictionary.ContainsKey(chunkCoord)) {
                    if (!chunkDictionary[chunkCoord].IsLoaded() &&
                        chunkDictionary[chunkCoord].IsGenerated()) {
                        chunksToLoad.Enqueue(chunkDictionary[chunkCoord]);
                    }
                } else {
                    // add chunks coordinates to dictionary and generate new chunk
                    chunkDictionary.Add(chunkCoord, new Chunk(chunkCoord));
                    chunksToGenerate.Enqueue(chunkDictionary[chunkCoord]);
                }
                chunksVisible.Add(chunkDictionary[chunkCoord]);
            }
        }

        /*Debug.Log (chunksVisible.Count + " chunks visible");
		Debug.Log ("Generating " + chunksToGenerate.Count + " chunks...");
		Debug.Log ("Loading " + chunksToLoad.Count + " chunks...");
		Debug.Log ("Unloading " + chunksToUnload.Count + " chunks...");
		Debug.Log ("--------------");*/
    }

    // enqueues chunks to be unloaded
    void CalculateChunksToUnload() {
        for (int i = 0; i < chunksVisible.Count; i++) {
            Chunk chunk = chunksVisible[i];
            if (!isWithinRenderDistance(chunk)) {
                chunksToUnload.Enqueue(chunk);
            }
        }
        chunksVisible.Clear();
    }

    bool isWithinRenderDistance(Chunk chunk) {
        if ((Mathf.Abs(currentChunkCoord.x - chunk.chunkCoord.x) > worldSettings.RENDER_DIST) ||
            (Mathf.Abs(currentChunkCoord.y - chunk.chunkCoord.y) > worldSettings.RENDER_DIST)) {
            return false;
        }
        return true;
    }
}
