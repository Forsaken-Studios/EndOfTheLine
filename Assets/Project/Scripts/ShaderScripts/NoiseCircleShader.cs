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
        public float frequency = 10f;
        
        public bool simSteps = false;
        public float stepDist = 0.5f;
        private float stepDistCounter;
        [Range(0.01f, 5f)]
        public float stepDecay = 2.5f;
        
        
        
        [Range(1f, 25f)]
        public float radiusDecay = 16f;
        public GameObject player;
        
        public float runRadius = 2f;
        public float walkRadius = 0.5f;
        private float currentRadius;
        
        private Material mat;
        private float pulseAlpha=1.0f;
        private float stepAlpha=1.0f;
        
        private float playerMoveSpeed;
        private float playerWalkSpeed;
        private float playerRunSpeed;


        // Start is called before the first frame update
        void Start()
        {
            mat = GetComponent<Renderer>().material;
            currentRadius = walkRadius;
            stepDistCounter = stepDist;
        }

        // Update is called once per frame
        void Update()
        {
            playerMoveSpeed = player.GetComponent<PlayerController>().GetMoveSpeed();
            playerWalkSpeed = player.GetComponent<PlayerController>().GetWalkSpeed();
            playerRunSpeed = player.GetComponent<PlayerController>().GetRunSpeed();
            
            
            Vector3 position = player.transform.position;
            mat.SetVector("_Center", position);

            if (playerMoveSpeed >= playerRunSpeed)
            {
                currentRadius = expDecay(currentRadius, runRadius, radiusDecay, Time.deltaTime);
            }else if (playerMoveSpeed < playerWalkSpeed)
            {
                currentRadius = expDecay(currentRadius, 0, radiusDecay, Time.deltaTime);
                
            }
            else
            {
                float speedParam = (playerMoveSpeed - playerWalkSpeed) / (playerRunSpeed - playerWalkSpeed);

                float goalRadius = walkRadius + speedParam * (runRadius - walkRadius);
                
                currentRadius = expDecay(currentRadius, goalRadius, radiusDecay, Time.deltaTime);
            }
            mat.SetFloat("_Radius", currentRadius);
            
            
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

            mat.SetFloat("_Alpha", pulseAlpha*stepAlpha);

        }

        float expDecay(float current, float goal, float decay, float dT)
        {
            //Advanced LERP function
            return goal + (current - goal)* Mathf.Exp(-decay * dT);
        }
        
    }
}
