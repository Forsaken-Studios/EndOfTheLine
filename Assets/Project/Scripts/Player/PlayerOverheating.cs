using UnityEngine;


namespace Player
{
    public class PlayerOverheating : IPlayer_Bar
    {
            
        public static PlayerOverheating Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[PlayerStamina.cs] : There is already a PlayerStamina Instance");
                Destroy(this);
            }

            Instance = this;
        }

        public override void SetEnergy(float newStamina)
        {
            this.energy = Mathf.Clamp(newStamina, 0, MAX_STAMINA);
        }
    }
}

