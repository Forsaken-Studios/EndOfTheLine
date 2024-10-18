using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        float masterVolume = 0;
        float FXVolume = 0;
        float musicVolume = 0;
        
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20f);
        
        if (PlayerPrefs.HasKey("SoundEffectsVolume"))
        {
            FXVolume = PlayerPrefs.GetFloat("SoundEffectsVolume");
        }
        audioMixer.SetFloat("SoundEffectsVolume", Mathf.Log10(FXVolume) * 20f);
        
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20f);
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", level);
    }
    
    
    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundEffectsVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("SoundEffectsVolume", level);
    }
    
    
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", level);
    }
}
