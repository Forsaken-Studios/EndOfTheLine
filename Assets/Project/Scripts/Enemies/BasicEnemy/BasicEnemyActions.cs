using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Utils.CustomLogs;
using static UnityEngine.GraphicsBuffer;

public class BasicEnemyActions : MonoBehaviour
{
    [Header("Adjsutable properties")]
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float _killPlayerDistance = 1f;
    [SerializeField] private float _timeToLookForPlayer = 5f;

    [Header("Patrol")]
    [SerializeField] private List<Transform> _patrolPoints;
    [SerializeField] private bool _isFullCircle;
    [SerializeField] private float _timeWaitEndPatrol = 2f;
    private float _timerWaitEndPatrol = 2f;
    private bool _isChangingPatrolPoint;
    private bool _isMovingForward = true;
    private int _newPatrolIndex = 0;

    [Header("External scripts")]
    [SerializeField] private DetectionPlayerManager _basicEnemyDetection;

    [Header("Resources")]
    [SerializeField] private GameObject _corpsePrefab;

    [Header("Animaciones")]
    private Animator _animator;

    private bool _isRotating = false;
    private Transform _player;
    private Vector3 _positionChased;
    private NavMeshAgent _agent;
    private Vector3 _initialPositionSelf;

    public bool isAtPlayerLastSeenPosition { get; private set; }
    public bool isAtInitialPosition { get; private set; }
    [HideInInspector] public float timerLookForPlayer;

    void Awake()
    {
        EnemyEvents.OnKnockDown += GetKnockedDown;
    }

    void OnDestroy()
    {
        EnemyEvents.OnKnockDown -= GetKnockedDown;
    }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        if (_patrolPoints == null)
        {
            _patrolPoints = new List<Transform>();
        }

        _isChangingPatrolPoint = true;
        timerLookForPlayer = _timeToLookForPlayer;

        _initialPositionSelf = transform.position;
        _positionChased = _initialPositionSelf;

        isAtPlayerLastSeenPosition = false;
        isAtInitialPosition = true;

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = 0;

