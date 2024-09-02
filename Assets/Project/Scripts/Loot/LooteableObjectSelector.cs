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

    [SerializeField] private GameObject lootSelectorPrefab;
    private GameObject lootSelectorGameObject;
    private LooteableObjectSelectorUI looteableObjectSelectorUI;
    private void Start()
    {
        looteableObjectInRange = new List<LooteableObject>();
      
    }
    

    public void AddOneInTrigger(LooteableObject loot)
    {
        looteableObjectInRange.Add(loot);
        if (looteableObjectInRange.Count > 1)
        {
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
            HideItemSelector();
        }
    }
    
    private void ShowItemSelector()
    {
        if(lootSelectorGameObject)
            Destroy(lootSelectorGameObject);
        lootSelectorGameObject = Instantiate(lootSelectorPrefab,
        looteableObjectInRange[0].gameObject.transform.position, Quaternion.identity);
        looteableObjectSelectorUI = lootSelectorGameObject.GetComponent<LooteableObjectSelectorUI>();
        
    } 
    private void HideItemSelector()
    {
        if (lootSelectorGameObject != null)
        {
            Destroy(lootSelectorGameObject);
            looteableObjectSelectorUI = null;
        }
    }

    public int GetLooteableObjectCount()
    {
        return looteableObjectInRange.Count;
    }

    public List<LooteableObject> GetLootList()
    {
        return looteableObjectInRange;
    }
    public bool GetIfSelectorIsActive()
    {
        return looteableObjectInRange.Count > 1;
    }
}
