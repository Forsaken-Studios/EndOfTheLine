using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerSpeedBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillAmountImage;


        public void UpdateImage(float speed)
        {
            float min = PlayerController.Instance.GetMinSpeed;
            float max = PlayerController.Instance.GetMaxSpeed;
            float value = (speed - min) / (max - min);
            this.fillAmountImage.fillAmount = value;
        }
    }
}

