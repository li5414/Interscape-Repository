using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;


public class VillageGenerator : MonoBehaviour {
    BuildingRule[] BUILDING_RULES;
    public RuleTile pathTileReference;
    public Tilemap pathTilemapReference;
    List<BuildingLayout> currentBuildings = new List<BuildingLayout>();
    List<BuildingRule> buildingRuleQueue = new List<BuildingRule>();
    void Start() {
        string jsonPath = "Assets/Resources/Buildings/BuildingRules.json";
        string jsonStr = File.ReadAllText(jsonPath);

        BuildingRulesRoot root = JsonUtility.FromJson<BuildingRulesRoot>(jsonStr);
        BUILDING_RULES = root.buildingRulesArray;
        Debug.Log(BUILDING_RULES[0].layout);
        GenerateVillage(new Vector2Int(0, 0));
    }

    public void GenerateVillage(Vector2Int centerPoint) {
        buildingRuleQueue.Add(BUILDING_RULES[0]);
        currentBuildings.Add(new BuildingLayout {
            worldCoordinates = centerPoint,
            layout = BUILDING_RULES[0].layout
        });
        int currentBuildingIndex = 0;
        int safeguard = 100;
        while (buildingRuleQueue.Count > 0 && safeguard > 0) {
            BuildingRule currentBuilding = buildingRuleQueue[0];

            if (currentBuilding.topConnectionPoint != null &&
            !currentBuildings[currentBuildingIndex].isConnectedUp) {
                addNewBuilding(Vector2Int.up, currentBuildingIndex, currentBuilding);
            }
            if (currentBuilding.bottomConnectionPoint != null &&
            !currentBuildings[currentBuildingIndex].isConnectedDown) {
                addNewBuilding(Vector2Int.down, currentBuildingIndex, currentBuilding);
            }
            if (currentBuilding.rightConnectionPoint != null &&
            !currentBuildings[currentBuildingIndex].isConnectedRight) {
                addNewBuilding(Vector2Int.right, currentBuildingIndex, currentBuilding);
            }
            if (currentBuilding.leftConnectionPoint != null &&
            !currentBuildings[currentBuildingIndex].isConnectedLeft) {
                addNewBuilding(Vector2Int.left, currentBuildingIndex, currentBuilding);
            }


            buildingRuleQueue.RemoveAt(0);
            currentBuildingIndex++;
            safeguard--;
        }

        drawToWorld();
    }

    private void addNewBuilding(Vector2Int direction, int currentBuildingIndex, BuildingRule currentBuilding) {
        BuildingRule nextBuilding = pickNextBuilding(direction);
        if (nextBuilding == null)
            return;

        // figure out which connection point we want to use from the existing building we want to connect to
        Vector2Int currentConnectionPoint;
        Vector2Int nextConnectionPoint;

        if (direction == Vector2Int.up) {
            currentConnectionPoint = currentBuilding.topConnectionPoint;
            nextConnectionPoint = nextBuilding.bottomConnectionPoint;

        } else if (direction == Vector2Int.down) {
            currentConnectionPoint = currentBuilding.bottomConnectionPoint;
            nextConnectionPoint = nextBuilding.topConnectionPoint;

        } else if (direction == Vector2Int.left) {
            currentConnectionPoint = currentBuilding.leftConnectionPoint;
            nextConnectionPoint = nextBuilding.rightConnectionPoint;

        } else {
            currentConnectionPoint = currentBuilding.rightConnectionPoint;
            nextConnectionPoint = nextBuilding.leftConnectionPoint;
        }

        // get world coordinates of the appropriate connection point of the existing building we want to connect to
        Vector2Int worldConnectionPos = (Vector2Int)currentBuildings[currentBuildingIndex].GetWorldPos(currentConnectionPoint.x, currentConnectionPoint.y);

        // add the new bulding to list of current buildings
        BuildingLayout newBuildingLayout = new BuildingLayout {
            worldCoordinates = alignedBuildingWorldPos(
                worldConnectionPos,
                nextConnectionPoint,
                direction),
            layout = nextBuilding.layout
        };

        if (overlapsAny(newBuildingLayout)) {
            return;
        }

        if (direction == Vector2Int.up)
            newBuildingLayout.isConnectedDown = true;
        else if (direction == Vector2Int.down)
            newBuildingLayout.isConnectedUp = true;
        else if (direction == Vector2Int.left)
            newBuildingLayout.isConnectedRight = true;
        else
            newBuildingLayout.isConnectedLeft = true;
        currentBuildings.Add(newBuildingLayout);
        buildingRuleQueue.Add(nextBuilding);
    }

    private bool overlapsAny(BuildingLayout newBuilding) {
        foreach (BuildingLayout oldBuilding in currentBuildings) {
            if (overlaps(newBuilding, oldBuilding))
                return true;
        }
        return false;
    }

    private bool overlaps(BuildingLayout newBuilding,
    BuildingLayout oldBuilding) {
        Vector2Int oldBottomLeft = oldBuilding.worldCoordinates;
        Vector2Int oldTopRight = new Vector2Int(
            oldBottomLeft.x + oldBuilding.layout[0].Length - 1,
            oldBottomLeft.y + oldBuilding.layout.Length - 1);

        Vector2Int newBottomLeft = newBuilding.worldCoordinates;
        Vector2Int newTopRight = new Vector2Int(
            newBottomLeft.x + newBuilding.layout[0].Length - 1,
            newBottomLeft.y + newBuilding.layout.Length - 1);

        if (newTopRight.y < oldBottomLeft.y
        || newBottomLeft.y > oldTopRight.y) {
            return false;
        }
        if (newTopRight.x < oldBottomLeft.x
        || newBottomLeft.x > oldTopRight.x) {
            return false;
        }
        return true;
    }

    private Vector2Int alignedBuildingWorldPos(Vector2Int worldConnectionPos, Vector2Int buildingConnectionPos, Vector2Int directionOffset) {
        return new Vector2Int(
            worldConnectionPos.x + directionOffset.x - buildingConnectionPos.x, worldConnectionPos.y + directionOffset.y - buildingConnectionPos.y);
    }

    private BuildingRule pickNextBuilding(Vector2Int direction) {
        List<BuildingRule> possibilities = new List<BuildingRule>();

        foreach (BuildingRule building in BUILDING_RULES) {
            if (direction == Vector2Int.up &&
                building.bottomConnectionPoint != null) {
                possibilities.Add(building);
            } else if (direction == Vector2Int.down &&
                  building.topConnectionPoint != null) {
                possibilities.Add(building);
            } else if (direction == Vector2Int.left &&
                  building.rightConnectionPoint != null) {
                possibilities.Add(building);
            } else if (direction == Vector2Int.right &&
                  building.leftConnectionPoint != null) {
                possibilities.Add(building);
            }
        }

        if (possibilities.Count <= 0) {
            return null;
        }
        return possibilities[Random.Range(0, possibilities.Count - 1)];
    }

    private void drawToWorld() {
        foreach (BuildingLayout buildingLayout in currentBuildings) {
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

    public bool isConnectedUp = false;
    public bool isConnectedDown = false;
    public bool isConnectedLeft = false;
    public bool isConnectedRight = false;

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
