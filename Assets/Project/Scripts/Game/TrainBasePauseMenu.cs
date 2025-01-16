using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainBasePauseMenu : MonoBehaviour
{
    
    public static TrainBasePauseMenu Instance;
    
    
   [SerializeField] private GameObject pauseMenuGameObject;
    [Header("Buttons")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button returnToMenuButton;
    [Header("SFX & Music Sliders")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider generalVolumeSlider;
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
        returnToMenuButton.onClick.AddListener(() => ReturnToMenuButton());
        quitButton.onClick.AddListener(() => QuitGame());
        pauseMenuGameObject.SetActive(false);
        LoadVolumeValues();
    }

    private void ReturnToMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    private void OnDestroy()
    {
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
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

    private bool AbilitiesNotActivated()
    {
        return !GameManager.Instance.GetHolder1Status() && !GameManager.Instance.GetHolder2Status();
    }
    
    private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
             if (TrainInventoryManager.Instance.GetInspectViewList().Count == 0)
             {
                 if (TrainManager.Instance.canvasActivated)
                 {
                     TrainManager.Instance.CloseWagonCanvas();
                 }else if (TrainManager.Instance.IsShowingWagonBuyUI)
                 {
                     TrainManager.Instance.BuyWagonScript.CancelBuy();
                 }
                 else if (!TrainInventoryManager.Instance.inventoryIsOpen)
                 {
                     PauseGame();
                 }

             }
         }
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
