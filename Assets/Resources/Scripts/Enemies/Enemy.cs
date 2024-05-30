using System;
using System.Collections;
using System.Collections.Generic;
using FieldOfView;
using UnityEngine;
using FieldOfView;
public class Enemy : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private FieldOfView.FieldOfView enemyFOV;
    [SerializeField] private BarDetectionProgress detectionBar;
    [SerializeField] private float timeBeforeForgettingPlayer = 3f;
    [SerializeField] private bool enemyIsForgetting = false;
    private bool _playerDetected;
    public bool PlayerDetected
    {
        get { return _playerDetected; }
        set { _playerDetected = value; }
    }
    
    private void Start()
    {
      //  detectionBar.onPlayerDetected += OnPlayerDetected;
        //enemyFOV.SetAimDirection(new Vector3(360, 360, 0));
        enemyFOV.SetOrigin(this.gameObject.transform.position);
    }

    /// <summary>
    /// When enemy stop seeing the player, we activate the timer for him to forget the player
    /// We are checking in other methods if player is being detected again. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCountdownToForgetPlayer()
    {
        while (true)
        {
            enemyIsForgetting = true;
            yield return new WaitForSeconds(timeBeforeForgettingPlayer);
            //Forget player
            enemyIsForgetting = false;
            detectionBar.ForgetPlayer();
            StopAllCoroutines();
            //StopCoroutine(nameof(StartCountdownToForgetPlayer));
            yield return null;
        }
    }

    /// <summary>
    /// I don't know why StopCoroutine by name is not working
    /// </summary>
    public void StopEnemyActionOfForgettingPlayer()
    {
        this.enemyIsForgetting = false;
        StopAllCoroutines();
        //StopCoroutine(nameof(StartCountdownToForgetPlayer));
    }
    
    
    public void ActivateCountdownToForgetPlayer()
    {
        StartCoroutine(StartCountdownToForgetPlayer());
    }

    public bool GetIfEnemyIsForgetting()
    {
        return enemyIsForgetting;
    }
    
    public FieldOfView.FieldOfView GetFOV()
    {
        return enemyFOV;
    }
}
