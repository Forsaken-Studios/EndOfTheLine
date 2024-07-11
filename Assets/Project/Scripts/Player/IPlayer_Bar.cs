using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class IPlayer_Bar : MonoBehaviour
{
    [SerializeField] protected Image staminaBar;

    protected int MAX_STAMINA;
    [SerializeField] protected float stamina;

    [SerializeField] private bool canRecoverEnergy;

    [Header("Stamina properties")] 
    [SerializeField] private float recoveryStaminaSpeed = 5f;
    [SerializeField] private float recoveryStaminaTime = 0.2f;
    [SerializeField] private float recoveryStaminaTimeLerp = 0.2f;

    [Header("Gas Zone Properties")] [SerializeField]
    private float valueStaminaDecrease = 5;

    [Header("Bar Lerp Speed")]
    [SerializeField] private float barLerpSpeed;
    
    private void Start()
    {
        staminaBar.fillAmount = 1.0f;
        MAX_STAMINA = 100;
        stamina = MAX_STAMINA;
        StartCoroutine(IncreaseStaminaOnTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState == GameState.OnGame)
        {
            if ((stamina / 100) != staminaBar.fillAmount)
            {
                this.staminaBar.fillAmount = Mathf.Lerp(this.staminaBar.fillAmount, GetStamina() / 100,
                    recoveryStaminaTimeLerp);
                if (this.staminaBar.fillAmount >= 0.98)
                {
                    this.staminaBar.fillAmount = 1;
                }
            }
        }
    }
    
    protected IEnumerator IncreaseStaminaOnTime()
    {
        //canRecoverEnergy = false;
        while (true)
        {
            if (stamina != MAX_STAMINA && canRecoverEnergy)
            {
                IncreaseStamina(recoveryStaminaSpeed);
                yield return new WaitForSeconds(recoveryStaminaTime);
            }

            yield return null;
        }
    }

    protected IEnumerator DecreaseStaminaOverTime()
    {
        //canRecoverEnergy = false;
        while (true)
        {
            if (stamina >= 0.01)
            {
                DecreaseStamina(valueStaminaDecrease);
                yield return new WaitForSeconds(recoveryStaminaTime);
            }

            yield return null;
        }
    }    
    
    public void ActivateStaminaDecreasing()
    {
        StopAllCoroutines();
        //StopCoroutine(IncreaseStaminaOnTime());
        StartCoroutine(DecreaseStaminaOverTime());
    }

    public void DesactivateStaminaDecreasing()
    {
        StopAllCoroutines();
        //StopCoroutine(DecreaseStaminaOverTime());
        StartCoroutine(IncreaseStaminaOnTime());
        canRecoverEnergy = true;
    }
    
    public void IncreaseStamina(float energyAmount)
    {
        SetStamina(energyAmount + GetStamina());
    }

    public void DecreaseStamina(float energyAmount)
    {
        SetStamina(GetStamina() - energyAmount);
    }
    public float GetStamina()
    {
        return stamina;
    }

    public abstract void SetStamina(float newStamina);
    

    public void SetCanRecoveryEnergy(bool aux)
    {
        this.canRecoverEnergy = aux;
    }
}
