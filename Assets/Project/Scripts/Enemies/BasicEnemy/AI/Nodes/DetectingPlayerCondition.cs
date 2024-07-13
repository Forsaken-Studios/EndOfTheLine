using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectingPlayerCondition : Node
{
    private DetectionPlayerManager _detectionPlayer;

    public DetectingPlayerCondition(DetectionPlayerManager detectionPlayer)
    {
        _detectionPlayer = detectionPlayer;
    }

    public override NodeState Evaluate()
    {
        if (_detectionPlayer.currentState == EnemyStates.FOVState.isDetecting)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
