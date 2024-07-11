using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(BasicEnemyActions))]
public class BasicEnemyAI : BTree
{
    private BasicEnemyActions _enemyActions;

    protected override void Start()
    {
        _enemyActions = GetComponent<BasicEnemyActions>();
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override Node SetUpTree()
    {
        // Enemigo en Alerta
        Node alertSequence = new NodeSequence(new List<Node>
        {
            new AlertCondition(_enemyActions),
            new NodeSelector(new List<Node>
            {
                new NodeSequence(new List<Node>
                {
                    new SeeingPlayerCondition(_enemyActions),
                    new ChasePlayerAction(_enemyActions)
                }),
                new NodeSequence(new List<Node>
                {
                    new NotSeeingPlayerCondition(_enemyActions),
                    new NodeSelector(new List<Node>
                    {
                        new NodeSequence(new List<Node>
                        {
                            new NotInPlayerLastSeenPositionCondition(_enemyActions),
                            new ChasePlayerLastSeenPositionAction(_enemyActions)
                        }),
                        new NodeSequence(new List<Node>
                        {
                            new IsInPlayerLastSeenPositionCondition(_enemyActions),
                            new LookForPlayerAction(_enemyActions)
                        })
                    })
                })
            })
        });

        // Enemigo Idle
        Node idleSequence = new NodeSequence(new List<Node>
        {
            new NotAlertCondition(_enemyActions),
            new NodeSelector(new List<Node>
            {
                new NodeSequence(new List<Node>
                {
                    new SeeingPlayerCondition(_enemyActions),
                    new StopLookingForPlayerAction(_enemyActions)
                }),
                new NodeSequence(new List<Node>
                {
                    new NotInInitialPositionCondition(_enemyActions),
                    new ChaseInitialPositionAction(_enemyActions)
                }),
                new NodeSequence(new List<Node>
                {
                    new InInitialPositionCondition(_enemyActions),
                    new PatrollingAction(_enemyActions)
                })
            })
        });

        // Enemigo Base
        NodeSelector root = new NodeSelector(new List<Node>
        {
            alertSequence,
            idleSequence
        });

        return root;
    }
}
