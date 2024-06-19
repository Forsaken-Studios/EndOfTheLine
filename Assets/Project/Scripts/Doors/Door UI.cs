using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class DoorUI : MonoBehaviour
{
    [SerializeField] private Sprite haveKeycardSprite; 
    [SerializeField] private Sprite notKeycardSprite;
    [SerializeField] private SpriteRenderer keycardRenderer;
    private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Sprite doorOpened;
     private bool playerCanTryToOpenTheDoor = false;
    // Start is called before the first frame update
    void Start()
    {
        doorSpriteRenderer = GetComponent<SpriteRenderer>();
        keycardRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleOpeningDoor();
    }

    public void SwapKeycardIcon()
    {
        bool playerHasKey = PlayerInventory.Instance.CheckIfPlayerHasKey(); 
        keycardRenderer.sprite = playerHasKey ? haveKeycardSprite : notKeycardSprite;
        playerCanTryToOpenTheDoor = playerHasKey; 
    }

    private void HandleOpeningDoor()
    {
        if (playerCanTryToOpenTheDoor)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (PlayerInventory.Instance.CheckIfPlayerHasKey())
                {
                    Debug.Log("Open Door");
                    doorSpriteRenderer.sprite = doorOpened;
                }
            }
        }
    }
    
    

}
