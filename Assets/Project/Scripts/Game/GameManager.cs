using System;
using System.Collections;
using System.Collections.Generic;
using Extraction;
using Inventory;
using Player;
using SaveManagerNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    
    [Header("End game")]
    private GameObject blackFadeEndGamePanel;
    [HideInInspector]
    public bool playerIsDead;

    [FormerlySerializedAs("inspectItemCanvas")]
    [Header("Canvas Helper")]
    [Tooltip("We use this reference, to link inspect item to this parent")]
    [SerializeField] private GameObject CanvasMenus;
    private string trainSceneName = "TrainBase";
    [SerializeField] private GameObject gridMain;
    private Collider2D wallCollider; 
    private Collider2D floorCollider;
    [Header("Loading Screen - Not needed in trainBase")]
    public GameObject loadingScreen;
    //public Image LoadingBarFill;
    public bool sceneIsLoading;


    
    private bool holder1Activated = false;
    private bool holder2Activated = false;
    

    private GameState _gameState;
    public GameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }
    [Header("Inventory Slot Size Properties")] 
    [SerializeField] private int MAX_AMOUNT_PER_SLOT_BASE = 4;
    [SerializeField] private int MAX_AMOUNT_PER_SLOT_GAME = 3;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[GameManager.cs] : There is already a GameManager Instance");
            Destroy(this);
        }
        Instance = this;
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name != trainSceneName)
        {
            StartCoroutine(ActivateLoadingScreen());
            GameState = GameState.onLoad;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.OnGame && SceneManager.GetActiveScene().name != trainSceneName)
        {
            Collider2D floorColliderAux = null;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Floor")
                {
                    if (floorCollider != wallCollider || wallCollider == null)
                    {
                        GameObject parent = hit.collider.gameObject.transform.parent.Find("Walls").gameObject;
                        wallCollider = parent.GetComponent<TilemapCollider2D>();
                        floorColliderAux = hit.collider;
                        floorCollider = floorColliderAux;
                    }
       
                }
            }
        }
   

    }
    private Vector2 GetPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    
    private IEnumerator ActivateLoadingScreen()
    {
        loadingScreen.SetActive(true);
        blackFadeEndGamePanel = CanvasMenus.gameObject.transform.Find("Black Fade End Game Panel").gameObject;
        while (sceneIsLoading)
        {
            //float newValue = LoadingBarFill.fillAmount += 0.25f;
            //LoadingBarFill.fillAmount = Mathf.Clamp(newValue, 0f, 0.80f);
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.5f);
        blackFadeEndGamePanel.SetActive(true);
        blackFadeEndGamePanel.GetComponent<Animator>().SetTrigger("starting");
        //LoadingBarFill.fillAmount = 1.0f;
        loadingScreen.SetActive(false);
        GetReferences();
        GameState = GameState.OnGame;
    }

    private void GetReferences()
    {
        if (SceneManager.GetActiveScene().name != trainSceneName)
        {
            //wallCollider = gridMain.transform.Find("Walls").GetComponent<Collider2D>();
            //floorCollider = gridMain.transform.Find("Floor").GetComponent<Collider2D>();
            //blackFadeEndGamePanel = CanvasMenus.gameObject.transform.Find("Black Fade End Game Panel").gameObject;
            //blackFadeEndGamePanel.SetActive(false);
        }
    }

    private IEnumerator EndGameCorroutine()
    {
        while (true)
        {
            PlayerController.Instance.SetIfPlayerCanMove(false);
            blackFadeEndGamePanel.SetActive(true);
            blackFadeEndGamePanel.GetComponent<Animator>().SetTrigger("ending");

            yield return new WaitForSeconds(3f);
            SceneManager.LoadSceneAsync(1);
            StopAllCoroutines();
            yield return null; 
        }
    }
    
    public void ActivateExtractionZone()
    {
        ExtractionManager.Instance.SetIfExtractionArrived(true);
    }

    public void DesactivateExtractionZone()
    {
        ExtractionManager.Instance.SetIfExtractionArrived(false);
    }
    
    public void EndGame(bool died=true)
    {
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            if (died)
            {
                PlayerController.Instance.GameObject().transform.Find("PlayerBody").GetComponent<SpriteRenderer>()
                    .sortingOrder = 2;
                SceneManager.LoadSceneAsync(3); 
            }
            else
            {
                PlayerPrefs.SetInt("TutorialPlayed", 1);
                StartCoroutine(EndGameCorroutine());
            }
        }
        else
        {
            //Sell scrap Items && Save items for train base
            if (!died) // si queremos que el tutorial dé los objetos, pasamos esto a general
            {
                PlayerInventory.Instance.HandleItemsAtEndGame();
                PlayerInventory.Instance.RemoveCoinFromInventory();
                SaveManager.Instance.SavePlayerInventoryJson();
            } else if (died)
            {
                Debug.Log("[GameManager.cs] : Player has died.");
                PlayerController.Instance.GameObject().transform.Find("PlayerBody").GetComponent<SpriteRenderer>()
                    .sortingOrder = 2;
                playerIsDead = true;
                PlayerAim playerAim = PlayerController.Instance.gameObject.GetComponent<PlayerAim>();
                PlayerController.Instance.GetComponentInChildren<CircleCollider2D>().enabled = false;
                PlayerController.Instance.GetComponentInChildren<Rigidbody2D>().Sleep();
                playerAim.SetIfCanRotateAim(false);
                playerAim.RemoveTriangle();
                PlayerController.Instance.PlayDeathAnimation();
            }

            //Add one more day to game
            int currentDay = PlayerPrefs.GetInt("CurrentDay");
            PlayerPrefs.SetInt("PreviousDay", currentDay);
            PlayerPrefs.SetInt("CurrentDay", currentDay + 1);
        }
        
  
        
        //End Game
        StartCoroutine(EndGameCorroutine());
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name == "TrainBase" || SceneManager.GetActiveScene().name == "MainMenu")
        {
            SaveManager.Instance.SaveGame();
        }
    }
    
    public int GetMaxAmountPerSlot()
    {
        if (SceneManager.GetActiveScene().name == trainSceneName)
        {
            return MAX_AMOUNT_PER_SLOT_BASE;
        }
        else
        {
          return MAX_AMOUNT_PER_SLOT_GAME;  
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public GameObject GetCanvasParent()
    {
        return CanvasMenus;
    }

    public string GetNameTrainScene()
    {
        return trainSceneName;
    }
    
    public Collider2D GetWallCollider()
    {
        return wallCollider;
    } 
    public Collider2D GetFloorCollider()
    {
        return floorCollider;
    }
    
    public void SetHolder(int id, bool aux)
    {
        if (id == 1)
        {
            this.holder1Activated = aux;
        }
        else
        {
            this.holder2Activated = aux;
        }
    }
    
    public bool GetHolder1Status()
    {
        return holder1Activated;
    }
    public bool GetHolder2Status()
    {
        return holder2Activated;
    }

    public GameObject GetMenuCanvas()
    {
        return CanvasMenus;
    }
}
