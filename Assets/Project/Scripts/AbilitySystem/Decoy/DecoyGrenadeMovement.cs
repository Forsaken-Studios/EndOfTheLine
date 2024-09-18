using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DecoyGrenadeMovement : MonoBehaviour
{
    private Vector2 endPosition;
    private Rigidbody2D rigidbody2D;
    private AbilityHolder holder;
    private bool keepMovingGrenade = true;
    [SerializeField] private GameObject smokeCollider;
    private void OnEnable()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        smokeCollider.SetActive(false);
    }

    private void Update()
    {
        if (keepMovingGrenade)
        {
            if (Vector2.Distance(this.gameObject.transform.position, endPosition) < 2 ||
                rigidbody2D.velocity.magnitude <= 5f)
            {
                //Stop
                
                Debug.Log("STOP GRENADE");
                //rigidbody2D.MovePosition(this.gameObject.transform.position);
                rigidbody2D.drag = 2000f;
                keepMovingGrenade = false;
            }
        }
    }

    public void ActivateNoiseCircle()
    {
        if(smokeCollider != null)
            smokeCollider.SetActive(true);
    }


    
    public void SetUpProperties(Vector2 endPosition, AbilityHolder abilityHolder)
    {
        this.endPosition = endPosition;
        this.holder = abilityHolder;
    }
}
