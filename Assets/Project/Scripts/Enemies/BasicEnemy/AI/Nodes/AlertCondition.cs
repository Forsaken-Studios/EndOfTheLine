using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class AlertCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public AlertCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if(_enemyActions.getIsDetected() == true)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
