using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmokeGrenadeCanvas : MonoBehaviour
{
    private AbilityHolder holder;

    [SerializeField] private Image positionImage;

    private GameObject parent;
    
    private float radius = 30;
    private float radiusX = 30;
    private float radiusY = 20;
    
    private float minX = -50;
    private float maxX = -50;
    private float minY = -50;
    private float maxY = -50;
    
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
                holder.UpdatePositionToThrowAbility(finalPos);
            }
            else
            {
                positionImage.rectTransform.position = GetPosition(); 
                holder.UpdatePositionToThrowAbility(GetPosition());
            }
            
        /*
             Square
            Vector2 position = new Vector2();
            minX = parent.transform.position.x - radiusX;
            maxX = parent.transform.position.x + radiusX;
            minY = parent.transform.position.y - radiusY;
            maxY = parent.transform.position.y + radiusY;
        
            position.x = Mathf.Clamp(GetPosition().x, minX, maxX);
            position.y = Mathf.Clamp(GetPosition().y, minY, maxY);
            Debug.Log(position);*/
        
    }


    public void SetHolder(AbilityHolder holder, GameObject parent)
    {
        this.holder = holder;
        this.parent = parent;
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
