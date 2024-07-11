using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InInitialPositionCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public InInitialPositionCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_enemyActions.isAtInitialPosition == true)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
