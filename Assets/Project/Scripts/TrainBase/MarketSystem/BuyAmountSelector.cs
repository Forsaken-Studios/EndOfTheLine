using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyAmountSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxText;
    [SerializeField] private TextMeshProUGUI currentAmount;
    private TMP_InputField inputField;
    private Slider slider;
    private Button splitButton;
    
    private MarketSlot itemSelected;

    private void Start()
    {
        splitButton = GetComponentInChildren<Button>(true);
        slider = GetComponentInChildren<Slider>(true);
        inputField = GetComponentInChildren<TMP_InputField>(true);
        inputField.onValueChanged.AddListener(delegate { InputFieldChanged(); });
        splitButton.onClick.AddListener(() => Buy());
        this.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
        currentAmount.text = slider.value.ToString();
    }

    private void InputFieldChanged()
    {
        
        string value = inputField.text;
        int intValue = Convert.ToInt32(value);
        if (intValue > slider.maxValue)
            inputField.text = slider.maxValue.ToString();
        else if(intValue < slider.minValue)
            inputField.text = slider.minValue.ToString();
        
        slider.value = intValue;
        currentAmount.text = value;
    }
    public void SetUpProperties(int maxAmount, MarketSlot itemSelected)
    {
        Debug.Log("SET UP");
        this.itemSelected = itemSelected;
        this.maxText.text = maxAmount.ToString();
        Debug.Log(slider);
        slider.maxValue = maxAmount;
        slider.value = maxAmount / 2;
        inputField.text = slider.value.ToString();
        inputField.text = slider.value.ToString();
        currentAmount.text = maxAmount.ToString();
    }

    
    private void Buy()
    {
        Debug.Log("SPLITTING: " + slider.value);
        this.gameObject.SetActive(false);
        if (slider.value != 0)
        {
            if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(itemSelected.GetItemSO(), (int) slider.value,
                    out int remainingItemsWithoutSpace))
            {          
                
                //MarketSystem.Instance.RemoveItemFromList(itemSelected.GetItemSO(), (int) slider.value);
                //TODO: SPEND MONEY POR AHORA SE HA DEPRECADO EL SELECTOR AMOUNT
                if (slider.maxValue == slider.value)
                {
                    itemSelected.ClearMarketSlot(); 
                }
                else
                {
                    itemSelected.RemoveAmountFromSlot((int) slider.value);  
                }

                SaveManager.Instance.SaveCurrentDayStoreJson();
            }
            else
            {
                Debug.Log("NO SPACE FOR ITEM");
            }
            this.gameObject.SetActive(false);
        }
        //AND THEN WE SPLIT
    }
}
