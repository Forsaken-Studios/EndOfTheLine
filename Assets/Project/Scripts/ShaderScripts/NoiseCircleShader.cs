using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class NoiseCircleShader : MonoBehaviour
    {
        public bool breathing = false;
        public float frequency = 1f;
        
        public bool simSteps = true;
        public float stepDist = 2.5f;
        private float stepDistCounter;
        [Range(0.01f, 5f)]
        public float stepDecay = 1.5f;
        
        public GameObject player;
        
        private float currentRadius;
        
        private Material mat;
        private float pulseAlpha=1.0f;
        private float stepAlpha=1.0f;
        
        private float playerMoveSpeed;


        // Start is called before the first frame update
        void Start()
        {
            mat = GetComponent<Renderer>().material;
            currentRadius = 0;
            stepDistCounter = stepDist;
        }

        // Update is called once per frame
        void Update()
        {
            playerMoveSpeed = player.GetComponent<PlayerController>().GetMoveSpeed();
            currentRadius = player.GetComponent<PlayerController>().GetCurrentRadius();
            
            Vector3 position = player.transform.position;
            mat.SetVector("_NCCenter", position);
            mat.SetFloat("_NCRadius", currentRadius);
            
            if (breathing)
            {
                pulseAlpha = 0.5f * (1 + Mathf.Sin((2f * Mathf.PI * frequency * Time.time)));
                //mat.SetFloat("_Alpha", pulseAlpha);
            }
            else
            {
                pulseAlpha = 1;
            }

            if (simSteps)
            {
                stepDistCounter -= (Time.deltaTime * playerMoveSpeed);
                stepAlpha = expDecay(stepAlpha, 0, stepDecay, Time.deltaTime);
                if (stepDistCounter <= 0)
                {
                    stepDistCounter += stepDist;
                    stepAlpha = 1;
                    //mat.SetFloat("_Alpha", stepAlpha);
                }
            }
            else
            {
                stepAlpha = 1;
            }

            mat.SetFloat("_NCAlpha", pulseAlpha*stepAlpha);

        }

        float expDecay(float current, float goal, float decay, float dT)
        {
            //Advanced LERP function
            return goal + (current - goal)* Mathf.Exp(-decay * dT);
        }
        
    }
}
