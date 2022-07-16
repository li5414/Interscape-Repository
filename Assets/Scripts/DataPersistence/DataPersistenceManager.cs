using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour {
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    public GameObject chunkParent;
    public static DataPersistenceManager instance { get; private set; }
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler fileDataHandler;
    private ChunkManager chunkManager;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than 1 DataPersistenceManager in the scene");
        }
        instance = this;
    }

    private void Start() {
        chunkManager = this.gameObject.GetComponent<ChunkManager>();
        if (LoadWorldSettings.GetFileName() != null) {
            fileName = LoadWorldSettings.GetFileName();
        } else if (NewWorldSettings.GetFileName() != null) {
            fileName = NewWorldSettings.GetFileName();
        }
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        LoadGame();
    }

    void Update() {
        // TODO make save game buttons
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadGame();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            SaveGame();
        }
    }

    public void NewGame() {
        this.gameData = new GameData(
            NewWorldSettings.GetSeed(),
            NewWorldSettings.GetSeedString());
    }

    public void SaveGame() {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.SaveData(gameData);
        }

        fileDataHandler.Save(gameData);
        Debug.Log("Saved " + fileName);
    }

    public void LoadGame() {
        this.gameData = fileDataHandler.Load();


        if (this.gameData == null) {
            Debug.Log("No game data was found. Initialising to default");
            NewGame();
        } else {
            chunkManager.chunksFromFile = getChunksFromFile();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.LoadData(gameData);
        }
        Debug.Log("Loaded " + fileName);
    }

    public List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    Dictionary<Vector2Int, ChunkData> getChunksFromFile() {
        Dictionary<Vector2Int, ChunkData> chunksFromFile = new Dictionary<Vector2Int, ChunkData>();

        foreach (ChunkData chunkData in gameData.worldData.chunkData) {
            chunksFromFile.Add(chunkData.chunkCoord, chunkData);
        }
        return chunksFromFile;
    }
}
