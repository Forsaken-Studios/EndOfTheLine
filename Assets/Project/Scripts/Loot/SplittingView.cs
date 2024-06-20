using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SplittingView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxText;
    [SerializeField] private TextMeshProUGUI currentAmount;
    private TMP_InputField inputField;
    private Slider slider;
    private Button splitButton;
  
    
    private DraggableItem draggableItem;
    private ItemSlot finalItemSlot;
    private ItemSlot previousItemSlot;
    private void Start()
    {
        
        splitButton = GetComponentInChildren<Button>();
        slider = GetComponentInChildren<Slider>();
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.onValueChanged.AddListener(delegate { InputFieldChanged(); });
        splitButton.onClick.AddListener(() => Split());
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
    public void SetUpProperties(int maxAmount, DraggableItem draggableItem, ItemSlot itemSlot, ItemSlot previousItemSlot)
    {
        this.draggableItem = draggableItem;
        this.finalItemSlot = itemSlot;
        this.previousItemSlot = previousItemSlot; 
        this.maxText.text = maxAmount.ToString();
        
        slider.maxValue = maxAmount;
        slider.value = maxAmount / 2;
        currentAmount.text = maxAmount.ToString();
    }
    private void Split()
    {
        Debug.Log("SPLITTING: " + slider.value);
        this.gameObject.SetActive(false);
        if(slider.value != 0)
             this.finalItemSlot.SwapItemsBetweenSlots(draggableItem, previousItemSlot, (int) slider.value);
        
        //AND THEN WE SPLIT
    }

}
