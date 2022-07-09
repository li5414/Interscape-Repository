using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{   
    public PlayerData playerData = new PlayerData();
    public WorldData worldData;

    public GameData(int seed, string seedString) {
        this.worldData = new WorldData(seed, seedString);
    }
}

[System.Serializable]
public class PlayerData
{   
    public Vector3 position = new Vector3(1000, 1000);
}

[System.Serializable]
public class WorldData
{   
    public int seed;
    public string seedString;
    public Dictionary<Vector2Int, ChunkData> chunkData = new Dictionary<Vector2Int, ChunkData>();

    public WorldData(int seed, string seedString) {
        this.seed = seed;
        this.seedString = seedString;
    }
}

