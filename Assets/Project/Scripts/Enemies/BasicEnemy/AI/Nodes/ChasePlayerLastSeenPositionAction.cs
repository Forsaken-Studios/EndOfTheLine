using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerLastSeenPositionAction : Node
{
    private BasicEnemyActions _enemyActions;

    public ChasePlayerLastSeenPositionAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        _enemyActions.timerLookForPlayer = _enemyActions.GetTimeToLookForPlayer();
        _enemyActions.ChasePlayerLastSeenPosition();
        Debug.Log("-IA-: Chase player last seen position action");
        return NodeState.SUCCESS;
    }
}
