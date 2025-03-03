using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using LootSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.CustomLogs;
using Object = UnityEngine.Object;

public class TrainManager : MonoBehaviour
{

    public static TrainManager Instance;
    
    private TrainStatus _trainStatus;
    public  TrainStatus TrainStatus
    {
        get { return _trainStatus; }
        set { _trainStatus = value; }
    }
    /// <summary>
    /// Mission Selector - 0
    /// Control Room - 1
    /// Extra - 2
    /// </summary>
    private int currentIndex = 0;

    [Header("Day in Game")]
    private int currentDay = 0;
    [SerializeField] private TextMeshProUGUI currentDayText;


    
    [SerializeField] private int numberOfWagons;
    [SerializeField] private Train train;
    private TrainPanels trainPanelsScript;
    private List<GameObject> screensDisplayed;
    private BuyWagon buyWagon;
    public BuyWagon BuyWagonScript => buyWagon;
    [Header("Canvas for different wagons")]
    [SerializeField] private GameObject missionSelectorCanvas;
    [SerializeField] private GameObject controlRoomCanvas;
    [SerializeField] private GameObject marketRoomCanvas;
    [SerializeField] private GameObject loreRoomRoomCanvas;
    [SerializeField] private GameObject barmanRoomCanvas;
    [SerializeField] private GameObject expeditionRoomCanvas;
    [Header("Wagon Lock List")] 
    private bool[] unlockedWagonsList; //True -> Unlocked, False -> Locked
    [Header("Lock Icon")]
    [SerializeField] private GameObject lockIcon;
    private GameObject currentCanvas;
    public bool canvasActivated => currentCanvas.activeSelf;
    
    [SerializeField] private TraderPanel tradePanel;
    
    [Header("Resources In Train")] 
    private int RESOURCES_AIR_FILTER; 

    private string RESOURCES_AIR_FILTER_NAME = "Resources_Air_Filter";
    [Header("Buy Wagon UI Prefab")] 
    [SerializeField] private GameObject buyWagonPrefab;
    private bool isShowingWagonBuyUI = false;
    public bool IsShowingWagonBuyUI => isShowingWagonBuyUI;
    [SerializeField] private GameObject generalCanvas;

    [Header("Music Mixer Check")]
    [SerializeField] MusicManager musicManager;
    /// <summary>
    /// 0 - Food
    /// 1 - Material
    /// 2 - Gold
    /// </summary>
    public delegate void OnVariableChangeDelegate(int newVal, int resourceType);
    public event OnVariableChangeDelegate OnVariableChange;
    public event EventHandler OnDayChanged;

