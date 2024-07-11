using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAlertCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public NotAlertCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_enemyActions.getIsDetected() == false)
        {
            SetData("isLookingForPlayer", false);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
