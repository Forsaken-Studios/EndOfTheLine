using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NoiseCircle : MonoBehaviour
{

    private CircleCollider2D circleCollider;

    private float currentRadius;
    private float normalRadius = 27;

    [Header("RADIUS")]
    [SerializeField] private float DASH_RADIUS;
    [SerializeField] private float DASH_SPRINT;
    
    void Start()
    { 
        circleCollider = GetComponent<CircleCollider2D>();
        UpdateColliderRadius(27);
    }

    private void Update()
    {
       
    }

    void OnDrawGizmos()
    {
            /*Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); //this is gray, could be anything
            Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, new Vector3(1, 1, 1));
            Gizmos.DrawSphere(Vector3.zero, currentRadius);
            Gizmos.matrix = oldMatrix;*/
            float corners = 25; // How many corners the circle should have
            float size = currentRadius; // How wide the circle should be
            Vector3 origin = transform.position; // Where the circle will be drawn around
            Vector3 startRotation = transform.right * size; // Where the first point of the circle starts
            Vector3 lastPosition = origin + startRotation;
            float angle = 0;
            while (angle <= 360)
            {
                angle += 360 / corners;
                Vector3 nextPosition = origin + (Quaternion.Euler(0, 0, angle) * startRotation);
                Gizmos.DrawLine(lastPosition, nextPosition);
                Gizmos.DrawSphere(nextPosition, 1);

                lastPosition = nextPosition;
            }     
    }
    public void UpdateColliderRadius(float radius)
    {
        this.circleCollider.radius = radius;
        currentRadius = radius;
    }

    public void UpdateColliderOnDash()
    {
        UpdateColliderRadius(DASH_RADIUS);
    }

    public void UpdateColliderOnSprint()
    {
        UpdateColliderRadius(DASH_SPRINT);
    }

    public void ResetColliderRadius()
    {
        this.circleCollider.radius = normalRadius;
        currentRadius = normalRadius;
    }
}
