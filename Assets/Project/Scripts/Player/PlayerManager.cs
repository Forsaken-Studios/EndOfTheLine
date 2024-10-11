using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {

        private bool isPlayerInGasZone = false;
        private bool isPlayerInExtractionPoint = false;

        private PlayerStamina playerStamina;


        private void Awake()
        {
            this.playerStamina = GetComponent<PlayerStamina>();
        }

        public void PlayerEnteredGasZone()
        {
            isPlayerInGasZone = true;
            this.playerStamina.ActivateEnergyDecreasing();
        }

        public void PlayerExitedGasZone()
        {
            isPlayerInGasZone = false;
            this.playerStamina.DesactivateEnergyDecreasing();
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