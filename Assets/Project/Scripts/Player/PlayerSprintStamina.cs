using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSprintStamina : MonoBehaviour
{

    [SerializeField] private float staminaReducer;
    private float staminaDivider = 1000f;
    [SerializeField] private float staminaRecoveryValue;
    [SerializeField] private Image staminaFillImage;

    private float currentStamina;
    public float CurrentStamina => currentStamina;
    private bool isSprinting => PlayerController.Instance.IsSprinting;
            
    public static PlayerSprintStamina Instance;
    private bool staminaDrained = false;
    private bool alreadyWaiting = false;
    private bool playerCanSprint = true;
    [SerializeField] private float timeForCooldown;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[PlayerStamina.cs] : There is already a PlayerStamina Instance");
            Destroy(this);
        }

        Instance = this;
    }

    private void Start()
    {
        currentStamina = 1f;
        staminaReducer /= staminaDivider;
        staminaRecoveryValue /= staminaDivider;
    }

    public bool PlayerCanSprint()
    {
        return playerCanSprint;
    }

    private void Update()
    {
        if (!staminaDrained)
        {
            if (isSprinting && !InventoryManager.Instance.inventoryIsOpen)
            {
                currentStamina -= staminaRecoveryValue;
                staminaFillImage.fillAmount -= staminaRecoveryValue;

                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    playerCanSprint = false;
                    if(!alreadyWaiting)
                        StartCoroutine(StartCooldown());
                }
            }
            else
            {
                if (staminaFillImage.fillAmount == 1)
                {
                    return;
                }
                currentStamina += staminaRecoveryValue;
                staminaFillImage.fillAmount += staminaRecoveryValue;
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        alreadyWaiting = true;
        playerCanSprint = false;
        staminaDrained = true;
        yield return new WaitForSeconds(timeForCooldown);
        staminaDrained = false;
        alreadyWaiting = false;

        while (currentStamina <= 0.4f)
        {
            yield return null;
        }

        playerCanSprint = true;
        
    }
}
