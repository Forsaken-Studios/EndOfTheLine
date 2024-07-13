using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotInPlayerLastSeenPositionCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public NotInPlayerLastSeenPositionCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_enemyActions.isAtPlayerLastSeenPosition == false)
        {
            SetData("isLookingForPlayer", false);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
