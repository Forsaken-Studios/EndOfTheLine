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
    [SerializeField] private GameObject tryingToRaidWithResourcesPRefab;
    private void Start()
    {
        playButton.onClick.AddListener(() => PlayMission());
    }

    public void MakePlayButtonInteractable()
    {
        playButton.interactable = true;
    }

    private void PlayMission()
    {
        if (PlayerInventory.Instance.GetInventoryItems().Count > 0)
        {
            //Show message
            Instantiate(tryingToRaidWithResourcesPRefab, new Vector2(0, 0), Quaternion.identity);
            TrainManager.Instance.TrainStatus = TrainStatus.showingSpecialScreen;
            playButton.interactable = false;
        }
        else
        {
            SaveManager.Instance.SaveGame();
            //Aqui ya tendríamos que hacer la generación o lo que sea, por ahora iniciamos test level
            SceneManager.LoadSceneAsync("Scenes/Gameplay/NewWorkingSceneAdriG");
        }
    }
}
