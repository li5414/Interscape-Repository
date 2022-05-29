using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using System.Threading;
using System;
using System.IO;

public class Chunk {
    // components
    public Vector2Int chunkPos;
    public Vector2Int chunkCoord;

    bool isLoaded;
    bool isGenerated;
    bool containsWater;
    bool containsSand;
    bool containsGrass;

    // parent for this particular chunk
    GameObject treeParent;

    // position arrays
    Vector3Int[] tilePositionsWorld;

    // tile arrays
    public RuleTile[] sandTileArray = new RuleTile[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];
    public Tile[] waterTileArray = new Tile[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];

    // tile to store the grass blade details (the size of the tile takes up the whole chunk)
    public Tile deetChunk;

    // biome information
    public Texture2D terrainColours;
    float[,] heights;
    float[,] temps;
    float[,] humidities;
    GameObject[,] entities;
    BiomeType[,] biomes;
    GameObject terrainChunk;
    GameObject waterChunk;

    // reference other scripts
    static ChunkManager chunkManager = GameObject.FindWithTag("SystemPlaceholder").GetComponent<ChunkManager>();
    static BiomeCalculations bCalc = GameObject.FindWithTag("SystemPlaceholder").GetComponent<BiomeCalculations>();
    static GreeneryGeneration gen = GameObject.FindWithTag("SystemPlaceholder").GetComponent<GreeneryGeneration>();
    static WorldSettings worldSettings = GameObject.FindWithTag("SystemPlaceholder").GetComponent<WorldSettings>();

    static GameObject TreeParent = GameObject.Find("TreeParent");

    private Vector2Int closestVillage;

    public Chunk(Vector2Int pos) {
        // get/initialise some important things
        chunkPos = new Vector2Int(pos.x * Consts.CHUNK_SIZE, pos.y * Consts.CHUNK_SIZE);
        chunkCoord = new Vector2Int(pos.x, pos.y);
        treeParent = new GameObject();
        treeParent.transform.SetParent(TreeParent.gameObject.transform);
        // GenerateChunkData();
    }

    public void GenerateChunkData() {
        heights = bCalc.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
        temps = bCalc.GetTemperaturesExtended(chunkPos.x, chunkPos.y, heights);
        humidities = bCalc.GetHumidityValuesExtended(chunkPos.x, chunkPos.y, heights);
        biomes = bCalc.GetBiomesExtended(heights, temps, humidities);

        // initalise arrays to be be used for settiles()
        tilePositionsWorld = new Vector3Int[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];

        // creates and sets tiles in tilearray to positions in position array
        GenerateTilesChunked();

        // generate the grass colour texture for that chunk
        GenerateExtendedColourTexture();

        // generate grass details
        GenerateDetailsChunk();

        // potentially generate a village on this chunk
        handleVillageGeneration();

        // array of gameobjects (use dict/list instead?)
        entities = gen.GeneratePlants(chunkPos, biomes, heights, treeParent);

        // load in the chunk
        isGenerated = true;
        chunkManager.chunksToLoad.Enqueue(this);
    }

    private void handleVillageGeneration() {
        if (isCloseToVillage()) {
            VillageGenerator village;
            if (chunkManager.newlyGeneratedVillages.TryGetValue(closestVillage, out village)) {
                village.PlaceVillageChunkInWorld(chunkPos);
            } else {
                Vector2Int villagePos = new Vector2Int(closestVillage.x * Consts.CHUNK_SIZE, closestVillage.y * Consts.CHUNK_SIZE);
                village = VillageGenerator.SpawnVillage(villagePos);
                chunkManager.newlyGeneratedVillages.Add(closestVillage, village);
                village.PlaceVillageChunkInWorld(chunkPos);
            }
        }
    }

