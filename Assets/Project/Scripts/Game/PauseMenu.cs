using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static PauseMenu Instance;

    [SerializeField] private GameObject pauseMenuGameObject;
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button returnToTrainButton;
    [Header("General & SFX & Music Sliders")]
    [SerializeField] private Slider generalVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("[PauseMenu.cs] : There is already a pause menu");
        }
    }

    private void Start()
    {
        playButton.onClick.AddListener(() => ResumeGame());
        returnToTrainButton.onClick.AddListener(() => ReturnToTrain());
        quitButton.onClick.AddListener(() => QuitGame());
        pauseMenuGameObject.SetActive(false);
        LoadVolumeValues();
    }

    public void LoadVolumeValues()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            generalVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        else
            generalVolumeSlider.value = 0.5f;
        
        if (PlayerPrefs.HasKey("SoundEffectsVolume"))
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume");
        else
            sfxVolumeSlider.value = 0.5f;   
        
        
        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        else
            musicVolumeSlider.value = 0.5f;
    }

    private void ReturnToTrain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);
    }

    private bool AbilitiesNotActivated()
    {
        return !GameManager.Instance.GetHolder1Status() && !GameManager.Instance.GetHolder2Status();
    }
    
    
    
    private void Update()
     {
         if (!GameManager.Instance.sceneIsLoading)
         {
             if (Input.GetKeyDown(KeyCode.Escape))
             {
                 if (InventoryManager.Instance.GetInspectViewList().Count == 0 
                     && !LoreManager.Instance.GetIfPlayerIsReadingLore())
                 {
                     if (AbilitiesNotActivated() && !InventoryManager.Instance.inventoryIsOpen)
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
                     }
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
        //Si no esta activado, paramos sonidos
        if (pauseMenuGameObject.activeSelf)
        {
            SoundManager.Instance.ResumeSounds();
        }
        else
        {
            SoundManager.Instance.PauseSounds();
        }
        
        //Ponemos lo contrario a lo que estaba
        pauseMenuGameObject.SetActive(!pauseMenuGameObject.activeSelf);
        //Si ahora está activado, es que hemos puesto en pausa
        GameManager.Instance.GameState = pauseMenuGameObject.activeSelf ? GameState.OnPause : GameState.OnGame;
        Time.timeScale = pauseMenuGameObject.activeSelf ? 0f: 1f;
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
