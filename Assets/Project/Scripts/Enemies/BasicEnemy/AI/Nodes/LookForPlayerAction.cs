using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayerAction : Node
{
    private BasicEnemyActions _enemyActions;

    public LookForPlayerAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("-IA-: LookForPlayerAction action");

        _enemyActions.timerLookForPlayer -= Time.deltaTime;
        if (_enemyActions.timerLookForPlayer <= 0)
        {
            _enemyActions.timerLookForPlayer = _enemyActions.GetTimeToLookForPlayer();
            EnemyEvents.OnForgetPlayer?.Invoke();
            return NodeState.SUCCESS;
        }
        else
        {
            _enemyActions.RotateInPlace();
            return NodeState.RUNNING;
        }
    }
}
