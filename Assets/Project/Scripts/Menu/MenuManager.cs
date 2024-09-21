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
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Panels")]
        [SerializeField] private GameObject _creditsPanel;

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
            creditsButton.onClick.AddListener(() => ShowCredits());
            quitButton.onClick.AddListener(() => QuitGame());
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
            SceneManager.LoadSceneAsync("Scenes/Gameplay/TrainBase");
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
