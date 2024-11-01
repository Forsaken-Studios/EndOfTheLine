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
            this.fillAmountImage.fillAmount = speed;
        }
    }
}

