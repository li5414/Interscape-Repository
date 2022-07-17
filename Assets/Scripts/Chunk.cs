using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using System;
using System.IO;

public class Chunk : MonoBehaviour, IDataPersistence {
    public Vector2Int chunkPos;
    public Vector2Int chunkCoord;

    public GameObject[,] objects { get; private set; }
    public TileBase[] sandTiles { get; private set; }
    public TileBase[] wallTiles { get; private set; }
    public RuleTile[] pathTiles { get; private set; }
    public TileBase[][] floorTileData { get; private set; }

    bool containsWater;
    bool containsGrass;
    bool hasGeneratedWaterMaterial;
    bool hasGeneratedTerrainMaterial;

    public ChunkStatus status;
    float[,] heights;
    float[,] temps;
    float[,] humidities;
    BiomeType[,] biomes;

    public Texture2D terrainColours;

    // References assigned in prefab
    public GameObject terrainChunk;
    public GameObject grassBlades;
    public GameObject waterChunk;
    public GameObject treeParent;

    private Vector2Int closestVillage;
    BoundsInt chunkBounds;

    // TODO have a prng for each chunk?

    // reference other scripts
    static ChunkManager chunkManager;
    static TerrainDataGenerator terrainData;
    static PlantsGenerator plantsGenerator;
    static WorldSettings worldSettings;
    public static BuildingResources buildingResources { get; private set; }
    static TileResources tileResources;
    static NatureResources natureResources;

    void Start() {
        findScriptReferences();
        chunkPos = new Vector2Int(
            (int)transform.position.x,
            (int)transform.position.y);
        chunkCoord = ChunkManager.GetChunkCoord(chunkPos);
        chunkBounds = new BoundsInt(chunkPos.x, chunkPos.y, 0, Consts.CHUNK_SIZE, Consts.CHUNK_SIZE, 1);
        floorTileData = new TileBase[buildingResources.getAllFloorTilemaps().Count][];

        status = ChunkStatus.NOT_GENERATED;
    }

    void findScriptReferences() {
        if (chunkManager == null) {
            GameObject gameManager = GameObject.FindWithTag("GameManager");
            chunkManager = gameManager.GetComponent<ChunkManager>();
            terrainData = gameManager.GetComponent<TerrainDataGenerator>();
            plantsGenerator = gameManager.GetComponent<PlantsGenerator>();
            worldSettings = gameManager.GetComponent<WorldSettings>();
            buildingResources = gameManager.GetComponent<BuildingResources>();
            tileResources = gameManager.GetComponent<TileResources>();
            natureResources = gameManager.GetComponent<NatureResources>();
        }
    }

    public void GenerateChunk() {
        heights = terrainData.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
        temps = terrainData.GetTemperaturesExtended(chunkPos.x, chunkPos.y, heights);
        humidities = terrainData.GetHumidityValuesExtended(chunkPos.x, chunkPos.y, heights);
        biomes = terrainData.GetBiomesExtended(heights, temps, humidities);

        refreshContainFlags();
        terrainColours = terrainData.GenerateExtendedColourTexture(heights, temps, humidities);

        // generate array of gameobjects (TODO optimise: use dict/list instead?)
        objects = plantsGenerator.GeneratePlants(chunkPos, biomes, heights, treeParent);

        sandTiles = terrainData.GenerateSandTiles(heights, tileResources.tileSandRule);
        VillageGenerator.TryGenerateVillage(chunkCoord, chunkPos, terrainData, chunkManager, worldSettings.SEED);

        treeParent.SetActive(false);
        chunkManager.chunksToLoad.Enqueue(this);
    }

    public void GenerateChunkFromFile(ChunkData chunkData) {
        heights = terrainData.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
        temps = terrainData.GetTemperaturesExtended(chunkPos.x, chunkPos.y, heights);
        humidities = terrainData.GetHumidityValuesExtended(chunkPos.x, chunkPos.y, heights);
        biomes = terrainData.GetBiomesExtended(heights, temps, humidities);

        refreshContainFlags();
        terrainColours = terrainData.GenerateExtendedColourTexture(heights, temps, humidities);

        generateObjectsFromChunkData(chunkData);

        sandTiles = ChunkData.GenerateTiles(chunkData.sandTiles, tileResources.tileSandRule);
        wallTiles = ChunkData.GenerateTiles(chunkData.wallTiles, buildingResources.idToRuleTile);

        // get floor tiles
        for (int floor = 0; floor < chunkData.floorTiles.GetAll().Count; floor++) {
            bool[] floorTileBools = chunkData.floorTiles.GetAll()[floor];
            floorTileData[floor] = ChunkData.GenerateTiles(floorTileBools, buildingResources.floorBaseTile);
        }

        treeParent.SetActive(false);
        chunkManager.chunksToLoad.Enqueue(this);
    }

