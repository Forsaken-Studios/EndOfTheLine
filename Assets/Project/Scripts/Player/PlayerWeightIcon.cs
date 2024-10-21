using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerWeightIcon : MonoBehaviour
    {
    
        private GameObject weightIcon;
        private Image icon;
        [SerializeField] private Sprite overweightIcon;
        [SerializeField] private Sprite maxWeightIcon;

        void Start()
        {
            PlayerController.Instance.OnWeightChange += ChangeSprite;
        
            weightIcon = GameManager.Instance.GetMenuCanvas().transform.Find("WeightIcon").gameObject;
            icon = weightIcon.GetComponentInChildren<Image>();
            weightIcon.SetActive(false);
        }


        private void OnDestroy()
        {
            PlayerController.Instance.OnWeightChange -= ChangeSprite;
        }

        private void ChangeSprite(int id)
        {
            switch (id)
            {
                case 0:
                    weightIcon.SetActive(false);
                    break;        
                case 1:
                    weightIcon.SetActive(true);
                    icon.sprite = overweightIcon;
                    break;     
                case 2:
                    weightIcon.SetActive(true);
                    icon.sprite = maxWeightIcon;
                    break;
            }
        }
    }
}