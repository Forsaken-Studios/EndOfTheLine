using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaidTime : MonoBehaviour
{
    [Header("UI Properties")]
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private float initialTime = 10;
    
    
    private float currentTime = 5f;
    // Start is called before the first frame update
    void Start()
    { 
        currentTime = initialTime;
        StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime <= 0)
        {
            timerText.text = "00:00";
            //END GAME
            GameManager.Instance.EndGame();
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
