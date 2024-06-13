using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.CustomLogs;

public class TrainManager : MonoBehaviour
{
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

    private bool canvasActivated
    {
        get { return currentCanvas.activeSelf;  }
    }
    private void Start()
    {
        currentCanvas = missionSelectorCanvas;
        TrainStatus = TrainStatus.onMissionSelector;
        trainPanelsScript = GetComponent<TrainPanels>(); 
    }

    // Update is called once per frame
    void Update()
    {
        LogManager.Log("TRAIN STATUS: " + TrainStatus.ToString(), FeatureType.TrainBase);
        HandleButtonPressed();
        if(!canvasActivated)
            HandleMovement();
    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //Check if we are already on top left (For now extra room)
            if (TrainStatus != TrainStatus.onExtraRoom)
            {
                MoveTrainToLeft();
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        { 
            //Check if we are already on top right
            if (TrainStatus != TrainStatus.onMissionSelector)
            {
                MoveTrainToRight();
            }
        }
    }

    private void MoveTrainToLeft()
    {
      
        if (train.CanMove)
        { 
            //TODO: Modify in which room are we.
            currentIndex++;
            UpdateRoomInfo();
            trainPanelsScript.HideTrainRoom(currentIndex - 1);
            trainPanelsScript.ShowTrainRoom(currentIndex);
            LogManager.Log("MOVING TRAIN TO LEFT", FeatureType.TrainBase);
            train.MoveTrainToLeft();  
        }
    }
    
    private void MoveTrainToRight()
    {
        if (train.CanMove)
        {
            currentIndex--;
            UpdateRoomInfo();
            trainPanelsScript.HideTrainRoom(currentIndex + 1);
            trainPanelsScript.ShowTrainRoom(currentIndex);
            LogManager.Log("MOVING TRAIN TO RIGHT", FeatureType.TrainBase);
            train.MoveTrainToRight();
        }
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
