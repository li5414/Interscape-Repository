using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{   
    public PlayerData playerData = new PlayerData();
    public WorldData worldData;

    public GameData(int seed) {
        this.worldData = new WorldData(seed);
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

    public WorldData(int seed) {
        this.seed = seed;
    }
}

