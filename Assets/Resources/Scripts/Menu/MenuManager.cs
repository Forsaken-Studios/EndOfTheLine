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
        [SerializeField] private Button quitButton;

   
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
            playButton.onClick.AddListener(() => PlayGame());
            quitButton.onClick.AddListener(() => QuitGame());
        }


        private void PlayGame()
        {
            SceneManager.LoadSceneAsync("Scenes/Gameplay/TestLevel");
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
 
}
