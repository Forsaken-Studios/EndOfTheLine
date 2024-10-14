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
    [Header("SFX & Music Sliders")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    
     private void Start()
    {
        playButton.onClick.AddListener(() => ResumeGame());
        returnToTrainButton.onClick.AddListener(() => ReturnToTrain());
        quitButton.onClick.AddListener(() => QuitGame());
        pauseMenuGameObject.SetActive(false);
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSfxValueChanged(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicValueChanged(); });
        LoadVolumeValues();
    }

    private void OnDestroy()
    {
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void OnSfxValueChanged()
    {
        SoundManager.Instance.ChangeSFXAudioVolume(sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }
    
    private void OnMusicValueChanged()
    {
        SoundManager.Instance.ChangeMusicAudioVolume(musicVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }
    private void LoadVolumeValues()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
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
