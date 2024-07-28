using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatModifier", menuName = "ScriptableObjects/Modifiers", order = 1)]
public class CharacterStatOverheatModifier : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Debug.Log("OVERHEAT MODIFIER WORKING");
    }
}
