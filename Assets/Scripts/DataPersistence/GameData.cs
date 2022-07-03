using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{   
    public PlayerData playerData = new PlayerData();
}

[System.Serializable]
public class PlayerData
{   
    public Vector3 position = new Vector3(1000, 1000);
}


