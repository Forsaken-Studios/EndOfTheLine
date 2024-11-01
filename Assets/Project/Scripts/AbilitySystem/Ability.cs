using System.Collections;
using System.Collections.Generic;
using LootSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class Ability : ScriptableObject
{

    public new string name;
    public bool enabled;
    [TextAreaAttribute(10, 10)]
    public new string description;
    public Sprite abilityIcon;
    public int abilityID;
    public float cooldownTime;
    public float activeTime;
    public float overheatCost;
    public bool needToBeReactivated;


    public int airFilterNeededToUnlock;
    public Item material1Item;
    public int material1NeededToUnlock;
    public Item material2Item;
    public int material2NeededToUnlock;
    public virtual void Activate(GameObject parent, Vector2 position,  Vector2 endPosition) {}

    public virtual void Activating(GameObject parent, Vector2 position, Vector2 endPosition,  out GameObject gameObject)
    {
        gameObject = null;
    }

    public virtual void PrepareAbility(GameObject parent, AbilityHolder abilityHolder, out GameObject gameObject)
    {
        gameObject = null;
    }
    public virtual void BeginCooldown(GameObject parent){}
}
