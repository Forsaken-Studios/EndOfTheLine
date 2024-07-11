using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
    /// <summary>
    /// Class to control player stamina (Gas Zone)
    /// </summary>
    public class PlayerStamina : IPlayer_Bar
    {
        
        public static PlayerStamina Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[PlayerStamina.cs] : There is already a PlayerStamina Instance");
                Destroy(this);
            }

            Instance = this;
        }
        
       
        override 
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
        
    }
}