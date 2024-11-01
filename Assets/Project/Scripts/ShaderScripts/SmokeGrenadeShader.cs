using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenadeShader : MonoBehaviour
{
    private bool activated = false;
    private bool finished = false;
    private Vector2 grenadeLocation;

    private float smokeRadius = 2f;
    private float currentRadius;
    private float smokeDuration = 4f;
    [Range(0.01f, 25f)]
    [SerializeField] private float smokeSpawnDecay = 2.5f;
    [Range(0.01f, 25f)]
    [SerializeField] private float smokeFadeDecay = 12f;
    
    private float timer = 0f;
    
    private float grenadeAlpha = 0f;
    
    private Material mat;

    private Vector2 currentOffset;
    private Vector2 initialOffset;
    [SerializeField] private bool smokeVortexEffectEnabled = false;
    [SerializeField] private float smokeVortexEffect = 1.5f;
    
    [SerializeField] private bool smokeRotationEffectEnabled = false;
    [SerializeField] private float smokeRotationEffect = 1.5f;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        currentRadius = smokeRadius;
        initialOffset = mat.GetTextureOffset("_SmokeTex");
        currentOffset = initialOffset;
        smokeRadius = AbilityManager.Instance.GetSmokeGrenadeRadius();
        smokeDuration = AbilityManager.Instance.GetSmokeGrenadeProperties().activeTime;
        grenadeLocation = AbilityManager.Instance.GetSmokePosition();


    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = grenadeLocation;
        mat.SetVector("_SGCenter", position);
        mat.SetFloat("_SGRadius", currentRadius);
        if (smokeRotationEffectEnabled)
        {
            mat.SetFloat("_SGRotation", 1f);
            mat.SetFloat("_SGSpeedX", smokeRotationEffect);
        }
        else
        {
            mat.SetFloat("_SGRotation", 0f);
        }

        activated = AbilityManager.Instance.GetActivatedSmoke();
        
        if (activated)
        {
            grenadeLocation = AbilityManager.Instance.GetSmokePosition();
            if (finished)
            {
                grenadeAlpha = expDecay(grenadeAlpha, 0, smokeFadeDecay, Time.deltaTime);
                if (smokeVortexEffectEnabled)
                {
                    currentOffset.y = expDecay(currentOffset.y, smokeVortexEffect, smokeFadeDecay, Time.deltaTime);
                }
                
                
                if (grenadeAlpha < 0.05f)
                {
                    AbilityManager.Instance.SetActivatedSmoke(false);
                    finished = false;
                    currentOffset = initialOffset;
                }
            }
            else
            {
                timer -= Time.deltaTime;
                grenadeAlpha = expDecay(grenadeAlpha, 1, smokeSpawnDecay, Time.deltaTime);
                if (smokeVortexEffectEnabled)
                {
                    currentOffset.y = expDecay(currentOffset.y, 0, smokeSpawnDecay, Time.deltaTime);
                }
                
                if (timer <= 0)
                {
                    finished = true;
                }
            }
        }
        else
        {
            timer = smokeDuration;
            currentOffset.y = smokeVortexEffect;
            grenadeAlpha = 0f;
        }
        mat.SetFloat("_SGAlpha",grenadeAlpha);
        mat.SetTextureOffset("_SmokeTex",currentOffset);
        
    }
    
    float expDecay(float current, float goal, float decay, float dT)
    {
        //Advanced LERP function
        return goal + (current - goal)* Mathf.Exp(-decay * dT);
    }
}
