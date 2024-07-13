using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSeeingPlayerCondition : Node
{
    private DetectionPlayerManager _detectionPlayer;

    public NotSeeingPlayerCondition(BasicEnemyActions enemyActions, DetectionPlayerManager detectionPlayer)
    {
        _detectionPlayer = detectionPlayer;
    }

    public override NodeState Evaluate()
    {
        if (_detectionPlayer.currentState != EnemyStates.FOVState.isSeeing)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
