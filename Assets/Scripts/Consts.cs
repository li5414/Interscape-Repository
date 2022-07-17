using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class is for storing constant variables
public static class Consts {
    // length of a chunk in number of tiles
    public static int CHUNK_SIZE = 16;
    public static int CHUNK_SIZE_SQUARED = 16 * 16;

    // length of the map in number of tiles
    public static int MAP_DIMENSION = 5000;

    public static float BEACH_HEIGHT = -0.27f;
    public static float WATER_HEIGHT = -0.3f;

    // table used to convert temp and humidity into biomes
    public static BiomeType[,] BIOME_TYPE_TABLE = {   
    //                                               <--Colder      Hotter -->            
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert},   // Dryest
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert},
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Desert,     BiomeType.Desert },
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,      BiomeType.Grassland,          BiomeType.Savanna,    BiomeType.Savanna,    BiomeType.Desert },
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Grassland,  BiomeType.Savanna,    BiomeType.Desert },
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },  // Wettest
	{ BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert },
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Taiga,  BiomeType.Taiga,          BiomeType.SeasonalForest,     BiomeType.Rainforest, BiomeType.Savanna,    BiomeType.Desert }
    };

    public static Dictionary<BiomeType, Color32> BIOME_COLOUR_DICT = new Dictionary<BiomeType, Color32>{
        {BiomeType.Water, new Color32(116, 144, 183, 255)},
        {BiomeType.DeepWater, new Color32(88, 115, 159, 255)},
        {BiomeType.Beach, new Color32(229, 209, 168, 255)},
        {BiomeType.Desert, new Color32 (229, 204, 159, 255)},
        {BiomeType.Savanna, new Color32(246, 226, 176, 255)},
        {BiomeType.Rainforest, new Color32 (69, 163, 117, 255)},
        {BiomeType.Grassland, new Color32(185, 205, 147, 255)},
        {BiomeType.SeasonalForest, new Color32 (130, 181, 146, 255)},
        {BiomeType.Taiga, new Color32(126, 166, 142, 255)},
        {BiomeType.Tundra, new Color32(144, 179, 164, 255)},
        {BiomeType.Ice, new Color32 (218, 231, 235, 255)}
    };

    // lookup index of 16x16 2D array condensed to 1D array
    public static int IndexOf(int x, int y) {
        return (x + 16 * y);
    }
}

public enum BiomeType {
    Desert,
    Savanna,
    Rainforest,
    Grassland,
    SeasonalForest,
    Taiga,
    Tundra,
    Ice,
    Water,
    DeepWater,
    Beach
}
