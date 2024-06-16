using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
             pauseMenuGameObject.SetActive(!pauseMenuGameObject.activeSelf);
             GameManager.Instance.GameState = pauseMenuGameObject.activeSelf ? GameState.OnPause : GameState.OnGame;
             Time.timeScale = pauseMenuGameObject.activeSelf ? 0f: 1f;
         }
     }
     
     private void ResumeGame()
    {
        pauseMenuGameObject.SetActive(false);
        GameManager.Instance.GameState = GameState.OnGame;
        Time.timeScale = 1f;
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
