using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Wall", order = 1)]
public class WallAbility : Ability
{

    [SerializeField] private GameObject wallCanvasPrefab;

    private GameObject canvasObject;
    private AbilityHolder holder;
    
    public override void Activating(GameObject parent, Vector2 position, out GameObject gm)
    {
        Destroy(canvasObject);
        gm = null;
    }
    
    
    public override void Activate(GameObject parent, Vector2 position)
    {
        //Instantiate wall
    }
    
    public override void PrepareAbility(GameObject parent, AbilityHolder abilityHolder, out GameObject currentCanvas)
    {
        canvasObject = Instantiate(wallCanvasPrefab, parent.transform.position, Quaternion.identity);
        canvasObject.GetComponentInChildren<WallCanvas>().SetHolder(abilityHolder, parent);
        holder = abilityHolder;
        currentCanvas = canvasObject;
    }
    public override void BeginCooldown(GameObject parent)
    {
        // Debug.Log("ABILITY " +this.name + " cooldown [Animation, or something if needed]");
    }
}
