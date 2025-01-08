using UnityEngine;

namespace Player
{
    public class NoiseCircle : MonoBehaviour
    {

        private CircleCollider2D circleCollider;
        private PlayerController playerController;

        private float currentRadius;


        [Header("RADIUS")]

        [SerializeField] private bool worldNoiseIsActivated;

        void Start()
        {
            playerController = GetComponentInParent<PlayerController>();
            circleCollider = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            currentRadius = PlayerController.Instance.GetCurrentRadius();
            this.circleCollider.radius = currentRadius;

            if (playerController.GetMoveSpeed() <= 0 || NoiseManager.Instance.GetIfWorldNoiseIsActivated())
            {
                circleCollider.enabled = false;
            }
            else
            {
                circleCollider.enabled = true;
            }
        }

        public void UpdateColliderOnWorldNoise()
        {
            worldNoiseIsActivated = true;
        }

        public void ResetColliderOnWorldNoise()
        {
            worldNoiseIsActivated = false;
        }

    }

}
