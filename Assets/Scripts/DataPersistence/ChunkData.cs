using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkPos;
    public Vector2Int chunkCoord;

    public int[] objects;
    public int[] wallTiles;
    public bool[] sandTiles;
    public bool[] pathTiles;
    public FloorData floorTiles;

    public ChunkData(Chunk chunk) {
        // world position of bottom-left tile in chunk
        this.chunkPos = chunk.chunkPos;
        // number of chunks away from 0,0
        this.chunkCoord = chunk.chunkCoord;

        this.objects = new int[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                GameObject objToSave = chunk.objects[i, j];

                if (objToSave != null) {
                    this.objects[IndexOf(i, j)] = objToSave.GetComponent<SaveId>().id;
                }
                
            }
        }

        // set sand tiles
        if (chunk.sandTiles != null) {
            for (int i = 0; i < chunk.sandTiles.Length; i++) {
                if (chunk.sandTiles[i] != null) {
                    if (sandTiles == null) {
                        sandTiles = new bool[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];
                    }
                    sandTiles[i] = true;
                }
            }
        }

        this.floorTiles = new FloorData();
    }

    // lookup index of 16x16 2D array condensed to 1D array
    public int IndexOf(int x, int y) {
        return (x * 16 + y);
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

[System.Serializable]
public class FloorRuleTileData
{
    RuleTile[] concreteFloorTiles;
    RuleTile[] darkBrickFloorTiles;
    RuleTile[] lightBrickFloorTiles;
    RuleTile[] lightWoodFloorTiles;
    RuleTile[] midWoodFloorTiles;
    RuleTile[] darkWoodFloorTiles;
    RuleTile[] stoneTileFloorTiles;
}