    private bool isCloseToVillage() {
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if ((chunkCoord.x + i) % 6 == 0 || (chunkCoord.y + j) % 6 == 0) {
                    // // TODO fix villages generating on water
                    // if (heights[0, 0] < Consts.BEACH_HEIGHT + offset)
                    //     return false;
                    // if (heights[Consts.CHUNK_SIZE - 1, Consts.CHUNK_SIZE - 1] < Consts.BEACH_HEIGHT + offset)
                    //     return false;
                    // if (heights[Consts.CHUNK_SIZE - 1, 0] < Consts.BEACH_HEIGHT + offset)
                    //     return false;
                    // if (heights[0, Consts.CHUNK_SIZE - 1] < Consts.BEACH_HEIGHT + offset)
                    //     return false;

                    // TODO optimise using hash function instead of prng
                    System.Random tempPrng = new System.Random((chunkCoord.x + i) * Consts.CHUNK_SIZE + (chunkCoord.y + j) * Consts.CHUNK_SIZE + worldSettings.SEED);

                    if (tempPrng.NextDouble() < 0.2) {
                        closestVillage = new Vector2Int(chunkCoord.x + i, chunkCoord.y + j);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void GenerateColourTexture() {
        terrainColours = new Texture2D(Consts.CHUNK_SIZE, Consts.CHUNK_SIZE);
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                float height = heights[i, j];

                float temp = temps[i, j];
                temp = Mathf.InverseLerp(-80f, 80f, temp);
                temp *= bCalc.biomeColourMap.width;

                float humidity = humidities[i, j];
                humidity = 1 - humidity;
                humidity *= bCalc.biomeColourMap.width;
                Color color = bCalc.biomeColourMap.GetPixel((int)temp, (int)humidity);

                if (height < -0.29f) {
                    color = Consts.BIOME_COLOUR_DICT[BiomeType.Beach];
                }
                color.a = Mathf.Clamp01(Mathf.InverseLerp(-1f, 1f, height)); // lower alpha is deeper
                terrainColours.SetPixel(i, j, color);
            }
        }
        terrainColours.Apply();

        // get rid of lines between textures :D
        terrainColours.wrapMode = TextureWrapMode.Clamp;
    }

    // the extended texture includes overlaps between chunks by 1 tile
    public void GenerateExtendedColourTexture() {
        int newWidth = Consts.CHUNK_SIZE + 2;
        Texture2D extendedTerrainColours = new Texture2D(newWidth, newWidth);
        int i = 0;
        int j = 0;

        for (i = 0; i < newWidth; i++) {
            for (j = 0; j < newWidth; j++) {
                float height = heights[i, j];

                float temp = temps[i, j];
                temp = Mathf.InverseLerp(-80f, 80f, temp);
                temp *= bCalc.biomeColourMap.width;

                float humidity = humidities[i, j];
                humidity = 1 - humidity;
                humidity *= bCalc.biomeColourMap.width;
                Color color = bCalc.biomeColourMap.GetPixel((int)temp, (int)humidity);

                // if (height < -0.29f) {
                // 	color = Consts.BIOME_COLOUR_DICT [BiomeType.Beach];
                // }
                // set alpha - lower alpha is deeper
                color.a = Mathf.Clamp01(Mathf.InverseLerp(-1f, 1f, height));
                extendedTerrainColours.SetPixel(i, j, color);
            }
        }

        extendedTerrainColours.Apply();
        extendedTerrainColours.wrapMode = TextureWrapMode.Clamp;
        terrainColours = extendedTerrainColours;
    }

    public void GenerateTilesChunked() {
        float heightVal;

        // in loop, enter all positions and tiles in arrays
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {

                // fill in tile position arrays
                tilePositionsWorld[at(i, j)] = new Vector3Int(i + chunkPos.x, j + chunkPos.y, 0);

                // get features for this tile
                heightVal = heights[i, j];

                // sand layer
                if (heightVal < Consts.BEACH_HEIGHT) {
                    sandTileArray[at(i, j)] = ChunkManager.tileResources.tileSandRule;
                    containsSand = true;
                }

                // grass layer
                if (heightVal >= Consts.WATER_HEIGHT - 0.1f) {
                    containsGrass = true;
                }

                // water layer
                // we want some overlapping
                if (heightVal < Consts.WATER_HEIGHT - 0.04f) {
                    containsWater = true;
                }
            }
        }
    }

