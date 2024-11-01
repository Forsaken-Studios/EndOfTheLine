using System;
using System.Collections;
using System.Collections.Generic;
using Extraction;
using TMPro;
using UnityEngine;

public class RaidTime : MonoBehaviour
{
    [Header("UI Properties")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    private float currentTime = 5f;
    // Start is called before the first frame update
    void Start()
    { 
        currentTime = ExtractionManager.Instance.GetRaidTime();
        //StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState == GameState.OnGame)
        {
            if (currentTime <= 0)
            {
                timerText.text = "0:00";
                StopAllCoroutines();
                //END GAME
                GameManager.Instance.EndGame();
            }
            else
            {
                currentTime -= Time.deltaTime;
                TimeSpan t = TimeSpan.FromSeconds(currentTime);
                string s = "";
                if (t.Seconds < 10)
                {
                    s  = string.Format("{0}:0{1}", t.Minutes, t.Seconds); 
                }
                else
                {
                    s= string.Format("{0}:{1}", t.Minutes, t.Seconds); 
                }
                timerText.text = s;    
            } 
        }
    }
    
    private IEnumerator StartCountdown()
    {
        while (true)
        {
            currentTime -= Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(currentTime);
            string s = "";
            if (t.Seconds < 10)
            {
               s  = string.Format("{0}:0{1}", t.Minutes, t.Seconds); 
            }
            else
            {
                s= string.Format("{0}:{1}", t.Minutes, t.Seconds); 
            }
            timerText.text = s;      
            yield return null;
        }
    }
}
