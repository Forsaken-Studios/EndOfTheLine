using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
