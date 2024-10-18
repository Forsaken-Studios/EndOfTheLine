using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuyWagon : MonoBehaviour
{
    [FormerlySerializedAs("foodResource")] [SerializeField] private GameObject material1Resource;
    [FormerlySerializedAs("materialResource")] [SerializeField] private GameObject material2Resource;
    [FormerlySerializedAs("goldResource")] [SerializeField] private GameObject airFilterResource;

    private TextMeshProUGUI foodText;
    private TextMeshProUGUI materialText;
    private TextMeshProUGUI goldText;

    [SerializeField] private Button restoreButton;
    [SerializeField] private Button cancelButton;

    private int airFilterValue;
    private int material1Value;
    private int material2Value;
    
    private void OnEnable()
    {
        restoreButton.onClick.AddListener(() => TryToRestoreWagon());
        cancelButton.onClick.AddListener(() => CancelBuy());
        
        foodText = material1Resource.GetComponentInChildren<TextMeshProUGUI>(true);
        materialText = material2Resource.GetComponentInChildren<TextMeshProUGUI>(true);
        goldText = airFilterResource.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void OnDisable()
    {
        restoreButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public void SetUpProperties(int material1Value, int material2Value, int airFilterValue)
    {
        if (material1Value != 0)
        {
            this.material1Resource.SetActive(true);
            this.foodText.text = material1Value.ToString();
            this.material2Value = material1Value;
        }
        if (material2Value != 0)
        {
            this.material2Resource.SetActive(true);
            this.materialText.text = material2Value.ToString();
            this.material1Value = material2Value;
        }
        if (airFilterValue != 0)
        {
            this.airFilterValue = airFilterValue; 
            this.airFilterResource.SetActive(true);
            this.goldText.text = airFilterValue.ToString();
        }
    }

    private void CancelBuy()
    {
        Destroy(this.gameObject);
        TrainManager.Instance.SetIsShowingWagonBuyUI(false);
    }

    private void TryToRestoreWagon()
    {
        if (TrainManager.Instance.TryToBuyWagon(airFilterValue, material1Value, material2Value))
        {
            //Buy Wagon
            Destroy(this.gameObject);
        }
        else
        {
            //We cant buy wagon
            Debug.Log("NO RESOURCES");
        }
    }


    
    
}
