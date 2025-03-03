using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {

        private bool isPlayerInGasZone = false;
        private bool isPlayerInExtractionPoint = false;

        private PlayerRadiation playerStamina;


        private void Awake()
        {
            this.playerStamina = GetComponent<PlayerRadiation>();
        }

        public void PlayerEnteredGasZone()
        {
            isPlayerInGasZone = true;
            this.playerStamina.ActivateEnergyDecreasing();
        }

        public void PlayerExitedGasZone()
        {
            if (!GameManager.Instance.playerIsDead)
            {
                isPlayerInGasZone = false;
                this.playerStamina.DesactivateEnergyDecreasing();
            }

        }
        
        #region Getters Setters

        public void SetIfPlayerIsInGasZone(bool aux)
        {
            this.isPlayerInGasZone = aux;
        }

        public bool GetIfPlayerIsInGasZone()
        {
            return isPlayerInGasZone;
        }

        public void SetIfPlayerInExtractionPoint(bool aux)
        {
            this.isPlayerInExtractionPoint = aux;
        }

        public bool GetIfPlayerInExtractionPoint()
        {
            return isPlayerInExtractionPoint;
        }

        #endregion
    }
}