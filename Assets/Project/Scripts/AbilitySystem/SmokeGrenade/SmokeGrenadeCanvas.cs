using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmokeGrenadeCanvas : MonoBehaviour
{
    private AbilityHolder holder;

    [SerializeField] private Image positionImage;

    private void Update()
    {
        positionImage.rectTransform.position = GetPosition(); 
        holder.UpdatePositionToThrowAbility(GetPosition());
    }


    public void SetHolder(AbilityHolder holder)
    {
        this.holder = holder;
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