    public void GenerateDetailsChunk() {
        int randNum = worldSettings.PRNG.Next(0, 7);
        deetChunk = ChunkManager.tileResources.grassDetailsChunk[randNum];
    }

    /*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    public void LoadChunk() {
        if (isLoaded)
            return;
        // the chunk must be generated in order to load
        Assert.IsTrue(isGenerated);

        // set parent gameobject for the plants
        treeParent.SetActive(true);

        if (containsSand)
            chunkManager.sandTilemap.SetTiles(tilePositionsWorld, sandTileArray);

        if (containsGrass) {
            if (!terrainChunk) {
                terrainChunk = UnityEngine.Object.Instantiate(bCalc.chunkTerrainPrefab, new Vector3Int(chunkPos.x + Consts.CHUNK_SIZE / 2, chunkPos.y + Consts.CHUNK_SIZE / 2, 199), Quaternion.identity);
                terrainChunk.transform.SetParent(bCalc.chunkTerrainParent.gameObject.transform);

                // generate a new material and apply it to the chunk
                Material newMat = new Material(Shader.Find("Unlit/ChunkTerrainShader"));
                newMat.CopyPropertiesFromMaterial(bCalc.chunkTerrainMaterial);
                terrainChunk.GetComponent<SpriteRenderer>().material = newMat;
                terrainChunk.GetComponent<SpriteRenderer>().material.SetTexture("_TileColours", terrainColours);

                // generate a new material for the grass blades in the chunk
                newMat = new Material(Shader.Find("Unlit/ChunkTerrainShader"));
                newMat.CopyPropertiesFromMaterial(bCalc.chunkGrassMaterial);
                terrainChunk.GetComponentsInChildren<SpriteRenderer>()[1].material = newMat;
                terrainChunk.GetComponentsInChildren<SpriteRenderer>()[1].material.SetTexture("_TileColours", terrainColours);
            } else {
                terrainChunk.SetActive(true);
            }
        }

        if (containsWater) {
            if (!waterChunk) {
                // generate a new material for the water in the chunk
                waterChunk = UnityEngine.Object.Instantiate(bCalc.chunkWaterPrefab, new Vector3Int(chunkPos.x + Consts.CHUNK_SIZE / 2, chunkPos.y + Consts.CHUNK_SIZE / 2, 198), Quaternion.identity);
                waterChunk.transform.SetParent(bCalc.chunkWaterParent.gameObject.transform);

                // generate a new material and apply it to the chunk
                Material newWaterMaterial = new Material(Shader.Find("Unlit/ChunkWaterShader"));
                newWaterMaterial.CopyPropertiesFromMaterial(bCalc.chunkWaterMaterial);
                waterChunk.GetComponent<SpriteRenderer>().material = newWaterMaterial;
                waterChunk.GetComponent<SpriteRenderer>().material.SetTexture("_TileColours", terrainColours);
            } else {
                waterChunk.SetActive(true);
            }
        }
        isLoaded = true;
    }

    public void UnloadChunk() {
        if (!isLoaded)
            return;
        Assert.IsTrue(isGenerated);

        // disable things
        treeParent.SetActive(false);

        // unload sand if there is some
        if (containsSand) {
            for (int i = 0; i < tilePositionsWorld.Length; i++) {
                chunkManager.sandTilemap.SetTile(tilePositionsWorld[i], null);
            }
        }

        // unload grass if there is some
        if (containsGrass) {
            terrainChunk.SetActive(false);
        }

        // unload water if there is some
        if (containsWater) {
            waterChunk.SetActive(false);
        }
        isLoaded = false;
    }

    public bool IsGenerated() {
        return isGenerated;
    }

    public bool IsLoaded() {
        return isLoaded;
    }

    // lookup index of 16x16 2D array condensed to 1D array
    public int at(int x, int y) {
        return (x * 16 + y);
    }

    // lookup index of 64x64 2D array condensed to 1D array
    public int at64(int x, int y) {
        return (x * 32 + y);
    }
}

