using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image blackPanel;

    private Ability ability;

    private bool countdownRunning;

    public void SetUpProperties(Ability ability)
    {
        this.ability = ability;
        this.abilityIcon.sprite = ability.abilityIcon;
        blackPanel.fillAmount = 0f;
    }
    

    public void StartCooldown()
    {
        StartCoroutine(CountDown(ability.cooldownTime));
    }
    
    IEnumerator CountDown(float time){
        countdownRunning = true;
        StartCoroutine("CountDownAnimation",time);
        yield return new WaitForSeconds(time);
        if(countdownRunning)
            //do stuff
	
            countdownRunning = false;
    }
    
    IEnumerator CountDownAnimation(float time){
        float animationTime = time;
        while (animationTime > 0) {
            animationTime -= Time.deltaTime;
            blackPanel.fillAmount = animationTime/time;
            yield return null;
        }
    }
    
    
    
}
