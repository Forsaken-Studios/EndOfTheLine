using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatManager : IPlayer_Bar
{
    public static OverheatManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[OverheatManager] : There is already a overheat manager");
            Destroy(this);
        }

        Instance = this;
    }
     
    public override void Start()
    {
        statusBar.fillAmount = 0f;
        MAX_STAMINA = 100;
        energy = 0;
        StartCoroutine(DecreaseEnergyOverTime());
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newStamina"></param>
    public override void SetEnergy(float newStamina)
    {
        this.energy = Mathf.Clamp(newStamina, 0, MAX_STAMINA);
        
        //here we control if we reach max overheat (if something happens)
    }

    public bool CheckIfWeCanThrowAbility(float cost)
    {
        return energy + cost <= MAX_STAMINA;
    }

    
}