        _player = GameObject.FindWithTag("Player").transform;
        StopChasing();
    }

    void Update()
    {
        Debug.Log($"--- Action : {_basicEnemyDetection.playerLastSeenPosition}");
        FixXYAxis();
        SetMovementSpeed();

        CheckIfKillPlayer();
        CheckIsAtInitialPosition();
        CheckIsAtPlayerLastSeenPosition();

        _agent.SetDestination(_positionChased);
        
        if (_isRotating)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        WalkAnimation();
    }

    private void WalkAnimation()
    {
        _animator.SetBool("isWalking", !_agent.isStopped);
    }

    private void SetMovementSpeed()
    {
        _agent.speed = _movementSpeed;
    }

    private void FixXYAxis()
    {
        if (_basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
        {
            // Dirección hacia el objetivo
            Vector3 direction = _player.position - transform.position;
            direction.z = 0; // Asegurarse de que la dirección esté en el plano XY

            // Calcular el ángulo en el plano XY
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Aplicar la rotación solo en el eje Z
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Mantener las rotaciones X e Y fijas en 0
            Vector3 fixedRotation = transform.rotation.eulerAngles;
            fixedRotation.x = 0;
            fixedRotation.y = 0;
            transform.rotation = Quaternion.Euler(fixedRotation);

            // Rotar el agente para seguir la dirección del movimiento
            if (_agent.velocity != Vector3.zero)
            {
                float angle = Mathf.Atan2(_agent.velocity.y, _agent.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    private void CheckIfKillPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer < _killPlayerDistance)
        {
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

            if (angleToPlayer < _basicEnemyDetection.GetFOVAngle() / 2)
            {
                Debug.Log("Jugador muerto");
                GameManager.Instance.EndGame();
            }
        }
    }

    public void ChasePlayerLastSeenPosition()
    {
        StopRotating();
        _agent.isStopped = false;
        _positionChased = _basicEnemyDetection.playerLastSeenPosition;
    }

    public void ChaseInitialPosition()
    {
        StopRotating();
        _agent.isStopped = false;
        _positionChased = _initialPositionSelf;
        EnemyEvents.OnIsAtPlayerLastSeenPosition?.Invoke(gameObject.transform.parent.gameObject, gameObject.transform.position, true);
    }

    public void StopChasing()
    {
        _agent.isStopped = true;
    }

    public void StopRotating()
    {
        _isRotating = false;
    }

    public void RotateInPlace()
    {
        StopChasing();
        _isRotating = true;
    }

    public void Patrol()
    {
        if (_patrolPoints.Count == 0)
        {
            RotateInPlace();
        }
        else
        {
            StopRotating();
        }

        _agent.isStopped = false;
        if (_isChangingPatrolPoint && _patrolPoints.Count > 0)
        {
            if (_isFullCircle)
            {
                if (_newPatrolIndex + 1 < _patrolPoints.Count)
                {
                    _newPatrolIndex += 1;
                }
                else
                {
                    _newPatrolIndex = 0;
                }
                _initialPositionSelf = _patrolPoints[_newPatrolIndex].position;
                _isChangingPatrolPoint = false;
            }
            else
            {
                if (_timerWaitEndPatrol > 0)
                {
                    _timerWaitEndPatrol -= Time.deltaTime;
                }
                else
                {
                    if (_isMovingForward)
                    {
                        if (_newPatrolIndex + 1 < _patrolPoints.Count)
                        {
                            _newPatrolIndex += 1;
                        }
                        else
                        {
                            _isMovingForward = false;
                            _timerWaitEndPatrol = _timeWaitEndPatrol;
                        }
                    }
                    else
                    {
                        if (_newPatrolIndex - 1 >= 0)
                        {
                            _newPatrolIndex -= 1;
                        }
                        else
                        {
                            _isMovingForward = true;
                            _timerWaitEndPatrol = _timeWaitEndPatrol;
                        }
                    }
                    _initialPositionSelf = _patrolPoints[_newPatrolIndex].position;
                    _isChangingPatrolPoint = false;
                }
            }
        }
    }

    private void CheckIsAtInitialPosition()
    {
        if (Vector3.Distance(transform.position, _initialPositionSelf) <= _agent.stoppingDistance + 0.15f)
        {
            isAtInitialPosition = true;

            _isChangingPatrolPoint = true;
        }
        else
        {
            isAtInitialPosition = false;
        }
    }

    private void CheckIsAtPlayerLastSeenPosition()
    {
        if (Vector3.Distance(transform.position, _basicEnemyDetection.playerLastSeenPosition) <= _agent.stoppingDistance + 0.15f)
        {
            isAtPlayerLastSeenPosition = true;
            EnemyEvents.OnIsAtPlayerLastSeenPosition?.Invoke(gameObject.transform.parent.gameObject, gameObject.transform.position, false);
        }
        else
        {
            isAtPlayerLastSeenPosition = false;
        }
    }

    public float GetTimeToLookForPlayer()
    {
        return _timeToLookForPlayer;
    }

    void OnDrawGizmos()
    {
        if (_basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _basicEnemyDetection.GetMaxDistanceToNearEnemyPartner());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            DoorUI doorUI = collision.gameObject.GetComponent<DoorUI>();
            doorUI.OpenDoorAI();
        }
    }

    private void GetKnockedDown(GameObject enemy)
    {
        if(enemy.GetInstanceID() == gameObject.GetInstanceID())
        {
            Vector3 corpsePosition = transform.position;
            if(_corpsePrefab != null)
            {
                Instantiate(_corpsePrefab, corpsePosition, Quaternion.identity);
            }
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    private void Dead()
    {
        _animator.SetBool("isDead", true);
    }
}