using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantsGenerator : MonoBehaviour {

    int chunkSize = Consts.CHUNK_SIZE;
    float offsetX;
    float offsetY;
    public GameObject treeParent;
    WorldSettings worldSettings;
    NatureResources natureResources;

    private void Start() {
        worldSettings = gameObject.GetComponent<WorldSettings>();
        natureResources = gameObject.GetComponent<NatureResources>();

        offsetX = worldSettings.PRNG.Next(-10000, 10000);
        offsetY = worldSettings.PRNG.Next(-10000, 10000);
    }

    public GameObject[,] GeneratePlants(Vector2Int chunkPos,
        BiomeType[,] biomes, float[,] heights, GameObject parent) {
        GameObject[,] entities = new GameObject[chunkSize, chunkSize];
        float perlinNoise;
        BiomeType biome;
        GameObject tree1;
        GameObject tree2;
        GameObject tree3;
        GameObject tree4;
        GameObject shrub1; // most common
        GameObject shrub2;
        GameObject shrub3;
        GameObject shrub4; // least common
        bool isTreeBiome;
        double randNum;
        float treeChance = 0;

        // for each tile choose whether to spawn shrub/tree/nothing
        for (int x = 1; x < chunkSize; x++) {
            for (int y = 1; y < chunkSize - 1; y++) {
                Vector3 pos = new Vector3(chunkPos.x + x + 0.5f, chunkPos.y + y + 0.5f, 198);

                isTreeBiome = true;
                biome = biomes[x, y];

                // choose what kind of trees/things to spawn depending on biome
                switch (biome) {
                    case BiomeType.Grassland:
                        treeChance = 0.005f;
                        tree1 = natureResources.treeRed;
                        tree2 = natureResources.treeBlue;
                        tree3 = natureResources.treeYellow;
                        tree4 = natureResources.treeOak;
                        shrub1 = natureResources.cane;
                        shrub2 = natureResources.wheat;
                        shrub3 = natureResources.dead_bush;
                        shrub4 = natureResources.rock2;
                        break;
                    case BiomeType.Savanna:
                        treeChance = 0.02f;
                        tree1 = natureResources.treeRed;
                        tree2 = natureResources.treeYellow;
                        tree3 = natureResources.treeJoshua;
                        tree4 = natureResources.treeYellow;
                        shrub1 = natureResources.dead_bush;
                        shrub2 = natureResources.rock2;
                        shrub3 = natureResources.rock1;
                        shrub4 = natureResources.wheat;
                        break;
                    case BiomeType.Taiga:
                        treeChance = 0.15f;
                        tree1 = natureResources.treePine;
                        tree2 = natureResources.treeBirch;
                        tree3 = natureResources.treePineSmall;
                        tree4 = natureResources.treeWhite;
                        shrub1 = natureResources.bush2;
                        shrub2 = natureResources.stick;
                        shrub3 = natureResources.rock2;
                        shrub4 = null;
                        break;
                    case BiomeType.SeasonalForest:
                        treeChance = 0.15f;
                        tree1 = natureResources.treeForest1;
                        tree2 = natureResources.treeForest2;
                        tree3 = natureResources.treePine;
                        tree4 = natureResources.treeOak;
                        shrub1 = natureResources.bush1;
                        shrub2 = natureResources.bush2;
                        shrub3 = natureResources.rock2;
                        shrub4 = null;
                        break;
                    case BiomeType.Rainforest:
                        treeChance = 0.25f;
                        tree1 = natureResources.treeRainforest1;
                        tree2 = natureResources.treePalm;
                        tree3 = natureResources.treeForest1;
                        tree4 = natureResources.treeFig;
                        shrub1 = natureResources.cane;
                        shrub2 = natureResources.fern1;
                        shrub3 = null;
                        shrub4 = null;
                        break;
                    case BiomeType.Tundra:
                        isTreeBiome = false;
                        tree1 = null;
                        tree2 = null;
                        tree3 = null;
                        tree4 = null;
                        shrub1 = natureResources.dead_bush;
                        shrub2 = natureResources.bush2;
                        shrub3 = natureResources.rock2;
                        shrub4 = natureResources.rock1;
                        break;
                    case BiomeType.Desert:
                        isTreeBiome = false;
                        tree1 = null;
                        tree2 = null;
                        tree3 = null;
                        tree4 = null;
                        shrub1 = natureResources.cactus;
                        shrub2 = natureResources.rock2;
                        shrub3 = natureResources.rock1;
                        shrub4 = natureResources.dead_bush;
                        break;
                    default:
                        isTreeBiome = false;
                        tree1 = null;
                        tree2 = null;
                        tree3 = null;
                        tree4 = null;
                        shrub1 = null;
                        shrub2 = null;
                        shrub3 = null;
                        shrub4 = null;
                        break;
                }

                // generate noise values
                perlinNoise = Mathf.PerlinNoise((chunkPos.x + x + offsetX / 3.5f) * 0.1f,
                    (chunkPos.y + y + offsetY / 3.5f) * 0.1f);


                // spawn in trees
                // TODO check nearby coordinates for trees ???
                if (isTreeBiome == true) {
                    // causes trees to generate in 'clusters'
                    treeChance *= perlinNoise;
                    randNum = worldSettings.PRNG.NextDouble();

                    if (worldSettings.PRNG.NextDouble() < treeChance) {
                        if (randNum < 0.01f) {
                            entities[x, y] = Instantiate(tree4, pos, Quaternion.identity); // rarer tree
                        } else if (randNum < 0.34f) {
                            entities[x, y] = Instantiate(tree1, pos, Quaternion.identity);
                        } else if (randNum > 0.66f) {
                            entities[x, y] = Instantiate(tree2, pos, Quaternion.identity);
                        } else {
                            entities[x, y] = Instantiate(tree3, pos, Quaternion.identity);
                        }
                        entities[x, y].transform.SetParent(parent.transform, true);
                    }
                }

                // spawn in shrubs
                randNum = worldSettings.PRNG.NextDouble();
                if (worldSettings.PRNG.NextDouble() < (0.15f * Mathf.InverseLerp(0.5f, 0.9f, perlinNoise)) && entities[x, y] == null) {
                    if (randNum < 0.05f && shrub4 != null) {
                        entities[x, y] = Instantiate(shrub4, pos, Quaternion.identity); // rarer tree
                        entities[x, y].transform.SetParent(parent.transform, true);
                    } else if (randNum < 0.3f && shrub3 != null) {
                        entities[x, y] = Instantiate(shrub3, pos, Quaternion.identity);
                        entities[x, y].transform.SetParent(parent.transform, true);
                    } else if (randNum < 0.6f && shrub2 != null) {
                        entities[x, y] = Instantiate(shrub2, pos, Quaternion.identity);
                        entities[x, y].transform.SetParent(parent.transform, true);
                    } else if (shrub1 != null) {
                        entities[x, y] = Instantiate(shrub1, pos, Quaternion.identity);
                        entities[x, y].transform.SetParent(parent.transform, true);
                    }

                }

            }
        }
        return entities;
    }
}