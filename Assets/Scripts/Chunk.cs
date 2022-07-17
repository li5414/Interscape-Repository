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
            chunkManager = GameObject.FindWithTag("GameManager").GetComponent<ChunkManager>();
            terrainData = GameObject.FindWithTag("GameManager").GetComponent<TerrainDataGenerator>();
            plantsGenerator = GameObject.FindWithTag("GameManager").GetComponent<PlantsGenerator>();
            worldSettings = GameObject.FindWithTag("GameManager").GetComponent<WorldSettings>();
            buildingResources = GameObject.FindWithTag("GameManager").GetComponent<BuildingResources>();
            tileResources = GameObject.FindWithTag("GameManager").GetComponent<TileResources>();
            natureResources = GameObject.FindWithTag("GameManager").GetComponent<NatureResources>();
        }
    }

    public void GenerateChunk() {
        heights = terrainData.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
        temps = terrainData.GetTemperaturesExtended(chunkPos.x, chunkPos.y, heights);
        humidities = terrainData.GetHumidityValuesExtended(chunkPos.x, chunkPos.y, heights);
        biomes = terrainData.GetBiomesExtended(heights, temps, humidities);

        refreshContainFlags();
        generateExtendedColourTexture();

        // generate array of gameobjects (TODO optimise: use dict/list instead?)
        objects = plantsGenerator.GeneratePlants(chunkPos, biomes, heights, treeParent);

        generateSand();
        generateVillage();

        treeParent.SetActive(false);
        chunkManager.chunksToLoad.Enqueue(this);
    }

    public void GenerateChunkFromFile(ChunkData chunkData) {
        heights = terrainData.GetHeightValuesExtended(chunkPos.x, chunkPos.y);
        temps = terrainData.GetTemperaturesExtended(chunkPos.x, chunkPos.y, heights);
        humidities = terrainData.GetHumidityValuesExtended(chunkPos.x, chunkPos.y, heights);
        biomes = terrainData.GetBiomesExtended(heights, temps, humidities);

        refreshContainFlags();
        generateExtendedColourTexture();

        getObjectsFromChunkData(chunkData);

        sandTiles = getTilesFromArray(chunkData.sandTiles, tileResources.tileSandRule);
        wallTiles = getTilesFromArray(chunkData.wallTiles, buildingResources.idToRuleTile);

        // get floor tiles
        for (int floor = 0; floor < chunkData.floorTiles.getAll().Count; floor++) {
            bool[] floorTileBools = chunkData.floorTiles.getAll()[floor];
            floorTileData[floor] = getTilesFromArray(floorTileBools, buildingResources.floorBaseTile);
        }

        treeParent.SetActive(false);
        chunkManager.chunksToLoad.Enqueue(this);
    }

    private void getObjectsFromChunkData(ChunkData chunkData) {
        this.objects = new GameObject[Consts.CHUNK_SIZE, Consts.CHUNK_SIZE];
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                if (chunkData.objects[at(i, j)] != 0) {
                    Vector3 pos = new Vector3(chunkPos.x + i + 0.5f, chunkPos.y + j + 0.5f, 198);

                    this.objects[i, j] = Instantiate(natureResources.idToGameObject[chunkData.objects[at(i, j)]], pos, Quaternion.identity);

                    this.objects[i, j].transform.SetParent(treeParent.transform, true);
                }
            }
        }
    }

    private TileBase[] getTilesFromArray(bool[] tileBools, RuleTile tile) {
        TileBase[] tiles = null;
        if (tileBools != null) {
            for (int i = 0; i < tileBools.Length; i++) {
                if (tileBools[i]) {
                    if (tiles == null) {
                        tiles = new TileBase[tileBools.Length];
                    }
                    tiles[i] = tile;
                }
            }
        }
        return tiles;
    }

    private TileBase[] getTilesFromArray(int[] tileIds, Dictionary<int, RuleTile> tileIdDict) {
        TileBase[] tiles = null;
        if (tileIds != null) {
            for (int i = 0; i < tileIds.Length; i++) {
                if (tileIds[i] > 0) {
                    if (tiles == null) {
                        tiles = new TileBase[tileIds.Length];
                    }
                    tiles[i] = tileIdDict[tileIds[i]];
                }
            }
        }
        return tiles;
    }

    private void generateVillage() {
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
    private void generateExtendedColourTexture() {
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

    private void generateSand() {
        float heightVal;

        // in loop, enter all positions and tiles in arrays
        for (int i = 0; i < Consts.CHUNK_SIZE; i++) {
            for (int j = 0; j < Consts.CHUNK_SIZE; j++) {
                // get features for this tile
                heightVal = heights[i, j];

                // sand layer
                if (heightVal < Consts.BEACH_HEIGHT) {
                    if (sandTiles == null) {
                        sandTiles = new RuleTile[Consts.CHUNK_SIZE_SQUARED];
                    }
                    sandTiles[at(i, j)] = tileResources.tileSandRule;
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

    public void LoadChunk() {
        // set parent gameobject for the plants
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
                grassBlades.SetActive(true);
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

        // save tiles to their respective arrays
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

    private void updateTileArrays() {
        wallTiles = getTilesInChunk(buildingResources.wallTilemap);

        for (int i = 0; i < buildingResources.getAllFloorTilemaps().Count; i++) {
            Tilemap floorTilemap = buildingResources.getAllFloorTilemaps()[i];
            floorTileData[i] = getTilesInChunk(floorTilemap);
        }
    }

    public int GetTilesBlockCount(Tilemap tilemap, Vector3Int min, Vector3Int max) {
        int count = 0;
        for (int i = 0; i < max.y - min.y; i++) {
            Vector3Int minRowPos = new Vector3Int(chunkBounds.min.x, chunkBounds.min.y + i, chunkBounds.min.z);
            Vector3Int maxRowPos = new Vector3Int(chunkBounds.max.x - 1, chunkBounds.min.y + i, chunkBounds.max.z);

            count += tilemap.GetTilesRangeCount(minRowPos, maxRowPos);
        }
        return count;
    }

    private TileBase[] getTilesInChunk(Tilemap tilemap) {

        if (GetTilesBlockCount(tilemap, chunkBounds.min, chunkBounds.max) > 0) {
            return tilemap.GetTilesBlock(chunkBounds);
        }
        return null;
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

    // lookup index of 16x16 2D array condensed to 1D array
    public int at(int x, int y) {
        return (x + 16 * y);
    }
}

public enum ChunkStatus {
    NOT_GENERATED,
    LOADED,
    UNLOADED,
}


