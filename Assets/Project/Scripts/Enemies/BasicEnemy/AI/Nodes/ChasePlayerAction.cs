using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerAction : Node
{
    private BasicEnemyActions _enemyActions;

    public ChasePlayerAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        _enemyActions.ChasePlayerLastSeenPosition();
        Debug.Log("Chase player.");
        return NodeState.SUCCESS;
    }
}
