using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
    public abstract class IPlayer_Bar : MonoBehaviour
    {
        [SerializeField] protected Image statusBar;

        protected int MAX_STAMINA; 
        [SerializeField] protected float energy;

        [SerializeField] private bool canRecoverEnergy;
        [Header("Energy properties")] 
        [Tooltip("Amount that increase [Increase Bar]")]
        [SerializeField] private float increaseEnergySpeed = 5f;
        [Tooltip("Time between waits")]
        [SerializeField] private float TimeCooldownBetweenRecoveries = 0.2f;
        [Tooltip("Decrease[Bar]")]
        [SerializeField]private float valueEnergyDecrease = 5;

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
                    this.statusBar.fillAmount = GetEnergy() / 100;
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
                    IncreaseEnergy(increaseEnergySpeed);
                    yield return new WaitForSeconds(TimeCooldownBetweenRecoveries);
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
                    DecreaseEnergy(valueEnergyDecrease);
                    yield return new WaitForSeconds(TimeCooldownBetweenRecoveries);
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
}