using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Player.PlayerController;

namespace Player
{
    public class NoiseCircleShader : MonoBehaviour
    {
        //Breathing Effect
        public bool breathing = false;
        public float frequency = 1f;
        
        //Simulate Steps
        public bool simSteps = true;
        public float stepDist = 2.5f;
        private float stepDistCounter;
        [Range(0.01f, 5f)]
        public float stepDecay = 1.5f;
        
        //Player Data From Player Singleton
        private PlayerController playerInstance;
        private GameObject player;
        private float playerMoveSpeed;
        private float currentRadius;
        
        //Material Controls
        private Material mat;
        private float pulseAlpha=1.0f;
        private float stepAlpha=1.0f;
        
        void Start()
        {
            mat = GetComponent<Renderer>().material;
            currentRadius = 0;
            stepDistCounter = stepDist;

            if (PlayerControllerInstance != null)
            {
                playerInstance = PlayerControllerInstance;
                player = playerInstance.gameObject;
            }
            else
            {
                Debug.LogWarning("[NoiseCircleShader.cs] : There is no PlayerController Instance in the Scene");
            }
        }

        void Update()
        {
            Vector3 position = player.transform.position;
            playerMoveSpeed = playerInstance.GetMoveSpeed();
            currentRadius   = playerInstance.GetCurrentRadius();
            
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
                }
            }
            else
            {
                stepAlpha = 1;
            }

            mat.SetFloat("_NCAlpha", pulseAlpha*stepAlpha);

        }

        //Advanced LERP function
        float expDecay(float current, float goal, float decay, float dT)
        {
            return goal + (current - goal)* Mathf.Exp(-decay * dT);
        }
        
    }
}
