using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{

    public new string name;
    [TextAreaAttribute(10, 10)]
    public new string description;
    public Sprite abilityIcon;
    public int abilityID;
    public float cooldownTime;
    public float activeTime;
    public float overheatCost;
    public bool needToBeReactivated;


    public int goldNeededToUnlock;
    public int materialNeededToUnlock;
    public virtual void Activate(GameObject parent, Vector2 position) {}

    public virtual void Activating(GameObject parent, Vector2 position, out GameObject gameObject)
    {
        gameObject = null;
    }

    public virtual void PrepareAbility(GameObject parent, AbilityHolder abilityHolder, out GameObject gameObject)
    {
        gameObject = null;
    }
    public virtual void BeginCooldown(GameObject parent){}
}
