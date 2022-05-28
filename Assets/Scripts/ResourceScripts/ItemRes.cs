using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemRes : MonoBehaviour {
    private static Sprite[] sprites = Resources.LoadAll<Sprite>("Items/ItemsVer1");
    private static Sprite[] terrainToolSprites = Resources.LoadAll<Sprite>("Items/TerrainTools");

    private static Sprite[] cobbleWallSprites = Resources.LoadAll<Sprite>("Items/CobblestoneWallIcons");
    private static RuleTile lightCobbleWall = Resources.Load<RuleTile>("Buildings/LightCobblestoneWall");
    private static RuleTile medCobbleWall = Resources.Load<RuleTile>("Buildings/CobblestoneWall");
    private static RuleTile darkCobbleWall = Resources.Load<RuleTile>("Buildings/DarkCobblestoneWall");

    private static Sprite[] woodDoorSprites = Resources.LoadAll<Sprite>("Items/WoodenDoorIcons");
    private static RuleTile lightWoodDoor = Resources.Load<RuleTile>("Buildings/DoorPrefabs/LightWoodDoor");
    private static RuleTile midWoodDoor = Resources.Load<RuleTile>("Buildings/DoorPrefabs/MidWoodDoor");
    private static RuleTile darkWoodDoor = Resources.Load<RuleTile>("Buildings/DoorPrefabs/DarkWoodDoor");
    private static RuleTile dirt = Resources.Load<RuleTile>("Paths/DirtTile");

    private static Sprite[] floorSprites = Resources.LoadAll<Sprite>("Items/FloorIcons");
    private static RuleTile floorBase = Resources.Load<RuleTile>("Floors/FloorBase");
    private static Material midWoodFloorMaterial = Resources.Load<Material>("Floors/MidWoodFloorMaterial");

    private static float defDur = 1;
    public static Dictionary<string, Item> ItemDict = new Dictionary<string, Item> {
                {"Axe", new Tool(
                    "Axe",
                    sprites[2],
                    "Good for killing trees (and anything really) \nbut quite heavy",
                    5,
                    defDur,
                    DamageType.Woodcutting,
                    300,
                    10,
                    0.5f)},
                {"Branch", new Item(
                    "Branch",
                    sprites[9],
                    "Tree arm",
                    2)},
                {"Log", new Item(
                    "Log",
                    sprites[10],
                    "Tree body",
                    6)},
                {"Pickaxe", new Tool(
                    "Pickaxe",
                    sprites[1],
                    "Good for breaking rocks",
                    3,
                    defDur,
                    DamageType.Rockbreaking,
                    300,
                    10,
                    0.5f)},
                {"Stone", new Item(
                    "Stone",
                    sprites[8],
                    "It's a stone.",
                    3)},
                {"Stone axe", new Tool(
                    "Stone axe",
                    sprites[5],
                    "A primitive axe",
                    3,
                    defDur,
                    DamageType.Woodcutting,
                    100,
                    5,
                    0.5f)},
                {"Hoe", new TerrainTool(
                    "Hoe",
                    terrainToolSprites[0],
                    ":o",
                    5,
                    defDur,
                    DamageType.Digging,
                    100,
                    5,
                    0.5f,
                    dirt,
                    "PathTilemap")},
                {"Shovel", new TerrainTool (
                    "Shovel",
                    terrainToolSprites[1],
                    "Let's get diggin'!",
                    5,
                    defDur,
                    DamageType.Digging,
                    100,
                    5,
                    0.5f,
                    dirt,
                    "PathTilemap")},
                {"Sword", new Tool (
                    "Sword",
                    sprites[0],
                    "Useful for murder",
                    2,
                    defDur,
                    DamageType.Slicing,
                    300,
                    10,
                    0.5f)},
                {"Light Cobblestone Wall", new BuildableItem(
                    "Light Cobblestone Wall",
                    cobbleWallSprites[0],
                    "A wall that looks like it's about to fall apart.",
                    6,
                    lightCobbleWall,
                    "WallTilemap")},
                {"Cobblestone Wall", new BuildableItem(
                    "Cobblestone Wall",
                    cobbleWallSprites[1],
                    "A wall that looks like it's about to fall apart.",
                    6,
                    medCobbleWall,
                    "WallTilemap")},
                {"Dark Cobblestone Wall", new BuildableItem(
                    "Dark Cobblestone Wall",
                    cobbleWallSprites[2],
                    "A wall that looks like it's about to fall apart.",
                    6,
                    darkCobbleWall,
                    "WallTilemap")},
                {"Light Wood Door", new BuildableItem(
                    "Light Wood Door",
                    woodDoorSprites[0],
                    "A fine choice of door.",
                    3,
                    lightWoodDoor,
                    "WallTilemap")},
                {"Mid Wood Door", new BuildableItem(
                    "Mid Wood Door",
                    woodDoorSprites[1],
                    "A fine choice of door.",
                    3,
                    midWoodDoor,
                    "WallTilemap")},
                { "Dark Wood Door", new BuildableItem(
                    "Dark Wood Door",
                    woodDoorSprites[2],
                    "A fine choice of door.",
                    3,
                    darkWoodDoor,
                    "WallTilemap")},
                {"Concrete Floor", new BuildableItem(
                    "Concrete Floor",
                    floorSprites[4],
                    "It's better than nothing.",
                    3,
                    floorBase,
                    "ConcreteFloorTilemap")},
                {"Dark Brick Floor", new BuildableItem(
                    "Dark Brick Floor",
                    floorSprites[1],
                    "A fine choice of floor.",
                    3,
                    floorBase,
                    "DarkBrickFloorTilemap")},
                {"Light Brick Floor", new BuildableItem(
                    "Light Brick Floor",
                    floorSprites[2],
                    "A fine choice of floor.",
                    3,
                    floorBase,
                    "LightBrickFloorTilemap")},
                {"Light Wood Floor", new BuildableItem(
                    "Light Wood Floor",
                    floorSprites[0],
                    "A fine choice of floor.",
                    3,
                    floorBase,
                    "LightWoodFloorTilemap")},
                {"Mid Wood Floor", new BuildableItem(
                    "Mid Wood Floor",
                    floorSprites[0],
                    "A fine choice of floor.",
                    3,
                    floorBase,
                    "MidWoodFloorTilemap")},
                {"Dark Wood Floor", new BuildableItem(
                    "Dark Wood Floor",
                    floorSprites[0],
                    "A fine choice of floor.",
                    3,
                    floorBase,
                    "DarkWoodFloorTilemap")},
                {"Stone Tile Floor", new BuildableItem(
                    "Stone Tile Floor",
                    floorSprites[3],
                    "A fine choice of floor.",
                    3,
                    floorBase,
                    "StoneTileFloorTilemap")},
    };

    public static Item MakeItemCopy(string itemName) {
        Item item;
        ItemRes.ItemDict.TryGetValue(itemName, out item);

        if (item == null)
            Debug.Log("Could not find item");

        if (item is Tool) {
            return new Tool(itemName);
        } else if (item is BuildableItem) {
            return new BuildableItem(itemName);
        }
        return new Item(itemName);
    }

}
