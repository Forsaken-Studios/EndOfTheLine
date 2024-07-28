using System;
using System.Collections;
using System.Collections.Generic;
using Extraction;
using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    
    [Header("End game")]
    [SerializeField] private GameObject blackFade;

    [Header("Canvas Helper")]
    [Tooltip("We use this reference, to link inspect item to this parent")]
    [SerializeField] private GameObject inspectItemCanvas; 
    
    [Header("Extraction Properties")] 
    private GameState _gameState;
    public GameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }
    
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
        GameState = GameState.OnGame;
        blackFade.SetActive(false);
    }
    
    private IEnumerator EndGameCorroutine()
    {
        while (true)
        {
            blackFade.SetActive(true);
            blackFade.GetComponent<Animator>().SetTrigger("ending");

            yield return new WaitForSeconds(3f);
            SceneManager.LoadSceneAsync("Scenes/Menu/MainMenu");
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
    
    public void EndGame()
    {
        //Sell scrap Items && Save items for train base
        PlayerInventory.Instance.HandleItemsAtEndGame();
        //Add one more day to game
        
        int currentDay = PlayerPrefs.GetInt("CurrentDay");
        PlayerPrefs.SetInt("PreviousDay", currentDay);
        PlayerPrefs.SetInt("CurrentDay", currentDay + 1);
        
        //End Game
        StartCoroutine(EndGameCorroutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public GameObject GetCanvasParent()
    {
        return inspectItemCanvas;
    }
}
