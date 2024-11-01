using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI airFilterText;
   
    void Start()
    {
        
        //Set Up text
        airFilterText.text = TrainManager.Instance.resourceAirFilter.ToString();

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
                airFilterText.text = newVal.ToString();
                break; 
        }
    }

}
