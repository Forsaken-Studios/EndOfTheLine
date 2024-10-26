using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class NoiseManager : MonoBehaviour
{

    public static NoiseManager Instance;

    [SerializeField] private bool activateWorldNoises = true;
    
    [Header("Min | Max time between world noises")]
    [SerializeField] private float minTimeRange = 1f;
    [SerializeField] private float maxTimeRange = 5f;

    [Header("Noise duration")] 
    [SerializeField] private float worldNoiseDurationMin;
    [SerializeField] private float worldNoiseDurationMax;

    [Header("World UI Prefab")]
    [SerializeField] private GameObject worldNoiseUIPrefab;
    private bool worldNoiseActivated = false;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("[NoiseManager] : There is already a NoiseManager");
            Destroy(this);
        }

        Instance = this;
    }

    void Start()
    {
        if (activateWorldNoises)
        {
            StartCoroutine(StartWorldNoiseCorroutine());  
        }
    }
    
    private IEnumerator StartWorldNoiseCorroutine()
    {
        while (true)
        {
            if(GameManager.Instance.GameState == GameState.OnGame)
            {
                float timeToWaitBetweenNoise = UnityEngine.Random.Range(minTimeRange, maxTimeRange);
                float worldNoiseDuration = UnityEngine.Random.Range(worldNoiseDurationMin, worldNoiseDurationMax);
                //yield return new WaitForSeconds(timeToWaitBetweenNoise); //WHEN FINISHED
                //Activate UI to show world noise
                GameObject worldNoiseUIPrefab = Instantiate(this.worldNoiseUIPrefab, Vector3.zero, Quaternion.identity);
                //Activate Sound
                AudioSource audioSource = SoundManager.Instance.ActivateSoundByName(SoundAction.WorldNoise_Start2, null, true);
                //Activate screen shake?
            
                //Reduce noise radius
                PlayerController.Instance.GetNoiseScript().UpdateColliderOnWorldNoise();
                worldNoiseActivated = true;
                yield return new WaitForSeconds(worldNoiseDuration);
            
                worldNoiseActivated = false; 
                SoundManager.Instance.StopSound();
                Destroy(worldNoiseUIPrefab);
                PlayerController.Instance.GetNoiseScript().ResetColliderOnWorldNoise();
                Destroy(audioSource.gameObject);
                SoundManager.Instance.ActivateSoundByName(SoundAction.WorldNoise_End, null, true);
                yield return new WaitForSeconds(timeToWaitBetweenNoise);
            }

            yield return null;
        }   
    }


    public bool GetIfWorldNoiseIsActivated()
    {
        return worldNoiseActivated;
    }
}
