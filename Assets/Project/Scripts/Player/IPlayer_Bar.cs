using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class IPlayer_Bar : MonoBehaviour
{
    [FormerlySerializedAs("staminaBar")] [SerializeField] protected Image statusBar;

    protected int MAX_STAMINA;
    [FormerlySerializedAs("stamina")] [SerializeField] protected float energy;

    [SerializeField] private bool canRecoverEnergy;

    [FormerlySerializedAs("recoveryStaminaSpeed")]
    [Header("Stamina properties")] 
    [SerializeField] private float recoveryEnergySpeed = 5f;
    [FormerlySerializedAs("recoveryStaminaTime")] [SerializeField] private float recoveryEnergyTime = 0.2f;
    [FormerlySerializedAs("recoveryStaminaTimeLerp")] [SerializeField] private float recoveryEnergyTimeLerp = 0.2f;

    [HideInInspector]
    [Header("Gas Zone Properties")] [SerializeField]
    private float valueStaminaDecrease = 5;
    [HideInInspector]
    [Header("Bar Lerp Speed")]
    [SerializeField] private float barLerpSpeed;
    
    public virtual void Start()
    {
        statusBar.fillAmount = 1.0f;
        MAX_STAMINA = 100;
        energy = MAX_STAMINA;
        StartCoroutine(IncreaseEnergyOnTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState == GameState.OnGame)
        {
            if ((energy / 100) != statusBar.fillAmount)
            {
                this.statusBar.fillAmount = Mathf.Lerp(this.statusBar.fillAmount, GetEnergy() / 100,
                    recoveryEnergyTimeLerp);
                if (this.statusBar.fillAmount >= 0.98)
                {
                    this.statusBar.fillAmount = 1;
                }
            }
        }
    }
    
    protected IEnumerator IncreaseEnergyOnTime()
    {
        //canRecoverEnergy = false;
        while (true)
        {
            if (energy != MAX_STAMINA && canRecoverEnergy)
            {
                IncreaseEnergy(recoveryEnergySpeed);
                yield return new WaitForSeconds(recoveryEnergyTime);
            }

            yield return null;
        }
    }

    protected IEnumerator DecreaseEnergyOverTime()
    {
        //canRecoverEnergy = false;
        while (true)
        {
            if (energy >= 0.01)
            {
                DecreaseEnergy(valueStaminaDecrease);
                yield return new WaitForSeconds(recoveryEnergyTime);
            }

            yield return null;
        }
    }    
    
    public void ActivateEnergyDecreasing()
    {
        StopAllCoroutines();
        //StopCoroutine(IncreaseStaminaOnTime());
        StartCoroutine(DecreaseEnergyOverTime());
    }

    public void DesactivateEnergyDecreasing()
    {
        StopAllCoroutines();
        //StopCoroutine(DecreaseStaminaOverTime());
        if (gameObject.activeSelf)
        {
            StartCoroutine(IncreaseEnergyOnTime());  
        }
        canRecoverEnergy = true;
    }
    
    public void IncreaseEnergy(float energyAmount)
    {
        SetEnergy(energyAmount + GetEnergy());
    }

    public void DecreaseEnergy(float energyAmount)
    {
        SetEnergy(GetEnergy() - energyAmount);
    }
    public float GetEnergy()
    {
        return energy;
    }

    public abstract void SetEnergy(float newStamina);
    

    public void SetCanRecoveryEnergy(bool aux)
    {
        this.canRecoverEnergy = aux;
    }
}
