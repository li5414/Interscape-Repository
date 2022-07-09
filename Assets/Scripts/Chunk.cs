using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using System;
using System.IO;

public class Chunk : MonoBehaviour {
    public Vector2Int chunkPos;
    public Vector2Int chunkCoord;
    float[,] heights;
    public ChunkData chunkData;

    // can generate from chunk data file
    GameObject[,] objects;
    RuleTile[] sandTiles = new RuleTile[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];
    bool containsWater;
    bool containsSand;
    bool containsGrass;
    bool hasGeneratedWaterMaterial;
    bool hasGeneratedTerrainMaterial;
    
    // don't need to read chunk data file to generate
    public ChunkStatus status;
    Vector3Int[] tilePositionsWorld;
    float[,] temps;
    float[,] humidities;
    BiomeType[,] biomes;

    public Texture2D terrainColours;

    // References assigned in prefab
    public GameObject terrainChunk;
    public GameObject grassBlades; // TODO change grass sprite
    public GameObject waterChunk;
    public GameObject treeParent;
    
    private Vector2Int closestVillage;

    // TODO have a prng for each chunk?

    // reference other scripts
    static ChunkManager chunkManager;
    static TerrainDataGenerator terrainData;
    static PlantsGenerator plantsGenerator;
    static WorldSettings worldSettings;
    static BuildingResources buildingResources;
    static TileResources tileResources;

    void Start() {
        findScriptReferences();
        chunkPos = new Vector2Int(
            (int)transform.position.x, 
            (int)transform.position.y);
        chunkCoord = ChunkManager.GetChunkCoord(chunkPos);

        chunkData = new ChunkData(chunkCoord, terrainData);
        status = ChunkStatus.NOT_GENERATED;
    }

    void findScriptReferences() {
        if (chunkManager == null) {
            chunkManager = GameObject.FindWithTag("GameManager").GetComponent<ChunkManager>();
            terrainData = GameObject.FindWithTag("GameManager").GetComponent<TerrainDataGenerator>();
            plantsGenerator = GameObject.FindWithTag("GameManager").GetComponent<PlantsGenerator>();
            worldSettings = GameObject.FindWithTag("GameManager").GetComponent<WorldSettings>();
            buildingResources = GameObject.FindWithTag("GameManager").GetComponent<BuildingResources>();
            tileResources = GameObject.FindWithTag("GameManager").GetComponent<TileResources>();
        }
    }

