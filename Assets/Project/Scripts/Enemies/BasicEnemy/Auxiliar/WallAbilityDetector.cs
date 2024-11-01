using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbilityDetector : MonoBehaviour
{
    private BasicEnemyActions _basicEnemyActions;

    private void Start()
    {
        _basicEnemyActions = GetComponentInParent<BasicEnemyActions>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("WallAbility"))
        {
            _basicEnemyActions.IsNearWallAbility = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("WallAbility"))
        {
            
            _basicEnemyActions.IsNearWallAbility = false;
        }
    }
}
