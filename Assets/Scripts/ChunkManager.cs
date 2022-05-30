using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class ChunkManager : MonoBehaviour {
    // references / objects
    public Transform observer;
    public Tilemap sandTilemap;

    // coordinate variables
    Vector2 viewerPosition;
    Vector2Int currentChunkCoord;
    Vector2Int lastChunkCoord;

    // chunk management dict, list and queues
    public Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    List<Chunk> loadedChunks = new List<Chunk>();
    Queue<Chunk> chunksToGenerate = new Queue<Chunk>();
    public Queue<Chunk> chunksToLoad = new Queue<Chunk>();
    Queue<Chunk> chunksToUnload = new Queue<Chunk>();

    // reference to the tile files
    public static TileResources tileResources;

    static WorldSettings worldSettings;

    public Dictionary<Vector2Int, VillageGenerator> newlyGeneratedVillages = new Dictionary<Vector2Int, VillageGenerator>();

    void Start() {
        worldSettings = GameObject.FindWithTag("SystemPlaceholder").GetComponent<WorldSettings>();
        tileResources = new TileResources();

        // initalise positions
        viewerPosition = new Vector2(observer.position.x, observer.position.y);
        currentChunkCoord = GetChunkCoord(viewerPosition);
        lastChunkCoord = GetChunkCoord(viewerPosition);

        updateChunks();
    }



    void Update() {
        // get viewer and chunk position
        viewerPosition = new Vector2(observer.position.x, observer.position.y);
        currentChunkCoord = GetChunkCoord(viewerPosition);

        // finishing generating chunks that need to be generated
        if (chunksToGenerate.Count > 0) {
            Chunk chunk = chunksToGenerate.Dequeue();
            chunk.GenerateChunkData();
            chunk.status = ChunkStatus.UNLOADED;
        }

        // fininish loading chunks that need to be loaded
        // do not enqueue already loaded chunks!
        if (chunksToLoad.Count > 0) {
            Chunk chunk = chunksToLoad.Peek();
            if (isWithinRenderDistance(chunk)) {
                // should only load chunk if it is unloaded
                if (chunk.status == ChunkStatus.UNLOADED) {
                    chunksToLoad.Dequeue();
                    chunk.LoadChunk();
                    loadedChunks.Add(chunk);
                    chunk.status = ChunkStatus.LOADED;
                }
                // sometimes the same chunk is in the queue twice
                else if (chunk.status == ChunkStatus.LOADED) {
                    chunksToLoad.Dequeue();
                }
            }
            // don't bother loading if it is outside render distance
            else {
                chunksToLoad.Dequeue();
            }
        }

        // fininish unloading chunks
        if (chunksToUnload.Count > 0) {
            Chunk chunk = chunksToUnload.Peek();
            if (!isWithinRenderDistance(chunk)) {
                // should only unload chunk if it is already loaded
                if (chunk.status == ChunkStatus.LOADED) {
                    chunk.UnloadChunk();
                    loadedChunks.Remove(chunk);
                    chunk.status = ChunkStatus.UNLOADED;
                }
                chunksToUnload.Dequeue();
            }
        }

        // if player moved a chunk, update chunks
        if (currentChunkCoord.x != lastChunkCoord.x
        || currentChunkCoord.y != lastChunkCoord.y) {
            updateChunks();
            lastChunkCoord = currentChunkCoord;
        }
    }

    public Vector2Int GetChunkCoord(Vector2 pos) {
        Vector2Int chunkCoord = new Vector2Int();
        chunkCoord.x = Mathf.FloorToInt(pos.x / Consts.CHUNK_SIZE);
        chunkCoord.y = Mathf.FloorToInt(pos.y / Consts.CHUNK_SIZE);
        return chunkCoord;
    }

    void updateChunks() {
        enqueueChunksToUnload();
        int renderDist = worldSettings.RENDER_DIST;

        // go through neighbouring chunks that need to be rendered
        for (int x = -renderDist; x <= renderDist; x++) {
            for (int y = -renderDist; y <= renderDist; y++) {
                Vector2Int chunkCoord = new Vector2Int(
                    currentChunkCoord.x + x, currentChunkCoord.y + y);

                // if chunk has been encountered before, load it
                if (chunkDictionary.ContainsKey(chunkCoord)) {
                    Chunk chunk = chunkDictionary[chunkCoord];
                    if (chunk.status != ChunkStatus.LOADED) {
                        chunksToLoad.Enqueue(chunk);
                    }
                } else {
                    // else, generate new chunk
                    Chunk chunk = new Chunk(chunkCoord);
                    chunkDictionary.Add(chunkCoord, chunk);
                    chunksToGenerate.Enqueue(chunk);
                }
            }
        }
    }

    void enqueueChunksToUnload() {
        for (int i = 0; i < loadedChunks.Count; i++) {
            Chunk chunk = loadedChunks[i];
            if (!isWithinRenderDistance(chunk)) {
                chunksToUnload.Enqueue(chunk);
            }
        }
    }

    bool isWithinRenderDistance(Chunk chunk) {
        if ((Mathf.Abs(currentChunkCoord.x - chunk.chunkCoord.x) > worldSettings.RENDER_DIST) ||
            (Mathf.Abs(currentChunkCoord.y - chunk.chunkCoord.y) > worldSettings.RENDER_DIST)) {
            return false;
        }
        return true;
    }
}
