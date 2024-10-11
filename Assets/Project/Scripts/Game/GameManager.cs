using System;
using System.Collections;
using System.Collections.Generic;
using Extraction;
using Inventory;
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

    [FormerlySerializedAs("inspectItemCanvas")]
    [Header("Canvas Helper")]
    [Tooltip("We use this reference, to link inspect item to this parent")]
    [SerializeField] private GameObject CanvasMenus;
    private string trainSceneName = "TrainBase";
    [SerializeField] private GameObject gridMain;
    private Collider2D wallCollider; 
    private Collider2D floorCollider;

    private bool holder1Activated = false;
    private bool holder2Activated = false;
    
    
    [Header("Extraction Properties")] 
    private GameState _gameState;
    public GameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }

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
        GetReferences();
        GameState = GameState.OnGame;
        
    }

    private void GetReferences()
    {
        if (SceneManager.GetActiveScene().name != trainSceneName)
        {
            wallCollider = gridMain.transform.Find("Walls").GetComponent<Collider2D>();
            floorCollider = gridMain.transform.Find("Floor").GetComponent<Collider2D>();
            blackFadeEndGamePanel = CanvasMenus.gameObject.transform.Find("Black Fade End Game Panel").gameObject;
            blackFadeEndGamePanel.SetActive(false);
        }
    }

    private IEnumerator EndGameCorroutine()
    {
        while (true)
        {
            blackFadeEndGamePanel.SetActive(true);
            blackFadeEndGamePanel.GetComponent<Animator>().SetTrigger("ending");

            yield return new WaitForSeconds(3f);
            SceneManager.LoadSceneAsync("Scenes/Gameplay/TrainBase");
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
        //Sell scrap Items && Save items for train base
        if (!died)
        {
            PlayerInventory.Instance.HandleItemsAtEndGame();
            PlayerInventory.Instance.RemoveCoinFromInventory();
            SaveManager.Instance.SavePlayerInventoryJson();
        }

        //Add one more day to game
        int currentDay = PlayerPrefs.GetInt("CurrentDay");
        PlayerPrefs.SetInt("PreviousDay", currentDay);
        PlayerPrefs.SetInt("CurrentDay", currentDay + 1);
        
        //End Game
        StartCoroutine(EndGameCorroutine());
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name == "TrainBase" || SceneManager.GetActiveScene().name == "MainMenu")
        {
            SaveManager.Instance.SaveGame();
        }
        else
        { 
            //SaveManager.Instance.EmptyDictionaryIfDisconnectInRaid();
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
