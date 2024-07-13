using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class DoorUI : MonoBehaviour
{
    [SerializeField] private Sprite haveKeycardSprite; 
    [SerializeField] private Sprite notKeycardSprite;
    [SerializeField] private SpriteRenderer keycardRenderer;
    private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Sprite doorOpenedSprite;
    private Collider2D doorCollider;
     private bool playerCanTryToOpenTheDoor = false;
     private bool doorOpened;
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

    public void SwapKeycardIcon(bool playerIsFarAway)
    {
        bool playerHasKey = PlayerInventory.Instance.CheckIfPlayerHasKey(); 
        keycardRenderer.sprite = playerHasKey ? haveKeycardSprite : notKeycardSprite;
        playerCanTryToOpenTheDoor = !playerIsFarAway && playerHasKey;
        doorCollider = GetComponent<Collider2D>();
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
                    
                    doorSpriteRenderer.sprite = doorOpenedSprite;
                    doorCollider.enabled = false;
                    doorOpened = !doorOpened;
                    
                }
            }
        }
    }

    public void OpenDoorAI()
    {
        //Cambiar sprite
        doorOpened = true;
        //En este caso sería animator en vez de cambiar sprite
        doorSpriteRenderer.sprite = doorOpenedSprite;
        doorCollider.enabled = false;
        StartCoroutine(CloseDoorInXTime(4f));
    }

    public void CloseDoorAI()
    {
        //Cambiar sprite
        doorOpened = false;
        //En este caso sería animator
        
        doorCollider.enabled = true;  
    }

    private IEnumerator CloseDoorInXTime(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            CloseDoorAI();
            StopAllCoroutines();
        }

        yield return null;
    }

}
