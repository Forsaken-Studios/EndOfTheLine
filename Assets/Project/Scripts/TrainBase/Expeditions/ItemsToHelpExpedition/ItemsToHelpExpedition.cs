using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsToHelpExpedition : MonoBehaviour
{
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private TextMeshProUGUI numberText;
    private int currentValue = 0;
    
    public static event EventHandler<int> onToolsIncreaseChanged;
    public static event EventHandler<int> onToolsDecreaseChanged;

    private int modifierChanceOfSuccess = 5;
    private int maxNumberOfTools = 0;
    void OnEnable()
    {
        maxNumberOfTools = TrainBaseInventory.Instance.GetNumberOfToolsInInventory(); //Base inventory
        maxNumberOfTools += TrainInventoryManager.Instance.GetNumberOfToolsInInventory(); //Player inventory 
        
        leftArrowButton.onClick.AddListener(() => SwapMissionToLeft());
        rightArrowButton.onClick.AddListener(() => SwapMissionToRight());
        
        MissionTypeChooser.onMissionChanged += onMissionChanged;

        if (maxNumberOfTools == 0 || ExpeditionManager.Instance.GetMission().basicChanceOfSuccess == 100)
        {
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false;
        }
    }

    private void onMissionChanged(object sender, EventArgs e)
    {
        if (ExpeditionManager.Instance.GetMission().basicChanceOfSuccess == 100)
        {
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false; 
        }
        else
        {
            leftArrowButton.interactable = true;
            rightArrowButton.interactable = true; 
        }
    }
    private void OnDisable()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
    }

    private void SwapMissionToLeft()
    {
        int newIndex = currentValue - 1;
        currentValue = (int) Mathf.Clamp(newIndex, 0, maxNumberOfTools);
        numberText.text = currentValue.ToString();
        //En vez de set up properties, es modificar las chances de succes
        onToolsDecreaseChanged?.Invoke(this, modifierChanceOfSuccess);
        BlockButtonIfFirstMission(currentValue);
    }
    
    private void SwapMissionToRight()
    {
        int newIndex = currentValue + 1;
        currentValue = Mathf.Clamp(newIndex, 0, maxNumberOfTools);
        numberText.text = currentValue.ToString();
        //En vez de set up properties, es modificar las chances de succes
        onToolsIncreaseChanged?.Invoke(this, modifierChanceOfSuccess);
        BlockButtonIfLastMission(currentValue);
    }
    
    private void BlockButtonIfFirstMission(int index)
    {
        if (index == 0)
            this.leftArrowButton.interactable = false;
        
        if (index < maxNumberOfTools)
            this.rightArrowButton.interactable = true;
    }
    private void BlockButtonIfLastMission(int index)
    {
        if (index > 0)
            this.leftArrowButton.interactable = true;
        
        if (index == maxNumberOfTools)
            this.rightArrowButton.interactable = false;
    }

}
