using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Resources.Scripts.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;
    
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button backFromOptionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Panels")]
        [SerializeField] private GameObject _creditsPanel;
        [SerializeField] private GameObject buttonsPanel;
        [SerializeField] private GameObject optionsPanel;

        [Header("General & SFX & Music Sliders")]
        [SerializeField] private Slider generalVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        
        private bool _isOnCredits = false;
   
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[MenuManager.cs] : There is already a MenuManager Instance");
                Destroy(this);
            }
            Instance = this;
        }

        private void Start()
        {
            _isOnCredits = false;

            playButton.onClick.AddListener(() => PlayGame());
            optionsButton.onClick.AddListener(() => ActivateOptionsPanel());
            backFromOptionsButton.onClick.AddListener(() => BackFromOptions());
            creditsButton.onClick.AddListener(() => ShowCredits());
            quitButton.onClick.AddListener(() => QuitGame());
            LoadVolumeValues();
        }

        private void BackFromOptions()
        {
            optionsPanel.SetActive(false);
            buttonsPanel.SetActive(true);
        }

        private void ActivateOptionsPanel()
        {
            optionsPanel.SetActive(true);
            buttonsPanel.SetActive(false);
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
        
        void Update()
        {
            CheckCredits();

            if(Input.GetKeyDown(KeyCode.Escape) && _isOnCredits)
            {
                _isOnCredits = false;
            }
        }

        private void CheckCredits()
        {
            if (_isOnCredits)
            {
                _creditsPanel.SetActive(true);
            }
            else
            {
                _creditsPanel.SetActive(false);
            }
        }

        private void PlayGame()
        {
            bool tutorialPlayed = PlayerPrefs.GetInt("TutorialPlayed") == 1;
            
            if(tutorialPlayed)
                SceneManager.LoadSceneAsync(1);
            else
                SceneManager.LoadSceneAsync(3);
        }

        private void QuitGame()
        {
            Application.Quit();
        }

        private void ShowCredits()
        {
            _isOnCredits = true;
        }
    }
 
}
