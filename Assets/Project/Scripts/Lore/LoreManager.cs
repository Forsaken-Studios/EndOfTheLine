using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance;


    [SerializeField] private GameObject pickupLorePrefab;
    [SerializeField] private GameObject expandedLoreViewPrefab;

    [SerializeField] private bool playerIsReadingLore = false;
    private GameObject currentLoreActive;
    private int currentNumberOfLorePages = 3;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[LoreManager.cs] : There is already a LoreManager Instance");
            Destroy(this);
        }
        Instance = this;
    }
    
    public GameObject GetPickUpLorePrefab()
    {
        return pickupLorePrefab;
    }   
    public GameObject GetExpandedLoreViewPrefab()
    {
        return expandedLoreViewPrefab;
    }

    public bool GetIfPlayerIsReadingLore()
    {
        return playerIsReadingLore;
    }

    public void SetIfPlayerIsReadingLore(bool aux)
    {
        this.playerIsReadingLore = aux;
    }

    public void SetCurrentLoreView(GameObject aux)
    {
        this.currentLoreActive = aux;
    }

    public void DestroyCurrentExpandedView()
    {
        Destroy(this.currentLoreActive);
    }

    public int GetCurrentNumberOfLorePages()
    {
        return currentNumberOfLorePages;
    }
}
