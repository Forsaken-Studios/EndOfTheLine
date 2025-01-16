using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecoyGrenadeCanvas : MonoBehaviour
{
        private AbilityHolder holder;

    [SerializeField] private Image positionImage;

    private GameObject parent;
    private float radius = 4;
    
    private void Update()
    {
            //Circle
            //Get Vector from the player to cursor
            Vector2 playerToCursor = GetPosition() -  (Vector2) parent.transform.position ;

            if (playerToCursor.magnitude > radius)
            {
                //Normalize it to get the direction
                Vector2 dir = playerToCursor.normalized;
                //Multiply direction buy your desired radius
                Vector2 cursorVector = dir * radius;
                //Add the cursor vector to the player position to get the final position
                Vector2 finalPos = (Vector2) parent.transform.position + cursorVector;
                positionImage.rectTransform.position = finalPos; 
                holder.UpdatePositionToThrowAbility(finalPos, Vector2.zero);
            }
            else
            {
                positionImage.rectTransform.position = GetPosition(); 
                holder.UpdatePositionToThrowAbility(GetPosition(), Vector2.zero);
            }
        
    }


    public void SetHolder(AbilityHolder holder, GameObject parent)
    {
        this.holder = holder;
        this.parent = parent;
    }
    
    
    void OnDrawGizmos()
    {
        /*Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); //this is gray, could be anything
        Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, new Vector3(1, 1, 1));
        Gizmos.DrawSphere(Vector3.zero, currentRadius);
        Gizmos.matrix = oldMatrix;*/
        float corners = 25; // How many corners the circle should have
        float size = 15f; // How wide the circle should be
        Vector3 origin = GetPosition(); // Where the circle will be drawn around
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
    
    private Vector2 GetPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