    public void LoadChunk() {
        treeParent.SetActive(true);

        if (sandTiles != null)
            buildingResources.sandTilemap.SetTilesBlock(chunkBounds, sandTiles);
        if (wallTiles != null)
            buildingResources.wallTilemap.SetTilesBlock(chunkBounds, wallTiles);
        for (int floor = 0; floor < floorTileData.Length; floor++) {
            Tilemap floorTilemap = buildingResources.getAllFloorTilemaps()[floor];
            if (floorTileData[floor] != null) {
                floorTilemap.SetTilesBlock(chunkBounds, floorTileData[floor]);
            }
        }

        if (containsGrass) {
            if (!hasGeneratedTerrainMaterial) {
                generateTerrainMaterials();
            } else {
                terrainChunk.SetActive(true);
                grassBlades.SetActive(true);
            }
        } else {
            terrainChunk.SetActive(false);
            grassBlades.SetActive(false);
        }

        if (containsWater) {
            if (!hasGeneratedWaterMaterial) {
                generateWaterMaterials();
            } else {
                waterChunk.SetActive(true);
            }
        } else {
            waterChunk.SetActive(false);
        }
    }

    public void UnloadChunk() {
        treeParent.SetActive(false);
        terrainChunk.SetActive(false);
        grassBlades.SetActive(false);
        waterChunk.SetActive(false);

        updateTileArrays();
        TileBase[] nullTileArray = new TileBase[Consts.CHUNK_SIZE_SQUARED];

        // unload sand
        if (sandTiles != null) {
            buildingResources.sandTilemap.SetTilesBlock(chunkBounds, nullTileArray);
        }

        // unload walls
        if (wallTiles != null)
            buildingResources.wallTilemap.SetTilesBlock(chunkBounds, nullTileArray);

        // unload floors
        for (int i = 0; i < buildingResources.getAllFloorTilemaps().Count; i++) {
            Tilemap floorTilemap = buildingResources.getAllFloorTilemaps()[i];
            floorTilemap.SetTilesBlock(chunkBounds, nullTileArray);
        }
    }

    public void LoadData(GameData data) {
    }
    public void SaveData(GameData data) {
        // remove old chunk
        ChunkData chunkToRemove = null;
        foreach (ChunkData chunkData in data.worldData.chunkData) {
            if (chunkData.chunkCoord == this.chunkCoord) {
                chunkToRemove = chunkData;
            }
        }
        if (chunkToRemove != null) {
            data.worldData.chunkData.Remove(chunkToRemove);
        }

        updateTileArrays();
        data.worldData.chunkData.Add(new ChunkData(this));
    }

    private void generateObjectsFromChunkData(ChunkData chunkData) {
        this.objects = new GameObject[Consts.CHUNK_SIZE, Consts.CHUNK_SIZE];
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                if (chunkData.objects[Consts.IndexOf(i, j)] != 0) {
                    Vector3 pos = new Vector3(chunkPos.x + i + 0.5f, chunkPos.y + j + 0.5f, 198);

                    this.objects[i, j] = Instantiate(natureResources.idToGameObject[chunkData.objects[Consts.IndexOf(i, j)]], pos, Quaternion.identity);

                    this.objects[i, j].transform.SetParent(treeParent.transform, true);
                }
            }
        }
    }

    private void refreshContainFlags() {
        float heightVal;
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                heightVal = heights[i, j];

                // minus extra amount to have some overlapping
                if (heightVal >= Consts.WATER_HEIGHT - 0.1f) {
                    containsGrass = true;
                }
                if (heightVal < Consts.WATER_HEIGHT - 0.04f) {
                    containsWater = true;
                }
            }
        }
    }

    private void updateTileArrays() {
        wallTiles = getTilesInChunk(buildingResources.wallTilemap);

        for (int i = 0; i < buildingResources.getAllFloorTilemaps().Count; i++) {
            Tilemap floorTilemap = buildingResources.getAllFloorTilemaps()[i];
            floorTileData[i] = getTilesInChunk(floorTilemap);
        }
    }

    private TileBase[] getTilesInChunk(Tilemap tilemap) {
        if (TileResources.GetTilesBlockCount(tilemap, chunkBounds.min, chunkBounds.max) > 0) {
            return tilemap.GetTilesBlock(chunkBounds);
        }
        return null;
    }

    private void generateTerrainMaterials() {
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
    }

    private void generateWaterMaterials() {
        // generate a new material for the water in the chunk
        Material newWaterMaterial = new Material(Shader.Find("Unlit/ChunkWaterShader"));
        newWaterMaterial.CopyPropertiesFromMaterial(terrainData.chunkWaterMaterial);
        waterChunk.GetComponent<SpriteRenderer>().material = newWaterMaterial;
        waterChunk.GetComponent<SpriteRenderer>().material.SetTexture("_TileColours", terrainColours);
        hasGeneratedWaterMaterial = true;
    }
}

public enum ChunkStatus {
    NOT_GENERATED,
    LOADED,
    UNLOADED,
}