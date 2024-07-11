using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSeeingPlayerCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public NotSeeingPlayerCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_enemyActions.GetFOVState() != FieldOfView.FOVState.isSeeing)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
