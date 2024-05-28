using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   
    
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private FieldOfView enemyFOV;
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

    private void OnPlayerDetected(object sender, EventArgs e)
    {
        PlayerDetected = true;
        
    }

    private IEnumerator StartCountdownToForgetPlayer()
    {
        while (true)
        {
            enemyIsForgetting = true;
            yield return new WaitForSeconds(timeBeforeForgettingPlayer);
            //Forget player
            Debug.Log("FORGET PLAYER");
            enemyIsForgetting = false;
            detectionBar.ForgetPlayer();
            StopAllCoroutines();
            //StopCoroutine(nameof(StartCountdownToForgetPlayer));
            yield return null;
        }
    }

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
    
    public FieldOfView GetFOV()
    {
        return enemyFOV;
    }
}
