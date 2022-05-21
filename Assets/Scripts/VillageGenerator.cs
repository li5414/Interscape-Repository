using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;


public class VillageGenerator : MonoBehaviour {
    BuildingRule[] BUILDING_RULES;
    public RuleTile pathTileReference;
    public Tilemap pathTilemapReference;
    void Start() {
        string jsonPath = "Assets/Resources/Buildings/BuildingRules.json";
        string jsonStr = File.ReadAllText(jsonPath);

        BuildingRulesRoot root = JsonUtility.FromJson<BuildingRulesRoot>(jsonStr);
        BUILDING_RULES = root.buildingRulesArray;
        Debug.Log(BUILDING_RULES[0].layout);
        GenerateVillage(new Vector2Int(0, 0));
    }

    public void GenerateVillage(Vector2Int centerPoint) {
        List<BuildingRule> buildingRuleQueue = new List<BuildingRule>();
        List<BuildingLayout> currentBuildings = new List<BuildingLayout>();
        buildingRuleQueue.Add(BUILDING_RULES[0]);
        currentBuildings.Add(new BuildingLayout {
            worldCoordinates = centerPoint,
            layout = BUILDING_RULES[0].layout
        });
        // Debug.Log(BUILDING_RULES[0].layout);

        drawToWorld(currentBuildings);
    }

    private void drawToWorld(List<BuildingLayout> buildingLayouts) {
        foreach (BuildingLayout buildingLayout in buildingLayouts) {
            for (int y = 0; y < buildingLayout.layout.Length; y++) {
                for (int x = 0; x < buildingLayout.layout[y].Length; x++) {
                    char c = getChar(buildingLayout.layout, x, y);

                    if (c == '_') {
                        pathTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), pathTileReference);
                    }
                }
            }
        }
    }

    private char getChar(string[] layout, int x, int y) {
        return layout[layout.Length - 1 - y][x];
    }
}

public class BuildingLayout {
    public Vector2Int worldCoordinates { get; set; }
    public string[] layout { get; set; }

    public Vector3Int GetWorldPos(int x, int y) {
        // TODO figure out if z matters
        return new Vector3Int(worldCoordinates.x + x, worldCoordinates.y + y, 0);
    }
}

[System.Serializable]
public class BuildingRulesRoot {
    public BuildingRule[] buildingRulesArray;
}

[System.Serializable]
public class BuildingRule {
    public Vector2Int topConnectionPoint;
    public Vector2Int bottomConnectionPoint;
    public Vector2Int leftConnectionPoint;
    public Vector2Int rightConnectionPoint;

    public string[] layout;
}
