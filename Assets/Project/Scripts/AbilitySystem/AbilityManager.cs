using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    [Header("Abilities Properties")]
    [SerializeField] private SmokeGrenade smokeGrenade;
    [SerializeField] private WallAbility wall;
    [SerializeField] private Decoy decoyGrenade;
    
    [Header("Smoke Grenade Properties")]
    [SerializeField] private float smokeGrenadeForce = 35f;
    [SerializeField] private float smokeRadius = 14f;
    private Vector2 smokePosition;

    private bool activatedSmoke;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }


    public SmokeGrenade GetSmokeGrenadeProperties()
    {
        return smokeGrenade;
    }
    
    public WallAbility GetWallProperties()
    {
        return wall;
    }
    
    public Decoy GetDecoyGrenadeProperties()
    {
        return decoyGrenade;
    }

    public void SetSmokePosition(Vector2 position)
    {
        this.smokePosition = position;
    }

    public Vector2 GetSmokePosition()
    {
        return this.smokePosition;
    }

    public float GetGrenadeForce()
    {
        return smokeGrenadeForce;
    }
    public float GetSmokeGrenadeRadius()
    {
        return smokeRadius;
    }

    public bool GetActivatedSmoke()
    {
        return activatedSmoke;
    }

    public void SetActivatedSmoke(bool activatedSmoke)
    {
        this.activatedSmoke = activatedSmoke;
    }

}
