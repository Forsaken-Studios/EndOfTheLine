using System;
using System.Collections;
using System.Collections.Generic;
using Extraction;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RaidTime : MonoBehaviour
{
    [Header("UI Properties")]
    [SerializeField] private TextMeshProUGUI timerText;

    private GameObject timerGameObject;
    [SerializeField] private KeyCode keybind;
    private bool isShowingPanel = false;
    [SerializeField] private float TimeToHidePanel;
    private Animator raidTimeAnimator;
    float delayBetweenPresses = 0.25f;
    bool pressedFirstTime = false;
    float lastPressedTime;
    private float currentTime = 5f;
    // Start is called before the first frame update
    void Start()
    { 
        currentTime = ExtractionManager.Instance.GetRaidTime();
        timerGameObject = this.transform.Find("Timer").gameObject;
        timerGameObject.SetActive(true); //Si queremos lo de pulsar un boton, quitar esta linea

        raidTimeAnimator = GetComponent<Animator>();
        //StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {

        ShowTimeWhenPressingButtonTwice();
        HandleTimer();
    }

    private void HandleTimer()
    {
        if (GameManager.Instance.GameState == GameState.OnGame)
        {
            switch (currentTime)
            {
                case float a when a <= 20:
                    timerText.color = Color.red;
                    break;
                case float b when b >= 20 && b <= 120: 
                    timerText.color = Color.yellow;
                    break;
                default: 
                    timerText.color = Color.white;
                    break;
            }
            if (currentTime <= 20)
            {
                timerText.color = Color.red;
            }
            
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

    private void ShowTimeWhenPressingButtonTwice()
    {
        if (isShowingPanel)
        {
            return;
        }
        
        if (Input.GetKeyDown(keybind))
        {
            if (pressedFirstTime) // we've already pressed the button a first time, we check if the 2nd time is fast enough to be considered a double-press
            {
                bool isDoublePress = Time.time - lastPressedTime <= delayBetweenPresses;
  
                if (isDoublePress)
                {
                    StartCoroutine(HideTimerInXSeconds());
                    timerGameObject.SetActive(true);
                    pressedFirstTime = false;
                }
            }
            else // we've not already pressed the button a first time
            {
                pressedFirstTime = true; // we tell this is the first time
            }
            lastPressedTime = Time.time;
        }
 
        if (pressedFirstTime && Time.time - lastPressedTime > delayBetweenPresses) // we're waiting for a 2nd key press but we've reached the delay, we can't consider it a double press anymore
        {
            // note that by checking first for pressedFirstTime in the condition above, we make the program skip the next part of the condition if it's not true,
            // thus we're avoiding the "heavy computation" (the substraction and comparison) most of the time.
            // we're also making sure we've pressed the key a first time before doing the computation, which avoids doing the computation while lastPressedTime is still uninitialized
            pressedFirstTime = false;
        }
    }

    private IEnumerator HideTimerInXSeconds()
    {
        raidTimeAnimator.SetBool("Enable", true);
        isShowingPanel = true;
        yield return new WaitForSeconds(TimeToHidePanel);
        //We desactivate panel in animator
        raidTimeAnimator.SetBool("Enable", false);
        isShowingPanel = false;
    }

    private void HideRaidTime()
    {
        this.timerGameObject.SetActive(false);
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
