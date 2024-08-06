using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Inventory;
using UnityEngine;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    
    private string _dataDirPath;
    private string dataFileName = "";

    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[[SaveManager]] :: There is already a SaveManager on the scene");
            Destroy(this);
        }

        Instance = this; 
    }
    
    public void SavePlayerInventoryJson(DataPlayerInventory data)
    {
        //Create Folder
        _dataDirPath = Application.persistentDataPath;
        string fullPath = Path.Combine(_dataDirPath, "inventory", "playerInventory");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
   
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("SAVED: " + json);
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(json);
        }
    }
    public DataPlayerInventory TryLoadPlayerInventoryInBaseJson()
    {
        try
        {
            _dataDirPath = Application.persistentDataPath;
            string fullPath = Path.Combine(_dataDirPath, "inventory", "playerInventory");
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (StreamReader streamReader = new StreamReader(fullPath))
            {
                string jsonFromFile = streamReader.ReadToEnd();
                Debug.Log("LOADED: " + jsonFromFile);
                return JsonConvert.DeserializeObject<DataPlayerInventory>(jsonFromFile);
            }

        }
        catch (Exception error)
        {
            Debug.LogWarning("ERROR: " + error);
            return null;
        }
        return null;
    }
    public void SaveBaseInventoryJson(DataBaseInventory data)
    {
        //Create Folder
        _dataDirPath = Application.persistentDataPath;
        string fullPath = Path.Combine(_dataDirPath, "inventory", "baseInventory");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
   
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("SAVED: " + json);
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(json);
        }
    }
    public DataBaseInventory TryLoadInventoryInBaseJson()
    {
        try
        {
            _dataDirPath = Application.persistentDataPath;
            string fullPath = Path.Combine(_dataDirPath, "inventory", "baseInventory");
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (StreamReader streamReader = new StreamReader(fullPath))
            {
                string jsonFromFile = streamReader.ReadToEnd();
                Debug.Log("LOADED: " + jsonFromFile);
                return JsonConvert.DeserializeObject<DataBaseInventory>(jsonFromFile);
            }

        }
        catch (Exception error)
        {
            Debug.LogWarning("ERROR: " + error);
            return null;
        }
        return null;
    }


    public void SaveGame()
    {
        SaveInventory();
    }

    private void SaveInventory()
    {
        DataPlayerInventory idDictionary = new DataPlayerInventory(
            SaveManager.Instance.ConvertItemsDictionaryIntoIDDictionary(PlayerInventory.Instance.GetInventoryItems()));

        DataBaseInventory baseInventory = new DataBaseInventory(TrainBaseInventory.Instance
            .GetBaseInventoryToSave());
        
        SaveManager.Instance.SavePlayerInventoryJson(idDictionary);
        SaveManager.Instance.SaveBaseInventoryJson(baseInventory);
    }
    
    public  Dictionary<int, int> ConvertItemsDictionaryIntoIDDictionary(Dictionary<Item, int> items)
    {
        Dictionary<int, int> idDictionary = new Dictionary<int, int>();
        foreach (var itemPair in items)
        {
            idDictionary.Add(itemPair.Key.itemID, itemPair.Value);
        }
        return idDictionary;

    }
    
}
