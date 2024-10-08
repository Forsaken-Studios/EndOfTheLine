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
    [SerializeField] private GameObject optionPrefab;
    private List<Button> optionsAvailable;


    private void LateUpdate()
    {
        EventSystem.current.SetSelectedGameObject(optionsAvailable[currentIndex].gameObject);
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
        //currentIndex = currentIndex < 0 ? 0 : currentIndex--;
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = 0;
        EventSystem.current.SetSelectedGameObject(optionsAvailable[currentIndex].gameObject);
    }

    private void OnEnable()
    {
        optionsAvailable = new List<Button>();
        SetUpProperties();
        this.transform.position = LooteableObjectSelector.Instance.GetLootList()[0].transform.position;
        currentIndex = 0;
    }

    private void SetUpProperties()
    {
        List<LooteableObject> auxList = LooteableObjectSelector.Instance.GetLootList();
        GridLayoutGroup gridLayoutParent = this.GetComponentInChildren<GridLayoutGroup>();
        for (int i = 0; i < auxList.Count; i++)
        {
            GameObject option = Instantiate(optionPrefab, this.transform.position, Quaternion.identity, gridLayoutParent.gameObject.transform);
            Button optionButton = option.GetComponent<Button>();
            optionsAvailable.Add(optionButton);
            option.GetComponentInChildren<TextMeshProUGUI>().text = SwapNames(auxList[i].name);
        }
    }

    private string SwapNames(string originalName)
    {
        if (originalName.Contains("LooteableObject[ToolBox]"))
        {
            originalName = "Tool Box";
        }
        
        switch (originalName)
        {
            case "TemporalBoxPrefab(Clone)":
                return "Temporal Crate";
                break;
            case "Tool Box":
                return "Tool Box"; 
                break;  
            case "Crate":
                return "Crate"; 
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
