using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyWagon : MonoBehaviour
{
    [SerializeField] private GameObject foodResource;
    [SerializeField] private GameObject materialResource;
    [SerializeField] private GameObject goldResource;

    private TextMeshProUGUI foodText;
    private TextMeshProUGUI materialText;
    private TextMeshProUGUI goldText;

    [SerializeField] private Button restoreButton;
    [SerializeField] private Button cancelButton;

    private int goldValue;
    private int materialValue;
    private int foodValue;
    
    private void OnEnable()
    {
        restoreButton.onClick.AddListener(() => TryToRestoreWagon());
        cancelButton.onClick.AddListener(() => CancelBuy());
        
        foodText = foodResource.GetComponentInChildren<TextMeshProUGUI>(true);
        materialText = materialResource.GetComponentInChildren<TextMeshProUGUI>(true);
        goldText = goldResource.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void OnDisable()
    {
        restoreButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public void SetUpProperties(int foodValue, int materialValue, int goldValue)
    {
        if (foodValue != 0)
        {
            Debug.Log(this.foodText);
            Debug.Log("VALUE: " + foodValue);
            this.foodResource.SetActive(true);
            this.foodText.text = foodValue.ToString();
            this.foodValue = foodValue;
        }
        if (materialValue != 0)
        {
            this.materialResource.SetActive(true);
            this.materialText.text = materialValue.ToString();
            this.materialValue = materialValue;
        }
        if (goldValue != 0)
        {
            this.goldValue = goldValue; 
            this.goldResource.SetActive(true);
            this.goldText.text = goldValue.ToString();
        }
    }

    private void CancelBuy()
    {
        Destroy(this.gameObject);
    }

    private void TryToRestoreWagon()
    {
        if (TrainManager.Instance.TryToBuyWagon(foodValue, materialValue, goldValue))
        {
            //Buy Wagon
            Destroy(this.gameObject);
            Debug.Log("BUY BUY BUY");
        }
        else
        {
            //We cant buy wagon
            Debug.Log("NO RESOURCES");
        }
    }


    
    
}
