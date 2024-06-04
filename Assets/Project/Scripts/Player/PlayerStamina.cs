using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public static PlayerStamina Instance;
    
    
    [SerializeField] private Image staminaBar;

    private int MAX_STAMINA;
    [SerializeField] private float stamina;

    [SerializeField] private bool canRecoverEnergy;
    [Header("Stamina properties")]
    [SerializeField] private float recoveryStaminaSpeed = 5f;
    [SerializeField] private float recoveryStaminaTime = 0.2f;
    [SerializeField] private float recoveryStaminaTimeLerp = 0.2f;

    [Header("Gas Zone Properties")]
    [SerializeField] private float gasZoneStaminaDecrease = 5;
    
    [SerializeField] private float barLerpSpeed;
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
    
   private IEnumerator IncreaseStaminaOnTime()
   {
       //canRecoverEnergy = false;
       while (true)
       {
           if (stamina != MAX_STAMINA && canRecoverEnergy)
           {
               Debug.Log("CORROUTINE INCREASE");
               IncreaseStamina(recoveryStaminaSpeed);
               yield return new WaitForSeconds(recoveryStaminaTime);
           }

           yield return null;
       }
   }
   
   private IEnumerator DecreaseStaminaOverTime()
   {
       //canRecoverEnergy = false;
       while (true)
       {
           if (stamina >= 0.01)
           {
               Debug.Log("CORROUTINE DECREASE");
               DecreaseStamina(gasZoneStaminaDecrease);
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

    public void SetStamina(float newStamina)
    {
        this.stamina = Mathf.Clamp(newStamina, 0, MAX_STAMINA);
        if (this.stamina == 0)
        {
            //Game Over
            this.gameObject.GetComponent<PlayerController>().SetIfPlayerCanMove(false);
            this.staminaBar.fillAmount = 0;
            GameManager.Instance.EndGame();
        }
       // SetEnergyBar(GetEnergy());
    }

    public void SetCanRecoveryEnergy(bool aux)
    {
        this.canRecoverEnergy = aux;
    }
}
