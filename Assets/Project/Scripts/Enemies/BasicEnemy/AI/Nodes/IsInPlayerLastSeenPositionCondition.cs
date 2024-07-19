using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInPlayerLastSeenPositionCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public IsInPlayerLastSeenPositionCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_enemyActions.isAtPlayerLastSeenPosition == true)
        {
            Debug.Log("is in player last seen position");
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
