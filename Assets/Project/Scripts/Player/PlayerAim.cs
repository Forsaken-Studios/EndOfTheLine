using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAim : MonoBehaviour
    {
        //Not sure if it is better to use serializefield or transform.find
        private Transform aimTransform;


        private void Update()
        {
            if (GameManager.Instance.GameState == GameState.OnGame)
            {
                HandleAim();
            }
        }

        private void HandleAim()
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            //In 2D, we only want to rotate over Z axis
            //X Y AIM DIRECTION VALUE
            Vector3 aimDirection = (mousePosition - transform.position).normalized; 
            //AIM DIRECTION -> EULER ANGLE
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            this.transform.eulerAngles = new Vector3(0, 0, angle);
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
}