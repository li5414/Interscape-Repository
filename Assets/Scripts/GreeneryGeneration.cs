using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GreeneryGeneration : MonoBehaviour {

    int chunkSize = Consts.CHUNK_SIZE;
    float offsetX;
    float offsetY;
    public GameObject treeParent;
    WorldSettings worldSettings;

    public Tilemap pathTilemap;

    private void Start() {
        worldSettings = gameObject.GetComponent<WorldSettings>();
        offsetX = worldSettings.PRNG.Next(-10000, 10000);
        offsetY = worldSettings.PRNG.Next(-10000, 10000);

        for (int i = 0; i < NatureResources.objects.Length; i++) {
            if (NatureResources.objects[i] == null) {
                Debug.Log("object " + i + " is null");
            }
        }
    }

    public bool IsEmpty(int x, int y) {
        // TODO figure out if z is important here
        Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 20);
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, -Vector2.zero);

        if (hits.Length > 0)
            return false;

        if (pathTilemap.GetTile(pathTilemap.WorldToCell(pos)) != null) {
            return false;
        }
        return true;
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

                if (!IsEmpty((int)pos.x, (int)pos.y))
                    continue;

                isTreeBiome = true;
                biome = biomes[x, y];

                //if (heights [x, y] < -0.26)
                //	biome = BiomeCalculations.BiomeType.Water;

                // choose what kind of trees/things to spawn depending on biome-----------------------------------
                switch (biome) {
                    case BiomeType.Grassland:
                        treeChance = 0.005f;
                        tree1 = NatureResources.treeRed;
                        tree2 = NatureResources.treeBlue;
                        tree3 = NatureResources.treeYellow;
                        tree4 = NatureResources.treeOak;
                        shrub1 = NatureResources.cane;
                        shrub2 = NatureResources.wheat;
                        shrub3 = NatureResources.dead_bush;
                        shrub4 = NatureResources.rock2;
                        break;
                    case BiomeType.Savanna:
                        treeChance = 0.02f;
                        tree1 = NatureResources.treeRed;
                        tree2 = NatureResources.treeYellow;
                        tree3 = NatureResources.treeJoshua;
                        tree4 = NatureResources.treeYellow;
                        shrub1 = NatureResources.dead_bush;
                        shrub2 = NatureResources.rock2;
                        shrub3 = NatureResources.rock1;
                        shrub4 = NatureResources.wheat;
                        break;
                    case BiomeType.Taiga:
                        treeChance = 0.15f;
                        tree1 = NatureResources.treePine;
                        tree2 = NatureResources.treeBirch;
                        tree3 = NatureResources.treePineSmall;
                        tree4 = NatureResources.treeWhite;
                        shrub1 = NatureResources.bush2;
                        shrub2 = NatureResources.stick;
                        shrub3 = NatureResources.rock2;
                        shrub4 = null;
                        break;
                    case BiomeType.SeasonalForest:
                        treeChance = 0.15f;
                        tree1 = NatureResources.treeForest1;
                        tree2 = NatureResources.treeForest2;
                        tree3 = NatureResources.treePine;
                        tree4 = NatureResources.treeOak;
                        shrub1 = NatureResources.bush1;
                        shrub2 = NatureResources.bush2;
                        shrub3 = NatureResources.rock2;
                        shrub4 = null;
                        break;
                    case BiomeType.Rainforest:
                        treeChance = 0.25f;
                        tree1 = NatureResources.treeRainforest1;
                        tree2 = NatureResources.treePalm;
                        tree3 = NatureResources.treeForest1;
                        tree4 = NatureResources.treeFig;
                        shrub1 = NatureResources.cane;
                        shrub2 = NatureResources.fern1;
                        shrub3 = null;
                        shrub4 = null;
                        break;
                    case BiomeType.Tundra:
                        isTreeBiome = false;
                        tree1 = null;
                        tree2 = null;
                        tree3 = null;
                        tree4 = null;
                        shrub1 = NatureResources.dead_bush;
                        shrub2 = NatureResources.bush2;
                        shrub3 = NatureResources.rock2;
                        shrub4 = NatureResources.rock1;
                        break;
                    case BiomeType.Desert:
                        isTreeBiome = false;
                        tree1 = null;
                        tree2 = null;
                        tree3 = null;
                        tree4 = null;
                        shrub1 = NatureResources.cactus;
                        shrub2 = NatureResources.rock2;
                        shrub3 = NatureResources.rock1;
                        shrub4 = NatureResources.dead_bush;
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
                // end of choice depending on biome--------------------------------------------------

                // generate noise values
                perlinNoise = Mathf.PerlinNoise((chunkPos.x + x + offsetX / 3.5f) * 0.1f,
                    (chunkPos.y + y + offsetY / 3.5f) * 0.1f);


                // spawn in trees
                if (isTreeBiome == true) { // check nearby coordinates for trees ???
                    treeChance *= perlinNoise; // causes trees to generate in 'clusters'
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