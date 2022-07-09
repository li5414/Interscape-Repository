using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkPos;
    public Vector2Int chunkCoord;
    public float[,] heights;

    int[,] objects;
    int[,] wallTiles;
    bool[,] sandTiles;
    bool[,] pathTiles;
    bool[,,] floorTiles;

    public ChunkData(Vector2Int chunkCoord, TerrainDataGenerator terrainData) {
        // world position of bottom-left tile in chunk
        this.chunkPos = new Vector2Int(
            chunkCoord.x * Consts.CHUNK_SIZE, 
            chunkCoord.y * Consts.CHUNK_SIZE);
        
        // number of chunks away from 0,0
        this.chunkCoord = chunkCoord;

        this.heights = terrainData.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
    }
}
