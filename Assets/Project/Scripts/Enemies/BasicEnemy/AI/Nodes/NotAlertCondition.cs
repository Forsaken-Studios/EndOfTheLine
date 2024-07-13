using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAlertCondition : Node
{
    private DetectionPlayerManager _detectionPlayer;

    public NotAlertCondition(DetectionPlayerManager detectionPlayer)
    {
        _detectionPlayer = detectionPlayer;
    }

    public override NodeState Evaluate()
    {
        if (_detectionPlayer.isPlayerDetected == false)
        {
            SetData("isLookingForPlayer", false);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
