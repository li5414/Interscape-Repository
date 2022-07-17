using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class ChunkData {
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

        this.objects = new int[Consts.CHUNK_SIZE_SQUARED];
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                GameObject objToSave = chunk.objects[i, j];

                if (objToSave != null) {
                    this.objects[Consts.IndexOf(i, j)] = objToSave.GetComponent<SaveId>().id;
                }

            }
        }

        // set sand tiles
        if (chunk.sandTiles != null) {
            for (int i = 0; i < chunk.sandTiles.Length; i++) {
                if (chunk.sandTiles[i] != null) {
                    if (sandTiles == null) {
                        sandTiles = new bool[Consts.CHUNK_SIZE_SQUARED];
                    }
                    sandTiles[i] = true;
                }
            }
        }

        // set wall tiles
        if (chunk.wallTiles != null) {
            for (int i = 0; i < chunk.wallTiles.Length; i++) {
                if (chunk.wallTiles[i] != null) {
                    if (wallTiles == null) {
                        wallTiles = new int[Consts.CHUNK_SIZE_SQUARED];
                    }
                    wallTiles[i] = ((RuleTile)chunk.wallTiles[i]).m_DefaultGameObject.GetComponent<SaveId>().id;
                }
            }
        }

        this.floorTiles = new FloorData(chunk.floorTileData);
    }

    public static TileBase[] GenerateTiles(bool[] tileBools, RuleTile tile) {
        TileBase[] tiles = null;
        if (tileBools != null) {
            for (int i = 0; i < tileBools.Length; i++) {
                if (tileBools[i]) {
                    if (tiles == null) {
                        tiles = new TileBase[tileBools.Length];
                    }
                    tiles[i] = tile;
                }
            }
        }
        return tiles;
    }

    public static TileBase[] GenerateTiles(int[] tileIds, Dictionary<int, RuleTile> tileIdDict) {
        TileBase[] tiles = null;
        if (tileIds != null) {
            for (int i = 0; i < tileIds.Length; i++) {
                if (tileIds[i] > 0) {
                    if (tiles == null) {
                        tiles = new TileBase[tileIds.Length];
                    }
                    tiles[i] = tileIdDict[tileIds[i]];
                }
            }
        }
        return tiles;
    }
}

[System.Serializable]
public class FloorData {
    public bool[] concreteFloorTiles;
    public bool[] darkBrickFloorTiles;
    public bool[] lightBrickFloorTiles;
    public bool[] lightWoodFloorTiles;
    public bool[] midWoodFloorTiles;
    public bool[] darkWoodFloorTiles;
    public bool[] stoneTileFloorTiles;

    public List<bool[]> GetAll() {
        return new List<bool[]>{
            concreteFloorTiles,
            darkBrickFloorTiles,
            lightBrickFloorTiles,
            lightWoodFloorTiles,
            midWoodFloorTiles,
            darkWoodFloorTiles,
            stoneTileFloorTiles};
    }

    public FloorData(TileBase[][] floorTileData) {
        this.concreteFloorTiles = TileToBooleanArray(floorTileData[0]);
        this.darkBrickFloorTiles = TileToBooleanArray(floorTileData[1]);
        this.lightBrickFloorTiles = TileToBooleanArray(floorTileData[2]);
        this.lightWoodFloorTiles = TileToBooleanArray(floorTileData[3]);
        this.midWoodFloorTiles = TileToBooleanArray(floorTileData[4]);
        this.darkWoodFloorTiles = TileToBooleanArray(floorTileData[5]);
        this.stoneTileFloorTiles = TileToBooleanArray(floorTileData[6]);
    }

    public bool[] TileToBooleanArray(TileBase[] tileArray) {
        if (tileArray == null)
            return null;
        bool[] boolArray = new bool[tileArray.Length];
        for (int i = 0; i < tileArray.Length; i++) {
            boolArray[i] = tileArray[i] != null;
        }
        return boolArray;
    }
}

