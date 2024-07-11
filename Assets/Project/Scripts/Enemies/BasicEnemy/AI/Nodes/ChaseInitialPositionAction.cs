using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseInitialPositionAction : Node
{
    private BasicEnemyActions _enemyActions;

    public ChaseInitialPositionAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        _enemyActions.ChaseInitialPosition();
        return NodeState.SUCCESS;
    }
}
