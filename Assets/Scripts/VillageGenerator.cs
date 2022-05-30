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

    List<BuildingLayout> currentBuildings = new List<BuildingLayout>();
    List<BuildingRule> buildingRuleQueue = new List<BuildingRule>();

    private System.Random prng;

    static BuildingResources res;
    void Start() {
        if (res == null) {
            res = GameObject.FindWithTag("SystemPlaceholder").GetComponent<BuildingResources>();
        }

        int seed = GameObject.FindWithTag("SystemPlaceholder").GetComponent<WorldSettings>().SEED;
        prng = new System.Random((int)transform.position.x + (int)transform.position.y * seed);

        BUILDING_RULES = loadJSON("Assets/Resources/Buildings/BuildingRules.json");
        PATH_RULES = loadJSON("Assets/Resources/Buildings/PathRules.json");

        GenerateVillage(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }

    private BuildingRule[] loadJSON(string filePath) {
        string jsonStr = File.ReadAllText(filePath);
        BuildingRulesRoot root = JsonUtility.FromJson<BuildingRulesRoot>(jsonStr);
        return root.buildingRulesArray;
    }

    public static VillageGenerator SpawnVillage(Vector2Int chunkPos) {
        if (res == null) {
            res = GameObject.FindWithTag("SystemPlaceholder").GetComponent<BuildingResources>();
        }
        return Instantiate(res.villageObject, new Vector3(chunkPos.x, chunkPos.y, 0), Quaternion.identity).GetComponent<VillageGenerator>();
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

        // placeVillageInWorld();
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

        if (overlapsAny(newBuildingLayout))
            return;
        if (!isWithinBounds(newBuildingLayout)) {
            Debug.Log("prevented out of bounds");
            return;
        }

        if (direction.Equals(Vector2Int.up))
            newBuildingLayout.isConnectedDown = true;
        else if (direction.Equals(Vector2Int.down))
            newBuildingLayout.isConnectedUp = true;
        else if (direction.Equals(Vector2Int.left))
            newBuildingLayout.isConnectedRight = true;
        else
            newBuildingLayout.isConnectedLeft = true;
        newBuildingLayout.floorTilemap = res.TilemapDict.ElementAt(prng.Next(0, res.TilemapDict.Count)).Value;
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

    private bool isWithinBounds(BuildingLayout newBuilding) {
        // make sure village's width and height is less than 5 chunks
        // village center is at bottom left corner of it's chunk
        Vector2Int bottomLeft = new Vector2Int(
            (int)gameObject.transform.position.x - Consts.CHUNK_SIZE * 2,
            (int)gameObject.transform.position.y - Consts.CHUNK_SIZE * 2);
        Vector2Int topRight = new Vector2Int(
            (int)gameObject.transform.position.x + Consts.CHUNK_SIZE * 3 - 1,
            (int)gameObject.transform.position.y + Consts.CHUNK_SIZE * 3 - 1);

        Vector2Int newBottomLeft = newBuilding.worldCoordinates;
        Vector2Int newTopRight = new Vector2Int(
            newBottomLeft.x + newBuilding.layout[0].Length - 1,
            newBottomLeft.y + newBuilding.layout.Length - 1);

        if (newTopRight.y < bottomLeft.y
        || newBottomLeft.y > topRight.y) {
            return false;
        }
        if (newTopRight.x < bottomLeft.x
        || newBottomLeft.x > topRight.x) {
            return false;
        }
        return true;
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
        if (prng.NextDouble() < 0.6)
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
        return possibilities[prng.Next(0, possibilities.Count)];
    }

    private Vector3Int tilePos(int x, int y, BuildingLayout buildingLayout) {
        return res.pathTilemap.WorldToCell(buildingLayout.GetWorldPos(x, y));
    }

    private void placeTileInWorld(char c, Tilemap floorTilemap, BuildingLayout buildingLayout, Vector3Int pos) {

        switch (c) {
            case '_':
                res.pathTilemap.SetTile(pos, pathTileReference);
                break;
            case 'W':
                res.wallTilemap.SetTile(pos, wallTileReference);
                res.pathTilemap.SetTile(pos, pathTileReference);
                floorTilemap.SetTile(pos, floorTileReference);
                break;
            case 'D':
                res.wallTilemap.SetTile(pos, doorTileReference);
                res.pathTilemap.SetTile(pos, pathTileReference);
                floorTilemap.SetTile(pos, floorTileReference);
                break;
            case '-':
                res.pathTilemap.SetTile(pos, pathTileReference);
                floorTilemap.SetTile(pos, floorTileReference);
                // spawn NPC
                if (prng.NextDouble() < 0.05) {
                    Instantiate(NPC, pos, Quaternion.identity);
                }
                break;
        }
    }

    private void placeVillageInWorld() {
        foreach (BuildingLayout buildingLayout in currentBuildings) {
            for (int y = 0; y < buildingLayout.layout.Length; y++) {
                for (int x = 0; x < buildingLayout.layout[y].Length; x++) {
                    char c = getChar(buildingLayout.layout, x, y);
                    Vector3Int pos = tilePos(x, y, buildingLayout);
                    RemoveObstacle(pos.x, pos.y);
                    placeTileInWorld(c, buildingLayout.floorTilemap, buildingLayout, pos);
                }
            }
        }
    }
    public void PlaceVillageChunkInWorld(Vector2Int chunkPos) {
        foreach (BuildingLayout buildingLayout in currentBuildings) {
            for (int y = 0; y < buildingLayout.layout.Length; y++) {
                for (int x = 0; x < buildingLayout.layout[y].Length; x++) {
                    char c = getChar(buildingLayout.layout, x, y);
                    Vector3Int pos = tilePos(x, y, buildingLayout);
                    if (pos.x >= chunkPos.x && pos.x < chunkPos.x + Consts.CHUNK_SIZE && pos.y >= chunkPos.y && pos.y < chunkPos.y + Consts.CHUNK_SIZE) {
                        RemoveObstacle(pos.x, pos.y);
                        placeTileInWorld(c, buildingLayout.floorTilemap, buildingLayout, pos);
                    }
                }
            }
        }
    }
    public bool RemoveObstacle(int x, int y) {
        // TODO figure out if z is important here
        Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 20);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, -Vector2.zero);

        for (int i = 0; i < hits.Length; i++) {
            Collider2D collider = hits[i].transform.gameObject.GetComponent<Collider2D>();
            if (collider != null && collider.gameObject.tag != "ItemDrop" && collider.gameObject.tag != "Player" && collider.gameObject.tag != "NPC") {
                Destroy(collider.gameObject);
            }
        }

        return true;
    }

    private char getChar(string[] layout, int x, int y) {
        return layout[layout.Length - 1 - y][x];
    }
}

public class BuildingLayout {
    public Vector2Int worldCoordinates { get; set; }
    public Tilemap floorTilemap;
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