    public int resourceAirFilter
    {
        get { return RESOURCES_AIR_FILTER; }
        set
        {
            RESOURCES_AIR_FILTER = value;
            if (OnVariableChange != null)
            {
                OnVariableChange(RESOURCES_AIR_FILTER, 0);
                PlayerPrefs.SetInt(RESOURCES_AIR_FILTER_NAME, RESOURCES_AIR_FILTER);
            }
        }
    }    

    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[GameManager.cs] : There is already a TrainManager Instance");
            Destroy(this);
        }
        Instance = this;
        unlockedWagonsList = new bool[numberOfWagons];
    }
    
    
    private void Start()
    {
        currentCanvas = missionSelectorCanvas;
        TrainStatus = TrainStatus.onMissionSelector;
        trainPanelsScript = GetComponent<TrainPanels>();
        screensDisplayed = new List<GameObject>();
        resourceAirFilter = PlayerPrefs.GetInt(RESOURCES_AIR_FILTER_NAME);
        
        NewDayInGame();
       //PlayerPrefs.SetInt("Wagon " + 3,0);
        LoadWagonsUnlockedList();
    }

    private bool ValidStatusToMove()
    {
        return TrainStatus != TrainStatus.showingSpecialScreen && GameManager.Instance.GameState != GameState.OnInventory; 
    }

    public bool ValidStatusToOpenInventory()
    {
        return TrainStatus != TrainStatus.showingSpecialScreen;
    }

    // Update is called once per frame
    void Update()
    {
        if(ValidStatusToOpenInventory() && GameManager.Instance.GameState != GameState.OnInventory)
            HandleButtonPressed();
        if(!canvasActivated && ValidStatusToMove())
            HandleMovement();
    }

    private void LoadWagonsUnlockedList()
    {
        //Mission Selector always true [Unlocked]
        unlockedWagonsList[0] = true;
        //Home always unlocked
        unlockedWagonsList[1] = true;
        //lore room always unlocked
        unlockedWagonsList[2] = true;
        //bar room always unlocked
        unlockedWagonsList[3] = true;
        //Activates the mechanic, lore and barman tune
        musicManager.StartFadeFunction(0);
        musicManager.StartFadeFunction(1);
        musicManager.StartFadeFunction(2);
        //TODO: Just for testing
        //PlayerPrefs.SetInt("Wagon 5", -1);
        //PlayerPrefs.SetInt("Wagon 6", -1);
        // -1 = Locked | 0 = Not defined | 1 = Unlocked
        if (PlayerPrefs.GetInt("Wagon 5") == 0)
            PlayerPrefs.SetInt("Wagon 5", -1);     
        if (PlayerPrefs.GetInt("Wagon 6") == 0)
            PlayerPrefs.SetInt("Wagon 6", -1);
        
        unlockedWagonsList[4] = PlayerPrefs.GetInt("Wagon 5") == 1;
        if (unlockedWagonsList[4]) musicManager.StartFadeFunction(3); //TODO: change to correct number in call when all wagons are set properly
        unlockedWagonsList[5] = PlayerPrefs.GetInt("Wagon 6") == 1;
        if (unlockedWagonsList[5]) musicManager.StartFadeFunction(4); //TODO: change to correct number in call when all wagons are set properly


    }

    private void HandleMovement()
    {
        if (!isShowingWagonBuyUI)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //Check if we are already on top left (For now extra room)
                if (TrainStatus != TrainStatus.onExpeditionRoom)
                {
                    MoveTrain(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {

                //Check if we are already on top right
                if (TrainStatus != TrainStatus.onMissionSelector)
                {
                    MoveTrain(false);
                }
            }
        }
    }

    public void MoveTrain(bool movingToLeft)
    {
            if (movingToLeft)
            {
                if (TrainStatus == TrainStatus.onMarketRoom) TrainArrows.Instance.HideLeftArrow();
                if (TrainStatus == TrainStatus.onMissionSelector) TrainArrows.Instance.ShowRightArrow();
                //LogManager.Log("MOVING TRAIN TO LEFT", FeatureType.TrainBase);
                currentIndex++;
                UpdateRoomInfo();
                trainPanelsScript.HideTrainRoom(currentIndex - 1);
                trainPanelsScript.ShowTrainRoom(currentIndex, unlockedWagonsList[currentIndex]);
            }
            else
            {
                if (TrainStatus == TrainStatus.onMechanicRoom) TrainArrows.Instance.HideRightArrow();
                if (TrainStatus == TrainStatus.onExpeditionRoom) TrainArrows.Instance.ShowLeftArrow();
                //LogManager.Log("MOVING TRAIN TO RIGHT", FeatureType.TrainBase);
                currentIndex--;
                trainPanelsScript.HideTrainRoom(currentIndex + 1);
                trainPanelsScript.ShowTrainRoom(currentIndex, unlockedWagonsList[currentIndex]);
            }
            UpdateRoomInfo();
            train.MoveTrain(currentIndex);  
    }
    public void MoveTrainOnClick(bool movingToLeft)
    {
        if (!isShowingWagonBuyUI)
        {        
            if (movingToLeft)
            {
                if (TrainStatus != TrainStatus.onExpeditionRoom)
                {
                    //LogManager.Log("MOVING TRAIN TO LEFT", FeatureType.TrainBase);
                    currentIndex++;
                    UpdateRoomInfo();
                    trainPanelsScript.HideTrainRoom(currentIndex - 1);
                    trainPanelsScript.ShowTrainRoom(currentIndex, unlockedWagonsList[currentIndex]);
                }
            }
            else
            {
                if (TrainStatus != TrainStatus.onMissionSelector)
                {
                    //LogManager.Log("MOVING TRAIN TO RIGHT", FeatureType.TrainBase);
                    currentIndex--;
                    trainPanelsScript.HideTrainRoom(currentIndex + 1);
                    trainPanelsScript.ShowTrainRoom(currentIndex, unlockedWagonsList[currentIndex]);
                }
            }
            
            UpdateRoomInfo();
            train.MoveTrain(currentIndex);  
        }
    }
    
    private void UpdateRoomInfo()
    {
        lockIcon.SetActive(!unlockedWagonsList[currentIndex]);
        switch (currentIndex)
        {
            case 0:
                TrainStatus = TrainStatus.onMissionSelector;
                currentCanvas = missionSelectorCanvas;
                break; 
            case 1:
                TrainStatus = TrainStatus.onMechanicRoom;
                currentCanvas = controlRoomCanvas;
                break; 
            case 2:
                TrainStatus = TrainStatus.onLoreRoom;
                currentCanvas = loreRoomRoomCanvas;
                break;    
            case 3:
                TrainStatus = TrainStatus.onBarmanRoom;
                currentCanvas = barmanRoomCanvas;
                break;
            case 4: 
                TrainStatus = TrainStatus.onMarketRoom;
                currentCanvas = marketRoomCanvas;
                break;
            case 5: 
                TrainStatus = TrainStatus.onExpeditionRoom;
                currentCanvas = expeditionRoomCanvas;
                break;
        }
    }
    
    private void HandleButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (unlockedWagonsList[currentIndex])
            {
                if (currentCanvas.activeSelf)
                {
                    CloseWagonCanvas();
                }
                else
                {
                    currentCanvas.SetActive(true);
                }
            }
            else
            {
                if (!isShowingWagonBuyUI)
                {
                    isShowingWagonBuyUI = true;
                    //We would like to buy this wagon
                    Object price = UnityEngine.Resources.Load("Wagons/Wagon " + (currentIndex + 1));
                    WagonPrice wagonPrice = price as WagonPrice;
                    
                    Vector2 initialPosition = new Vector2(960, 540);
                    GameObject buyWagonUI = Instantiate(buyWagonPrefab, initialPosition, Quaternion.identity, generalCanvas.transform);
                    buyWagon = buyWagonUI.GetComponent<BuyWagon>();
                    buyWagonUI.GetComponent<BuyWagon>().SetUpProperties(wagonPrice.foodNeeded, wagonPrice.materialNeeded, wagonPrice.goldNeeded);
                }
            }
        } 
    }

    public void CloseWagonCanvas()
    {
        //Check if we need to disable some screen before
        if (screensDisplayed.Count == 0)
        {
            currentCanvas.SetActive(false);
        }
        else
        {
            RemoveScreenFromList(screensDisplayed.Last());
        }
    }
    


    public MissionSelector GetMissionSelector()
    {
        return missionSelectorCanvas.GetComponentInChildren<MissionSelector>();
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public bool TryToBuyWagon(int airFilterNeeded, int material1Needed, int material2Needed)
    {
        Item foodItem = UnityEngine.Resources.Load<Item>("Items/Scrap/FoodcanHam");
        Item screwsItem = UnityEngine.Resources.Load<Item>("Items/Scrap/Screws");
        
        if (airFilterNeeded <= resourceAirFilter && TrainBaseInventory.Instance.GetIfItemIsInInventory(foodItem, material2Needed) &&
            TrainBaseInventory.Instance.GetIfItemIsInInventory(screwsItem, material1Needed))
        {
            resourceAirFilter -= airFilterNeeded;
            TrainBaseInventory.Instance.DeleteItemFromList(foodItem, material2Needed);
            TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(foodItem, material2Needed);
            TrainBaseInventory.Instance.DeleteItemFromList(screwsItem, material1Needed);
            TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(screwsItem, material1Needed);
            unlockedWagonsList[currentIndex] = true;
            trainPanelsScript.UnlockTrain(currentIndex);
            PlayerPrefs.SetInt("Wagon " + (currentIndex + 1), 1);
            isShowingWagonBuyUI = false;
            lockIcon.SetActive(false);
            musicManager.StartFadeFunction(currentIndex-1);
            return true;
        }
        else
        {
            return false;
        }
    }

    
    public void SetIsShowingWagonBuyUI(bool aux)
    {
        isShowingWagonBuyUI = aux;
    }

    private void NewDayInGame()
    {
        int previousDay = PlayerPrefs.GetInt("PreviousDay"); 
        int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
        if (currentDayLocal != previousDay)
        {
            tradePanel.HandleTrade();
            PlayerPrefs.SetInt("PreviousDay", currentDayLocal);
            this.currentDay = currentDayLocal;
            currentDayText.text = "DAY: " + currentDayLocal.ToString();
            OnDayChanged?.Invoke(this, EventArgs.Empty);
            //Update store
            UpdateStore();
        }
        else
        {
            currentDayText.text = "DAY: " + currentDayLocal.ToString();
        }
        
        //Handle expedition behavior
        //HandleExpeditionResult(currentDayLocal); -> Now in takeRewardsButton
    }

 


    private void UpdateStore()
    {
        OnDayChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddScreenToList(GameObject newScreen)
    {
        this.screensDisplayed.Add(newScreen);
    }

    public void RemoveScreenFromList(GameObject screen)
    {
        this.screensDisplayed.Remove(screen);
        Destroy(screen);
    }
}
