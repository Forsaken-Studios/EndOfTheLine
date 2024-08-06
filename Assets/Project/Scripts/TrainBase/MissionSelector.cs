using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionSelector : MonoBehaviour
{
    [SerializeField] private Button playButton;
    
    private void Start()
    {
        playButton.onClick.AddListener(() => PlayMission());
    }

    private void PlayMission()
    {
        DataPlayerInventory idDictionary = new DataPlayerInventory(
            SaveManager.Instance.ConvertItemsDictionaryIntoIDDictionary(PlayerInventory.Instance.GetInventoryItems()));
        
        SaveManager.Instance.SavePlayerInventoryJson(idDictionary);
        //Aqui ya tendríamos que hacer la generación o lo que sea, por ahora iniciamos test level
        SceneManager.LoadSceneAsync("Scenes/Gameplay/WorkingSceneAdriG");
    }
}
