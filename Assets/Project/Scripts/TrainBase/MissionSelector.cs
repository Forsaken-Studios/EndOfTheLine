using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionSelector : MonoBehaviour
{
    [SerializeField] private Button playButton;
    
    private void Start()
    {
        playButton.onClick.AddListener(() => PlayMission());
    }

    private void PlayMission()
    {
        //Aqui ya tendríamos que hacer la generación o lo que sea, por ahora iniciamos test level
        SceneManager.LoadSceneAsync("Scenes/Gameplay/NivelPrototipo Tilemapped");
    }
}
