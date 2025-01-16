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

        private bool canRotateAim = true;

        [Header("Player 2D Model")]
        [SerializeField] private GameObject playerTorso;
        [SerializeField] private GameObject playerTriangle;

        private void Update()
        {
            if (GameManager.Instance.GameState == GameState.OnGame && canRotateAim)
            {
                HandleAim();
            }
        }

        private void HandleAim()
        {
            if(!PlayerController.Instance.isRunning)
            {
                Vector3 mousePosition = GetMouseWorldPosition();
                //In 2D, we only want to rotate over Z axis
                //X Y AIM DIRECTION VALUE
                Vector3 aimDirection = (mousePosition - transform.position).normalized;
                //Vector3 aimDirection = (mousePosition - playerTorso.transform.position).normalized; 
                //AIM DIRECTION -> EULER ANGLE
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                //this.transform.eulerAngles = new Vector3(0, 0, angle);
                playerTorso.transform.eulerAngles = new Vector3(0, 0, angle - 90f);
            }
        }


        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public void SetIfCanRotateAim(bool canRotate)
        {
            canRotateAim = canRotate;
        }

        public void RemoveTriangle()
        {
            // Ver si da problemas cuando se mezcle todo
            // Transform triangle = PlayerController.Instance.transform.Find("PlayerAim/Triangle").transform;
            // triangle.gameObject.SetActive(false);
            playerTriangle.SetActive(false);
            GameObject legsModel = PlayerController.Instance.transform.Find("Legs").gameObject;
            legsModel.SetActive(false);
        }

        private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}