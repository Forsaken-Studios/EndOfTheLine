using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingAction : Node
{
    private BasicEnemyActions _enemyActions;

    public PatrollingAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("-IA-: Patrolling action");
        _enemyActions.Patrol();
        return NodeState.SUCCESS;
    }
}
