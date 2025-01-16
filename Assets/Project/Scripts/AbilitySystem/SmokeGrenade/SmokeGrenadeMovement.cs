using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenadeMovement : MonoBehaviour
{


    private Vector2 endPosition;
    private Rigidbody2D rigidbody2D;
    private AbilityHolder holder; 
    private bool keepMovingGrenade = true;
    [SerializeField] private GameObject smokeCollider;
    [SerializeField] private GameObject grenadeSprite;
    private AudioSource audioSource;
    
    private void OnEnable()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        smokeCollider.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SoundManager.Instance.onResumeAudios += ResumeAudios;
        SoundManager.Instance.onStopAudios += StopAudios;
    }

    private void StopAudios(object sender, EventArgs e)
    {
        if(audioSource != null)
            audioSource.Stop();
    }

    private void ResumeAudios(object sender, EventArgs e)
    {
        if(audioSource != null)
            audioSource.Play();
    }

    private void Update()
    {
        if (keepMovingGrenade)
        {
            if (Vector2.Distance(this.gameObject.transform.position, endPosition) < 0.05f ||
                rigidbody2D.velocity.magnitude <= 5f)
            {
                //Stop
                AbilityManager.Instance.SetSmokePosition(gameObject.transform.position);
                smokeCollider.GetComponent<CircleCollider2D>().radius =
                    AbilityManager.Instance.GetSmokeGrenadeRadius();
                grenadeSprite.SetActive(false); 
                smokeCollider.SetActive(true);
                AbilityManager.Instance.SetActivatedSmoke(true);
                rigidbody2D.drag = 2000f;
                audioSource.Play();
                holder.ActivateAbility();
                keepMovingGrenade = false;
            }
        }
    }
    public void SetUpProperties(Vector2 endPosition, AbilityHolder abilityHolder)
    {
        this.endPosition = endPosition;
        this.holder = abilityHolder;
    }
}
