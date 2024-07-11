using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotInInitialPositionCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public NotInInitialPositionCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if(_enemyActions.isAtInitialPosition == false)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
