using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkPos;
    public Vector2Int chunkCoord;

    int[] objects;
    int[] wallTiles;
    bool[] sandTiles;
    bool[] pathTiles;
    FloorData floorTiles;

    public ChunkData(Chunk chunk) {
        // world position of bottom-left tile in chunk
        this.chunkPos = chunk.chunkPos;
        // number of chunks away from 0,0
        this.chunkCoord = chunk.chunkCoord;

        this.floorTiles = new FloorData();
    }
}

[System.Serializable]
public class FloorData
{
    bool[] concreteFloorTiles;
    bool[] darkBrickFloorTiles;
    bool[] lightBrickFloorTiles;
    bool[] lightWoodFloorTiles;
    bool[] midWoodFloorTiles;
    bool[] darkWoodFloorTiles;
    bool[] stoneTileFloorTiles;
}

