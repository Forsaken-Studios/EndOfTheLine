using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ability/Decoy", order = 1)]
public class Decoy : Ability
{
    public float force = 140f;
    public float decoyRadius = 14f;
    [SerializeField] private GameObject decoyGrenadeCanvasPrefab;
    [SerializeField] private GameObject decoyGrenadePrefab;
    private GameObject canvasObject;
    private DecoyGrenadeMovement decoyGrenadeScript;
    private Vector2 offset = new Vector2(2, 4);
    private AbilityHolder holder;


    public override void Activating(GameObject parent, Vector2 position, Vector2 endPosition,  out GameObject gm)
    {
        //instantiate smoke grenade and push it to place
        Destroy(canvasObject);
        Vector2 dir = (position - (Vector2) parent.transform.position).normalized;
        Vector2 playerPosition = (Vector2) parent.transform.position;
        Vector2 spawnPosition = (playerPosition + offset);
        GameObject decoyGrenade = Instantiate(decoyGrenadePrefab, playerPosition, Quaternion.identity);
        Rigidbody2D rb = decoyGrenade.GetComponentInChildren<Rigidbody2D>();
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        gm = decoyGrenade;
        decoyGrenadeScript = decoyGrenade.GetComponentInChildren<DecoyGrenadeMovement>();
        decoyGrenadeScript.SetUpProperties(position, holder);
    }
    public override void Activate(GameObject parent, Vector2 position, Vector2 endPosition)
    {
        decoyGrenadeScript.ActivateNoiseCircle();
    }
    public override void PrepareAbility(GameObject parent, AbilityHolder abilityHolder, out GameObject currentCanvas)
    {
        canvasObject = Instantiate(decoyGrenadeCanvasPrefab, parent.transform.position, Quaternion.identity);
        canvasObject.GetComponentInChildren<DecoyGrenadeCanvas>().SetHolder(abilityHolder, parent);
        holder = abilityHolder;
        currentCanvas = canvasObject;
    }
    public override void BeginCooldown(GameObject parent)
    {
        // Debug.Log("ABILITY " +this.name + " cooldown [Animation, or something if needed]");
    }
}
