using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
public class DataPersistenceManager : MonoBehaviour
{
    [Header("File storage config")]
    [SerializeField] private string fileName;
    public GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one data persistence manager in scene");
        }
        instance = this;
    }
    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        // Load any saved data from a file using data handler.
        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
        Debug.Log("High Score = " + gameData.highscore);
    }
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}