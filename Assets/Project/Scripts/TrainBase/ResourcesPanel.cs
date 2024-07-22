using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI materialText;
    [SerializeField] private TextMeshProUGUI goldText;
   
    void Start()
    {
        //Set Up text
        foodText.text = TrainManager.Instance.resourceFood.ToString();
        materialText.text = TrainManager.Instance.resourceMaterial.ToString();
        goldText.text = TrainManager.Instance.resourceGold.ToString();

        TrainManager.Instance.OnVariableChange += VariableChanged;
    }

    /// <summary>
    /// </summary>
    /// <param name="newVal"></param>
    /// <param name="index">
    /// 0 - Food
    /// 1 - Material
    /// 2 - Gold
    /// </param>
    private void VariableChanged(int newVal, int index)
    {
        switch (index)
        {
            case 0:
                foodText.text = newVal.ToString();
                break; 
            case 1:
                materialText.text = newVal.ToString();
                break; 
            case 2:
                goldText.text = newVal.ToString();
                break;
        }
    }

}
