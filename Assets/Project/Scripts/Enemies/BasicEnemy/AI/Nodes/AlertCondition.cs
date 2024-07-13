using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class AlertCondition : Node
{
    private DetectionPlayerManager _detectionPlayer;

    public AlertCondition(DetectionPlayerManager detectionPlayer)
    {
        _detectionPlayer = detectionPlayer;
    }

    public override NodeState Evaluate()
    {
        if(_detectionPlayer.isPlayerDetected == true)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
