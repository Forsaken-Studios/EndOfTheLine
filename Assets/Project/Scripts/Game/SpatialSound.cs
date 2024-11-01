using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialSound : MonoBehaviour
{
    AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        SoundManager.Instance.onResumeAudios += ResumeAudios;
        SoundManager.Instance.onStopAudios += StopAudios;
    }

    private void OnDestroy()
    {
        SoundManager.Instance.onResumeAudios -= ResumeAudios;
        SoundManager.Instance.onStopAudios -= StopAudios;
    }

    private void StopAudios(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void ResumeAudios(object sender, EventArgs e)
    {
        _audioSource.Play();
    }

    public void PlaySpatialSound()
    {
        _audioSource.Play();
    }
}
