using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VillageGenerator : MonoBehaviour {
    void Start() {
        string jsonPath = "Assets/Resources/Buildings/BuildingRules.json";
        string jsonStr = File.ReadAllText(jsonPath);

        BuildingRules buildingRules = JsonUtility.FromJson<BuildingRules>(jsonStr);
        Debug.Log(buildingRules.buildingRules.Length);
    }
}

[System.Serializable]
public class BuildingRules {
    public BuildingRule[] buildingRules;
}

[System.Serializable]
public class BuildingRule {
    public int[][] connectionPoints;
    public string[][] layout;
}
