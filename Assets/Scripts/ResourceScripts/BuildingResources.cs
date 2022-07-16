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

    public Dictionary<int, RuleTile> idToRuleTile;
    public Dictionary<string, Tilemap> TilemapDict;


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

        idToRuleTile = new Dictionary<int, RuleTile>{
            {1, wallTiles[0]},
            {2, wallTiles[1]},
            {3, wallTiles[2]},
            {4, doorTiles[0]},
            {5, doorTiles[1]},
            {6, doorTiles[2]},
        };
    }

    public List<Tilemap> getAllFloorTilemaps() {
        return new List<Tilemap>{
            concreteFloorTilemap,
            darkBrickFloorTilemap,
            lightBrickFloorTilemap,
            lightWoodFloorTilemap,
            midWoodFloorTilemap,
            darkWoodFloorTilemap,
            stoneTileFloorTilemap};
    }
}
