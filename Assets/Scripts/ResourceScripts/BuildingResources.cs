using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BuildingResources : MonoBehaviour {
    public GameObject villageObject;
    public Tilemap sandTilemap;
    public Tilemap pathTilemap;
    public Tilemap wallTilemap;
    public Tilemap concreteFloorTilemap;
    public Tilemap darkBrickFloorTilemap;
    public Tilemap lightBrickFloorTilemap;
    public Tilemap lightWoodFloorTilemap;
    public Tilemap midWoodFloorTilemap;
    public Tilemap darkWoodFloorTilemap;
    public Tilemap stoneTileFloorTilemap;

    public RuleTile[] wallTiles;
    public RuleTile[] doorTiles;

    void Awake() {
        TilemapDict = new Dictionary<string, Tilemap>{
        {"ConcreteFloorTilemap", concreteFloorTilemap},
        {"DarkBrickFloorTilemap", darkBrickFloorTilemap},
        {"LightBrickFloorTilemap", lightBrickFloorTilemap},
        {"LightWoodFloorTilemap", lightWoodFloorTilemap},
        {"MidWoodFloorTilemap", midWoodFloorTilemap},
        {"DarkWoodFloorTilemap", darkWoodFloorTilemap},
        {"StoneTileFloorTilemap", stoneTileFloorTilemap},
        };
    }

    public Dictionary<string, Tilemap> TilemapDict;
}
