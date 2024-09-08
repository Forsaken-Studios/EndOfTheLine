using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Ability Test 1", order = 1)]
public class TestAbility : Ability
{
    public float test;

    public override void Activate(GameObject parent, Vector2 position)
    {
        Debug.Log("ABILITY " +this.name + " ACTIVATED");
    }
    public override void BeginCooldown(GameObject parent)
    {
        Debug.Log("ABILITY " +this.name + " cooldown [Animation, or something if needed]");
    }
}
