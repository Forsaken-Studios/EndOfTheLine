using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenuGameObject;
        
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button returnToTrainButton;
    
     private void Start()
    {
        playButton.onClick.AddListener(() => ResumeGame());
        returnToTrainButton.onClick.AddListener(() => ReturnToTrain());
        quitButton.onClick.AddListener(() => QuitGame());
        pauseMenuGameObject.SetActive(false);
    }

    private void ReturnToTrain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("Scenes/Gameplay/TrainBase");
    }

    private bool AbilitiesNotActivated()
    {
        return !GameManager.Instance.GetHolder1Status() && !GameManager.Instance.GetHolder2Status();
    }
    
    private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
             if (InventoryManager.Instance.GetInspectViewList().Count == 0 
                 && !LoreManager.Instance.GetIfPlayerIsReadingLore())
             {
                 if (AbilitiesNotActivated())
                 {
                     PauseGame();
                 }
             }
             else
             {
                 if (LoreManager.Instance.GetIfPlayerIsReadingLore())
                 {
                     GameManager.Instance.GameState = GameState.OnGame;
                     LoreManager.Instance.SetIfPlayerIsReadingLore(false);
                     LoreManager.Instance.DestroyCurrentExpandedView(); 
                     LoreManager.Instance.SetCurrentLoreView(null);
                 }else
                 {
                     List<GameObject> inspectList = InventoryManager.Instance.GetInspectViewList();
                     GameObject mostRecentInspectView = inspectList[inspectList.Count - 1];
                     Destroy(mostRecentInspectView);
                     InventoryManager.Instance.RemoveInspectView(mostRecentInspectView);
                 }
             }
         }
     }
     
     private void ResumeGame()
    {
        pauseMenuGameObject.SetActive(false);
        GameManager.Instance.GameState = GameState.OnGame;
        Time.timeScale = 1f;
    }

    private void PauseGame()
    {
        pauseMenuGameObject.SetActive(!pauseMenuGameObject.activeSelf);
        GameManager.Instance.GameState = pauseMenuGameObject.activeSelf ? GameState.OnPause : GameState.OnGame;
        Time.timeScale = pauseMenuGameObject.activeSelf ? 0f: 1f;
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
