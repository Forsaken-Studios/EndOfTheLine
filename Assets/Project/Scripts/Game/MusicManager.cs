using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private AudioMixer musicMixer;

    [SerializeField] private AudioSource[] musicClips;
    [SerializeField] private string[] mixerVariables;

    // Start is called before the first frame update
    void Start()
    {
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void StartMusic()
    {
        for(int i=0; i<musicClips.Length; i++)
        {
            musicClips[i].Play();
        }
    }

    public void StartFadeFunction(int valueNum)
    {
        //Debug.Log("FADING CHANNEL " + valueNum);
        StartCoroutine(StartFade(mixerVariables[valueNum], 1.5f, 0));
    }

    public IEnumerator StartFade(String channelName, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start;
        musicMixer.GetFloat(channelName, out start);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicMixer.SetFloat(channelName, Mathf.Lerp(start, targetVolume, currentTime / duration));
            yield return null;
        }
        StopCoroutine("StartFade");
        yield break;
    }
}
