using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Ability Test 2", order = 1)]
public class TestAbility2 : Ability
{

    public override void Activate(GameObject parent, Vector2 position)
    {
        Debug.Log("ABILITY " +this.name + " ACTIVATED");
    }
}
