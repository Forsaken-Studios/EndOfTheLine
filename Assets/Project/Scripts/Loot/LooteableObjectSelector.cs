using System;
using System.Collections;
using System.Collections.Generic;
using Loot;
using UnityEngine;

public class LooteableObjectSelector : MonoBehaviour
{
    public static LooteableObjectSelector Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LooteableObjectSelector! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    //TEST 

    private List<LooteableObject> looteableObjectInRange;

    [SerializeField] private GameObject lootSelectorGameObject;
    private LooteableObjectSelectorUI looteableObjectSelectorUI;
    private void Start()
    {
        looteableObjectInRange = new List<LooteableObject>();
        looteableObjectSelectorUI = lootSelectorGameObject.GetComponent<LooteableObjectSelectorUI>();
    }
    

    public void AddOneInTrigger(LooteableObject loot)
    {
        looteableObjectInRange.Add(loot);
        if (looteableObjectInRange.Count > 1)
        {
            //TODO: GameObject not made yet
            ShowItemSelector();
        }
    }

    public bool GetIfIndexIsThisLooteableObject(LooteableObject looteableObject)
    {
        return looteableObjectInRange[looteableObjectSelectorUI.GetCurrentIndex()].Equals(looteableObject); 
    }
    
    public void DecreaseOneInTrigger(LooteableObject loot)
    {
        looteableObjectInRange.Remove(loot);
        if (looteableObjectInRange.Count < 2)
        {
            //TODO: GameObject not made yet
            HideItemSelector();
        }
    }
    
    private void ShowItemSelector()
    {
        lootSelectorGameObject.SetActive(true);
    } 
    private void HideItemSelector()
    {
        lootSelectorGameObject.SetActive(false);
    }

    public int GetLooteableObjectCount()
    {
        return looteableObjectInRange.Count;
    }

    public bool GetIfSelectorIsActive()
    {
        return looteableObjectInRange.Count > 1;
    }
}
