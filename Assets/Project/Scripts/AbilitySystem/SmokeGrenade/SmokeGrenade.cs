using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Smoke Grenade", order = 1)]
public class SmokeGrenade : Ability
{
    public float force = 35f;
    public float smokeRadius = 14f;
    [SerializeField] private GameObject smokeGrenadeCanvasPrefab;
    [SerializeField] private GameObject smokeGrenadePrefab;
    private GameObject canvasObject;
    private Vector2 offset = new Vector2(2, 4);
    private AbilityHolder holder;


    public override void Activating(GameObject parent, Vector2 position, Vector2 endPosition,  out GameObject gm)
    {
        //instantiate smoke grenade and push it to place
        Destroy(canvasObject);
        Vector2 dir = (position - (Vector2) parent.transform.position).normalized;
        Vector2 playerPosition = (Vector2) parent.transform.position;
        Vector2 spawnPosition = (playerPosition + offset);
        GameObject smokeGrenade = Instantiate(smokeGrenadePrefab, playerPosition, Quaternion.identity);
        Rigidbody2D rb = smokeGrenade.GetComponentInChildren<Rigidbody2D>();
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        gm = smokeGrenade;
        smokeGrenade.GetComponentInChildren<SmokeGrenadeMovement>().SetUpProperties(position, holder);
    }
    public override void Activate(GameObject parent, Vector2 position, Vector2 endPosition)
    {
        //Instantiate smoke
        
        Debug.Log("SMOKE SMOKE SMOKE");
    }
    public override void PrepareAbility(GameObject parent, AbilityHolder abilityHolder, out GameObject currentCanvas)
    {
        canvasObject = Instantiate(smokeGrenadeCanvasPrefab, parent.transform.position, Quaternion.identity);
        canvasObject.GetComponentInChildren<SmokeGrenadeCanvas>().SetHolder(abilityHolder, parent);
        holder = abilityHolder;
        currentCanvas = canvasObject;
    }
    public override void BeginCooldown(GameObject parent)
    {
       // Debug.Log("ABILITY " +this.name + " cooldown [Animation, or something if needed]");
    }
}
