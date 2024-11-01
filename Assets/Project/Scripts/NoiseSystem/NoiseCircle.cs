using UnityEngine;

namespace Player
{
   public class NoiseCircle : MonoBehaviour
{

    private CircleCollider2D circleCollider;

    private float currentRadius;
    

    [Header("RADIUS")]

    [SerializeField] private bool worldNoiseIsActivated;
    
    void Start()
    { 
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        currentRadius = PlayerController.Instance.GetCurrentRadius();
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