    public void GenerateChunkData() {
        heights = terrainData.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
        temps = terrainData.GetTemperaturesExtended(chunkPos.x, chunkPos.y, heights);
        humidities = terrainData.GetHumidityValuesExtended(chunkPos.x, chunkPos.y, heights);
        biomes = terrainData.GetBiomesExtended(heights, temps, humidities);

        // initalise arrays to be be used for settiles()
        tilePositionsWorld = new Vector3Int[Consts.CHUNK_SIZE * Consts.CHUNK_SIZE];

        // creates and sets tiles in tilearray to positions in position array
        GenerateTilesChunked();

        // generate the grass colour texture for that chunk
        GenerateExtendedColourTexture();

        // array of gameobjects (use dict/list instead?)
        objects = plantsGenerator.GeneratePlants(chunkPos, biomes, heights, treeParent);

        // potentially generate a village on this chunk
        handleVillageGeneration();

        treeParent.SetActive(false);
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
        // TODO figure out a way to check if village lands on a chunk that is not so inefficient lol
        for (int i = -2; i <= 2; i++) {
            for (int j = -2; j <= 2; j++) {
                if ((chunkCoord.x + i) % 8 == 0 && (chunkCoord.y + j) % 8 == 0) {
                    int chunkPosX = (chunkCoord.x + i) * Consts.CHUNK_SIZE;
                    int chunkPosY = (chunkCoord.y + j) * Consts.CHUNK_SIZE;

                    // stop villages generating on water
                    if (terrainData.GetHeightValue(
                        chunkPosX + (int)(Consts.CHUNK_SIZE / 2),
                        chunkPosY + (int)(Consts.CHUNK_SIZE / 2))
                        < Consts.BEACH_HEIGHT + 0.2)
                        return false;

                    // TODO optimise using hash function instead of prng
                    System.Random tempPrng = new System.Random(chunkPosX + chunkPosY + worldSettings.SEED);

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
                temp *= terrainData.biomeColourMap.width;

                float humidity = humidities[i, j];
                humidity = 1 - humidity;
                humidity *= terrainData.biomeColourMap.width;
                Color color = terrainData.biomeColourMap.GetPixel((int)temp, (int)humidity);

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
                temp *= terrainData.biomeColourMap.width;

                float humidity = humidities[i, j];
                humidity = 1 - humidity;
                humidity *= terrainData.biomeColourMap.width;
                Color color = terrainData.biomeColourMap.GetPixel((int)temp, (int)humidity);

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
                    sandTiles[at(i, j)] = tileResources.tileSandRule;
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

    public void LoadChunk() {
        // set parent gameobject for the plants
        treeParent.SetActive(true);

        if (containsSand)
            buildingResources.sandTilemap.SetTiles(tilePositionsWorld, sandTiles);

        if (containsGrass) {
            if (!hasGeneratedTerrainMaterial) {
                // generate a new material and apply it to the chunk
                Material newMat = new Material(Shader.Find("Unlit/ChunkTerrainShader"));
                newMat.CopyPropertiesFromMaterial(terrainData.chunkTerrainMaterial);
                terrainChunk.GetComponent<SpriteRenderer>().material = newMat;
                terrainChunk.GetComponent<SpriteRenderer>().material.SetTexture("_TileColours", terrainColours);

                // generate a new material for the grass blades in the chunk
                SpriteRenderer grassBladeRenderer = grassBlades.GetComponent<SpriteRenderer>();
                
                newMat = new Material(Shader.Find("Unlit/ChunkTerrainShader"));
                newMat.CopyPropertiesFromMaterial(terrainData.chunkGrassMaterial);
                grassBladeRenderer.material = newMat;
                grassBladeRenderer.material.SetTexture("_TileColours", terrainColours);

                int randNum = worldSettings.PRNG.Next(0, 7);
                grassBladeRenderer.sprite = tileResources.grassDetailsChunk[randNum];
                hasGeneratedTerrainMaterial = true;
            } else {
                terrainChunk.SetActive(true);
            }
        } else {
            terrainChunk.SetActive(false);
            grassBlades.SetActive(false);
        }

        if (containsWater) {
            if (!hasGeneratedWaterMaterial) {
                // generate a new material for the water in the chunk
                Material newWaterMaterial = new Material(Shader.Find("Unlit/ChunkWaterShader"));
                newWaterMaterial.CopyPropertiesFromMaterial(terrainData.chunkWaterMaterial);
                waterChunk.GetComponent<SpriteRenderer>().material = newWaterMaterial;
                waterChunk.GetComponent<SpriteRenderer>().material.SetTexture("_TileColours", terrainColours);
                hasGeneratedWaterMaterial = true;
            } else {
                waterChunk.SetActive(true);
            }
        } else {
            waterChunk.SetActive(false);
        }
    }

    public void UnloadChunk() {
        // disable things
        treeParent.SetActive(false);
        terrainChunk.SetActive(false);
        grassBlades.SetActive(false);
        waterChunk.SetActive(false);

        // unload sand if there is some
        if (containsSand) {
            for (int i = 0; i < tilePositionsWorld.Length; i++) {
                buildingResources.sandTilemap.SetTile(tilePositionsWorld[i], null);
            }
        }
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

public enum ChunkStatus {
    NOT_GENERATED,
    LOADED,
    UNLOADED,
}


