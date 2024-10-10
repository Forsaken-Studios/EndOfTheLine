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
 
    #region Save Expedition Rewards

    public void SaveExpeditionRewardJson(ItemsDiccionarySave data)
    {
        //Create Folder
        _dataDirPath = Application.persistentDataPath;
        string fullPath = Path.Combine(_dataDirPath, "expedition", "expeditionReward");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
   
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("SAVED: " + json);
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(json);
        }
    }
      
    public ItemsDiccionarySave TryLoadExpeditionRewardJson()
    {
        try
        {
            _dataDirPath = Application.persistentDataPath;
            string fullPath = Path.Combine(_dataDirPath, "expedition", "expeditionReward");
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (StreamReader streamReader = new StreamReader(fullPath))
            {
                string jsonFromFile = streamReader.ReadToEnd();
                Debug.Log("LOADED EXPEDITION REWARD: " + jsonFromFile);
                return JsonConvert.DeserializeObject<ItemsDiccionarySave>(jsonFromFile);
            }
        }
        catch (Exception error)
        {
            Debug.LogWarning("ERROR: " + error);
            return null;
        }
        return null;
    }
    
    #endregion
    #region Save Player Inventory
    public void SavePlayerInventoryJson()
    {
        //Create Folder
        Dictionary<int, int> idDictionary = SaveManager.Instance.ConvertItemsDictionaryIntoIDDictionary(PlayerInventory.Instance.GetInventoryItems());
        ItemsDiccionarySave data = new ItemsDiccionarySave(idDictionary);
        
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
    public ItemsDiccionarySave TryLoadPlayerInventoryInBaseJson()
    {
        try
        {
            _dataDirPath = Application.persistentDataPath;
            string fullPath = Path.Combine(_dataDirPath, "inventory", "playerInventory");
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (StreamReader streamReader = new StreamReader(fullPath))
            {
                string jsonFromFile = streamReader.ReadToEnd();
                Debug.Log("LOADED PLAYER INVENTORY: " + jsonFromFile);
                return JsonConvert.DeserializeObject<ItemsDiccionarySave>(jsonFromFile);
            }
        }
        catch (Exception error)
        {
            Debug.LogWarning("ERROR: " + error);
            return null;
        }
        return null;
    }
    #endregion
    #region Save Store 
    
    public void SaveCurrentDayStoreJson()
    {
        //Create Folder
        Dictionary<int, int> idDictionary = SaveManager.Instance.ConvertItemsDictionaryIntoIDDictionary(MarketSystem.Instance.GetItemsInMarket());
        ItemsDiccionarySave data = new ItemsDiccionarySave(idDictionary);
        
        _dataDirPath = Application.persistentDataPath;
        string fullPath = Path.Combine(_dataDirPath, "market", "store");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
   
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("SAVED: " + json);
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(json);
        }
    }
    public ItemsDiccionarySave TryLoadCurrentDayStoreJson()
    {
        try
        {
            _dataDirPath = Application.persistentDataPath;
            string fullPath = Path.Combine(_dataDirPath, "market", "store");
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (StreamReader streamReader = new StreamReader(fullPath))
            {
                string jsonFromFile = streamReader.ReadToEnd();
                Debug.Log("LOADED STORE: " + jsonFromFile);
                return JsonConvert.DeserializeObject<ItemsDiccionarySave>(jsonFromFile);
            }
        }
        catch (Exception error)
        {
            Debug.LogWarning("ERROR: " + error);
            return null;
        }
        return null;
    }
    
    
    #endregion
    
    
    #region Save Base Inventory
    public void SaveBaseInventoryJson(DataBaseInventory data)
    {
        //Create Folder
        _dataDirPath = Application.persistentDataPath;
        string fullPath = Path.Combine(_dataDirPath, "inventory", "baseInventory");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
   
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("SAVED BASE INVENTORY: " + json);
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
                Debug.Log("LOADED BASE INVENTORY: " + jsonFromFile);
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
    #endregion
    
    #region Save Inventory
    public void SaveGame()
    {
        SaveInventory();
    }

    public void EmptyDictionaryIfDisconnectInRaid()
    {
        SaveManager.Instance.SavePlayerInventoryJson();
    }
    
    private void SaveInventory()
    {
        try
        {
            DataBaseInventory baseInventory = new DataBaseInventory(TrainBaseInventory.Instance
                .GetBaseInventoryToSave());

            SaveManager.Instance.SavePlayerInventoryJson();
            SaveManager.Instance.SaveBaseInventoryJson(baseInventory);
        }
        catch (Exception err)
        {
            Debug.LogError("THERE WAS AN ERROR WHILE SAVING INVENTORY");
        }

    }
    #endregion
    
    public  Dictionary<int, int> ConvertItemsDictionaryIntoIDDictionary(Dictionary<Item, int> items)
    {
        Dictionary<int, int> idDictionary = new Dictionary<int, int>();
        foreach (var itemPair in items)
        {
            if (idDictionary.ContainsKey(itemPair.Key.itemID))
            {
                idDictionary[itemPair.Key.itemID] += itemPair.Value;
            }
            else
            {
                idDictionary.Add(itemPair.Key.itemID, itemPair.Value);
            }
  
        }
        return idDictionary;

    }
    
}
