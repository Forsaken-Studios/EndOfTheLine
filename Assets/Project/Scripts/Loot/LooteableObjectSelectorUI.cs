using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooteableObjectSelectorUI : MonoBehaviour
{

    private int currentIndex = 0;

    
    private void Update()
    {
        Debug.Log(currentIndex);
        if (Input.GetKeyDown(KeyCode.E))
        {
            IncreaseIndex();
        }else if (Input.GetKeyDown(KeyCode.Q))
        {
           DecreaseIndex();
        }
    }
    
    public int GetCurrentIndex()
    {
        return currentIndex;
    }
    private void IncreaseIndex()
    {
        int maxValue = LooteableObjectSelector.Instance.GetLooteableObjectCount(); 
        //currentIndex = currentIndex > maxValue ? maxValue : currentIndex++;
        currentIndex++;
   
    }    
    private void DecreaseIndex()
    {
        Debug.Log("DECREASE");
        //currentIndex = currentIndex < 0 ? 0 : currentIndex--;
        currentIndex--;
    }

    private void OnEnable()
    {
        Debug.Log("ENABLE");
        currentIndex = 0;
    }

    private void OnDisable()
    {
        Debug.Log("DISABLE");
        currentIndex = 0;
    }
}
