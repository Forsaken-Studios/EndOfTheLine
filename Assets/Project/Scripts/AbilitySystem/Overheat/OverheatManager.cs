using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class OverheatManager : IPlayer_Bar
{
    public static OverheatManager Instance;
    
    [SerializeField] private AbilityHolder holder1;
    [SerializeField] private AbilityHolder holder2;
    
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
    
    public AbilityHolder GetHolder1()
    {
        return holder1;
    }

    public void SetHolderToPrepareAbility(int holder)
    {
        if (holder == 1)
        {
            holder2.TryToCancelAbility();
        }
        else
        {
            holder1.TryToCancelAbility();
        }
    }
    

    public AbilityHolder GetHolder2()
    {
        return holder2;
    }
}
