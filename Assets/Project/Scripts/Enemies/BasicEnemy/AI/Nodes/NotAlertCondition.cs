using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAlertCondition : Node
{
    private DetectionPlayerManager _detectionPlayer;
    private BasicEnemyActions _enemyActions;

    public NotAlertCondition(DetectionPlayerManager detectionPlayer, BasicEnemyActions enemyActions)
    {
        _detectionPlayer = detectionPlayer;
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        if (_detectionPlayer.isPlayerDetected == false)
        {
            _enemyActions.timerLookForPlayer = _enemyActions.GetTimeToLookForPlayer();
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
