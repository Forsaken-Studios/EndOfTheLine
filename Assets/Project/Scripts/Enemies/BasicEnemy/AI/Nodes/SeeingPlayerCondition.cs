using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class SeeingPlayerCondition : Node
{
    private DetectionPlayerManager _detectionPlayer;

    public SeeingPlayerCondition(DetectionPlayerManager detectionPlayer)
    {
        _detectionPlayer = detectionPlayer;
    }

    public override NodeState Evaluate()
    {
        if (_detectionPlayer.currentState == EnemyStates.FOVState.isSeeing)
        {
            SetData("isLookingForPlayer", false);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
