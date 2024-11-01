using UnityEngine;

namespace Player
{
   public class NoiseCircle : MonoBehaviour
{

    private CircleCollider2D circleCollider;

    private float currentRadius;
    

    [Header("RADIUS")]

    [SerializeField] private bool worldNoiseIsActivated;

    [SerializeField] private GameObject player;
    
    void Start()
    { 
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        currentRadius = player.GetComponent<PlayerController>().GetCurrentRadius();
        this.circleCollider.radius = currentRadius;
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
