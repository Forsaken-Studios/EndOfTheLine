using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class SeeingPlayerCondition : Node
{
    private BasicEnemyActions _enemyActions;

    public SeeingPlayerCondition(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_enemyActions.GetFOVState() == FieldOfView.FOVState.isSeeing)
        {
            SetData("isLookingForPlayer", false);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
