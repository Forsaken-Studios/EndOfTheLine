using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        TrainStatus = TrainStatus.onMissionSelector;
    }

    // Update is called once per frame
    void Update()
    {
        LogManager.Log("TRAIN STATUS: " + TrainStatus.ToString(), FeatureType.TrainBase);
        HandleButtonPressed();
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
                break; 
            case 1:
                TrainStatus = TrainStatus.onControlRoom;
                break; 
            case 2:
                TrainStatus = TrainStatus.onExtraRoom;
                break;
        }
    }
    
    private void HandleButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (TrainStatus)
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
            }
        } 
    }
    
    private void ActivateMissionSelectorMenu()
    {
        LogManager.Log("BUTTON PRESSED ON MISSION SELECTOR", FeatureType.TrainBase);
    }   
    private void ActivateControlRoom()
    {
        LogManager.Log("BUTTON PRESSED ON CONTROL ROOM", FeatureType.TrainBase);
    }   
    private void ActivateExtraRoom()
    {
        LogManager.Log("BUTTON PRESSED ON EXTRA ROOM", FeatureType.TrainBase);
    }
}
