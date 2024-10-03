using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Wall", order = 1)]
public class WallAbility : Ability
{

    [SerializeField] private GameObject wallCanvasPrefab;
    [SerializeField] private GameObject wallPlacerPrefab;
    private GameObject canvasObject;
    private AbilityHolder holder;
    private GameObject wallSquare;
    public override void Activating(GameObject parent, Vector2 position, Vector2 endPosition, out GameObject gm)
    {
        Destroy(canvasObject);
        //Instantiate placer in walls
        GameObject placers = Instantiate(wallPlacerPrefab, Vector2.zero, Quaternion.identity);
        placers.GetComponent<WallPlacers>().SetLocations(position, endPosition, out wallSquare);
        gm = placers;
    }
    public override void Activate(GameObject parent, Vector2 position, Vector2 endPosition)
    {
        //Instantiate wall
        wallSquare.SetActive(true);
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
