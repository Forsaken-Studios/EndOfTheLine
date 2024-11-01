using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLookingForPlayerAction : Node
{
    private BasicEnemyActions _enemyActions;

    public StopLookingForPlayerAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        _enemyActions.StopRotating();
        _enemyActions.StopChasing();
        Debug.Log("-IA-: Stop look for player action");
        return NodeState.SUCCESS;
    }
}
