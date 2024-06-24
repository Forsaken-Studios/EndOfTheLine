using System;
using System.Collections;
using System.Collections.Generic;
using Loot;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LooteableObjectSelectorUI : MonoBehaviour
{

    private int currentIndex = 0;

    [SerializeField] private List<Button> optionsAvailable;
    
    private void Update()
    {
        EventSystem.current.SetSelectedGameObject(optionsAvailable[currentIndex].gameObject);
        Debug.Log(currentIndex);
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            IncreaseIndex();
        }else if (Input.GetAxis("Mouse ScrollWheel") > 0)
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
        int maxValue = LooteableObjectSelector.Instance.GetLooteableObjectCount() - 1; 
        //currentIndex = currentIndex > maxValue ? maxValue : currentIndex++;
        currentIndex++;
        if (currentIndex > maxValue)
            currentIndex = maxValue;
        EventSystem.current.SetSelectedGameObject(optionsAvailable[currentIndex].gameObject);

    }    
    private void DecreaseIndex()
    {
        Debug.Log("DECREASE");
        //currentIndex = currentIndex < 0 ? 0 : currentIndex--;
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = 0;
        EventSystem.current.SetSelectedGameObject(optionsAvailable[currentIndex].gameObject);
    }

    private void OnEnable()
    {
        Debug.Log("ENABLE");
        SetUpProperties();
        this.transform.position = LooteableObjectSelector.Instance.GetLootList()[0].transform.position;
        currentIndex = 0;
    }

    private void SetUpProperties()
    {
        List<LooteableObject> auxList = LooteableObjectSelector.Instance.GetLootList();
        for (int i = 0; i < auxList.Count; i++)
        {
            optionsAvailable[i].GetComponentInChildren<TextMeshProUGUI>().text = SwapNames(auxList[i].name);
        }
    }

    private string SwapNames(string originalName)
    {
        switch (originalName)
        {
            case "TemporalBox(Clone)":
                return "Temporal Crate";
                break;
            case "LooteableObject[ToolBox]":
                return "Tool Box"; 
                break; 
        }

        return "Undefined"; 
    }
    
    private void OnDisable()
    {
        Debug.Log("DISABLE");
        currentIndex = 0;
    }
}
