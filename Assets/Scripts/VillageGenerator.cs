using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class VillageGenerator : MonoBehaviour {
    BuildingRule[] BUILDING_RULES;
    BuildingRule[] PATH_RULES;
    public GameObject NPC;
    public RuleTile pathTileReference;
    public RuleTile wallTileReference;
    public RuleTile doorTileReference;
    public RuleTile floorTileReference;

    public Tilemap pathTilemapReference;
    public Tilemap wallTilemapReference;
    public Tilemap floorTilemapReference;

    List<BuildingLayout> currentBuildings = new List<BuildingLayout>();
    List<BuildingRule> buildingRuleQueue = new List<BuildingRule>();
    void Start() {
        string jsonPath = "Assets/Resources/Buildings/BuildingRules.json";
        string jsonStr = File.ReadAllText(jsonPath);
        BuildingRulesRoot root = JsonUtility.FromJson<BuildingRulesRoot>(jsonStr);
        BUILDING_RULES = root.buildingRulesArray;

        string jsonPath2 = "Assets/Resources/Buildings/PathRules.json";
        string jsonStr2 = File.ReadAllText(jsonPath2);
        BuildingRulesRoot root2 = JsonUtility.FromJson<BuildingRulesRoot>(jsonStr2);
        PATH_RULES = root2.buildingRulesArray;

        GenerateVillage(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }

    public void GenerateVillage(Vector2Int centerPoint) {
        buildingRuleQueue.Add(PATH_RULES[0]);
        currentBuildings.Add(new BuildingLayout {
            worldCoordinates = centerPoint,
            layout = PATH_RULES[0].layout
        });
        int currentBuildingIndex = 0;
        int safeguard = 100;
        while (buildingRuleQueue.Count > 0 && safeguard > 0) {
            BuildingRule currentBuilding = buildingRuleQueue[0];

            if (currentBuilding.topConnectionPoint.x != -1 &&
            !currentBuildings[currentBuildingIndex].isConnectedUp) {
                addNewBuilding(Vector2Int.up, currentBuildingIndex, currentBuilding);
            }
            if (currentBuilding.bottomConnectionPoint.x != -1 && !currentBuildings[currentBuildingIndex].isConnectedDown) {
                addNewBuilding(Vector2Int.down, currentBuildingIndex, currentBuilding);
            }
            if (currentBuilding.rightConnectionPoint.x != -1 && !currentBuildings[currentBuildingIndex].isConnectedRight) {
                addNewBuilding(Vector2Int.right, currentBuildingIndex, currentBuilding);
            }
            if (currentBuilding.leftConnectionPoint.x != -1 &&
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

        if (direction.Equals(Vector2Int.up)) {
            currentConnectionPoint = currentBuilding.topConnectionPoint;
            nextConnectionPoint = nextBuilding.bottomConnectionPoint;

        } else if (direction.Equals(Vector2Int.down)) {
            currentConnectionPoint = currentBuilding.bottomConnectionPoint;
            nextConnectionPoint = nextBuilding.topConnectionPoint;

        } else if (direction.Equals(Vector2Int.left)) {
            currentConnectionPoint = currentBuilding.leftConnectionPoint;
            nextConnectionPoint = nextBuilding.rightConnectionPoint;

        } else {
            currentConnectionPoint = currentBuilding.rightConnectionPoint;
            nextConnectionPoint = nextBuilding.leftConnectionPoint;
        }

        // get world coordinates of the appropriate connection point of the existing building we want to connect to
        Vector2Int worldConnectionPos = (Vector2Int)(currentBuildings[currentBuildingIndex].GetWorldPos(currentConnectionPoint.x, currentConnectionPoint.y));

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

        // Debug.Log(worldConnectionPos + " " + nextConnectionPoint);
        if (direction.Equals(Vector2Int.up))
            newBuildingLayout.isConnectedDown = true;
        else if (direction.Equals(Vector2Int.down))
            newBuildingLayout.isConnectedUp = true;
        else if (direction.Equals(Vector2Int.left))
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
        BuildingRule[] RULES = BUILDING_RULES;
        if (Random.value < 0.6)
            RULES = PATH_RULES.Skip(1).ToArray();

        foreach (BuildingRule building in RULES) {
            if (direction == Vector2Int.up &&
                building.bottomConnectionPoint.x != -1) {
                possibilities.Add(building);
            } else if (direction == Vector2Int.down &&
                  building.topConnectionPoint.x != -1) {
                possibilities.Add(building);
            } else if (direction == Vector2Int.left &&
                  building.rightConnectionPoint.x != -1) {
                possibilities.Add(building);
            } else if (direction == Vector2Int.right &&
                  building.leftConnectionPoint.x != -1) {
                possibilities.Add(building);
            }
        }

        if (possibilities.Count <= 0) {
            return null;
        }
        return possibilities[Random.Range(0, possibilities.Count)];
    }

    private void drawToWorld() {
        foreach (BuildingLayout buildingLayout in currentBuildings) {
            for (int y = 0; y < buildingLayout.layout.Length; y++) {
                for (int x = 0; x < buildingLayout.layout[y].Length; x++) {
                    char c = getChar(buildingLayout.layout, x, y);

                    if (c == '_') {
                        pathTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), pathTileReference);
                    } else if (c == 'W') {
                        wallTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), wallTileReference);

                        pathTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), pathTileReference);

                        floorTilemapReference.SetTile(floorTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), floorTileReference);

                    } else if (c == 'D') {
                        wallTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), doorTileReference);

                        pathTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), pathTileReference);

                        floorTilemapReference.SetTile(floorTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), floorTileReference);

                    } else if (c == '-') {
                        pathTilemapReference.SetTile(pathTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), pathTileReference);

                        floorTilemapReference.SetTile(floorTilemapReference.WorldToCell(buildingLayout.GetWorldPos(x, y)), floorTileReference);

                        // spawn NPC
                        if (Random.value < 0.05) {
                            Instantiate(NPC, buildingLayout.GetWorldPos(x, y), Quaternion.identity);
                        }
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
