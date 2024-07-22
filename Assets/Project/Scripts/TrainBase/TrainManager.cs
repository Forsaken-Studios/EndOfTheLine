using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.CustomLogs;

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
    [SerializeField] private Train train;
    private TrainPanels trainPanelsScript;
    [Header("Canvas for different wagons")]
    [SerializeField] private GameObject missionSelectorCanvas;
    [SerializeField] private GameObject controlRoomCanvas;
    [SerializeField] private GameObject extraRoomCanvas;

    private GameObject currentCanvas;

    [Header("Resources In Train")] 
    private int RESOURCES_GOLD; 
    private int RESOURCES_FOOD; 
    private int RESOURCES_MATERIAL; 

    private string RESOURCES_GOLD_NAME = "Resources_Gold"; 
    private string RESOURCES_FOOD_NAME = "Resources_Food"; 
    private string RESOURCES_MATERIAL_NAME = "Resources_Material";
    
    
    /// <summary>
    /// 0 - Food
    /// 1 - Material
    /// 2 - Gold
    /// </summary>
    public delegate void OnVariableChangeDelegate(int newVal, int resourceType);
    public event OnVariableChangeDelegate OnVariableChange;
    public int resourceGold
    {
        get { return RESOURCES_GOLD; }
        set
        {
            RESOURCES_GOLD = value;
            if (OnVariableChange != null)
                OnVariableChange(RESOURCES_GOLD, 2);

        
        }
    }   
    public int resourceFood
    {
        get { return RESOURCES_FOOD; }
        set
        {
            RESOURCES_FOOD = value;
            if (OnVariableChange != null)
                OnVariableChange(RESOURCES_FOOD, 0);
            
        }
    }    
    public int resourceMaterial
    {
        get { return RESOURCES_MATERIAL; }
        set
        {
            RESOURCES_MATERIAL = value;
            if (OnVariableChange != null)
                OnVariableChange(RESOURCES_MATERIAL, 1);

        }
    }
    
    private bool canvasActivated
    {
        get { return currentCanvas.activeSelf;  }
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[GameManager.cs] : There is already a TrainManager Instance");
            Destroy(this);
        }
        Instance = this;
    }
    
    
    
    private void Start()
    {
        currentCanvas = missionSelectorCanvas;
        TrainStatus = TrainStatus.onMissionSelector;
        trainPanelsScript = GetComponent<TrainPanels>(); 
        
        RESOURCES_GOLD = PlayerPrefs.GetInt(RESOURCES_GOLD_NAME);
        RESOURCES_MATERIAL = PlayerPrefs.GetInt(RESOURCES_MATERIAL_NAME); 
        RESOURCES_FOOD = PlayerPrefs.GetInt(RESOURCES_FOOD_NAME); 
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleButtonPressed();
        if(!canvasActivated)
            HandleMovement();


        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
 
            resourceMaterial--;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            resourceMaterial++;
        }
        

    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //Check if we are already on top left (For now extra room)
            if (TrainStatus != TrainStatus.onExtraRoom)
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

    private void MoveTrain(bool movingToLeft)
    {
            if (movingToLeft)
            {
                LogManager.Log("MOVING TRAIN TO LEFT", FeatureType.TrainBase);
                currentIndex++;
                UpdateRoomInfo();
                trainPanelsScript.HideTrainRoom(currentIndex - 1);
                trainPanelsScript.ShowTrainRoom(currentIndex);
            }
            else
            {
                LogManager.Log("MOVING TRAIN TO RIGHT", FeatureType.TrainBase);
                currentIndex--;
                trainPanelsScript.HideTrainRoom(currentIndex + 1);
                trainPanelsScript.ShowTrainRoom(currentIndex); 
            }
            
            UpdateRoomInfo();
            train.MoveTrain(currentIndex);  
    }

    
    private void UpdateRoomInfo()
    {
        switch (currentIndex)
        {
            case 0:
                TrainStatus = TrainStatus.onMissionSelector;
                currentCanvas = missionSelectorCanvas;
                break; 
            case 1:
                TrainStatus = TrainStatus.onControlRoom;
                currentCanvas = controlRoomCanvas;
                break; 
            case 2:
                TrainStatus = TrainStatus.onExtraRoom;
                currentCanvas = extraRoomCanvas;
                break;
        }
    }
    
    private void HandleButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
          /*  switch (TrainStatus)
            {
                case TrainStatus.onMissionSelector:
                    ActivateMissionSelectorMenu(); 
                    break; 
                case TrainStatus.onControlRoom:
                    ActivateControlRoom(); 
                    break; 
                case TrainStatus.onExtraRoom:
                    ActivateExtraRoom();
                    break;
            }*/
          if (currentCanvas.activeSelf)
          {
              currentCanvas.SetActive(false);
          }
          else
          {
              currentCanvas.SetActive(true);
          }
        } 
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    
    
    
}
