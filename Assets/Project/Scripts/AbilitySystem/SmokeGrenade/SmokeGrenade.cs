using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Smoke Grenade", order = 1)]
public class SmokeGrenade : Ability
{
    public float force = 15f;
    public float smokeRadius = 14f;
    [SerializeField] private GameObject smokeGrenadeCanvasPrefab;
    [SerializeField] private GameObject smokeGrenadePrefab;
    private GameObject canvasObject;
    private Vector2 offset = new Vector2(2, 4);
    private AbilityHolder holder;

    public override void Activating(GameObject parent, Vector2 position)
    {
        //instantiate smoke grenade and push it to place
        Destroy(canvasObject);
        Vector2 dir = (position - (Vector2) parent.transform.position).normalized;
        Vector2 playerPosition = (Vector2) parent.transform.position;
        Vector2 spawnPosition = (playerPosition + offset);
        GameObject smokeGrenade = Instantiate(smokeGrenadePrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = smokeGrenade.GetComponentInChildren<Rigidbody2D>();
        rb.AddForce(dir * 15f, ForceMode2D.Impulse); 
        smokeGrenade.GetComponentInChildren<SmokeGrenadeMovement>().SetUpProperties(position, holder);
    }
    public override void Activate(GameObject parent, Vector2 position)
    {
        //Instantiate smoke
    }
    public override void PrepareAbility(GameObject parent, AbilityHolder abilityHolder)
    {
        canvasObject = Instantiate(smokeGrenadeCanvasPrefab, parent.transform.position, Quaternion.identity);
        canvasObject.GetComponentInChildren<SmokeGrenadeCanvas>().SetHolder(abilityHolder);
        holder = abilityHolder;

    }
    public override void BeginCooldown(GameObject parent)
    {
       // Debug.Log("ABILITY " +this.name + " cooldown [Animation, or something if needed]");
    }
}
