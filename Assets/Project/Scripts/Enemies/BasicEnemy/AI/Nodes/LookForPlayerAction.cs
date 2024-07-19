using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayerAction : Node
{
    private BasicEnemyActions _enemyActions;
    private bool _isLookingForPlayer;
    private float _timer;

    public LookForPlayerAction(BasicEnemyActions enemyActions)
    {
        _enemyActions = enemyActions;
    }

    public override NodeState Evaluate()
    {
        object isLookingForPlayer = GetData("isLookingForPlayer");
        if(isLookingForPlayer != null)
        {
            _isLookingForPlayer = (bool)isLookingForPlayer;
        }
        else
        {
            _isLookingForPlayer = true;
            SetData("isLookingForPlayer", true);
        }

        if(_isLookingForPlayer == false)
        {
            _timer = _enemyActions.GetTimeToLookForPlayer();
            _isLookingForPlayer = true;
            SetData("isLookingForPlayer", true);
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _isLookingForPlayer = false;
            _timer = _enemyActions.GetTimeToLookForPlayer();
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
