using FieldOfView;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Utils.CustomLogs;
using static UnityEngine.GraphicsBuffer;

public class BasicEnemyActions : MonoBehaviour
{
    private EnemyVision _basicEnemyVision;
    private FieldOfView.FieldOfView _basicEnemyFOV;

    private bool _isRotating = true;
    private float rotationSpeed = 3f;
    private Vector3 _playerLastSeenPosition;
    private Vector3 _initialPosition;
    private Transform _player;
    private bool _isChasing = false;
    private Vector3 _positionChased;
    private NavMeshAgent _agent;
    private Vector3 _initialPositionSelf;
    private Vector3 _initialPositionCanvas;
    [SerializeField] private float _maxDistanceToNearEnemyPartner = 5f;
    [SerializeField] private float _killPlayerDistance = 1f;
    [SerializeField] private GameObject _CanvasObject;
    [SerializeField] private float _timeToLookForPlayer;

    public bool isAtPlayerLastSeenPosition { get; private set; }
    public bool isAtInitialPosition { get; private set; }

    void Awake()
    {
        GameEventsEnemy.OnSeeingPlayer += CheckIfFollowPlayer;
    }

    private void OnDestroy()
    {
        GameEventsEnemy.OnSeeingPlayer -= CheckIfFollowPlayer;
    }

    void Start()
    {
        _basicEnemyVision = GetComponent<EnemyVision>();
        _basicEnemyFOV = _basicEnemyVision.GetFOV();
        _initialPositionSelf = transform.position;
        _initialPositionCanvas = _CanvasObject.transform.position;

        isAtPlayerLastSeenPosition = false;
        isAtInitialPosition = true;

        _initialPosition = transform.position;

        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        StopChasing();
    }

    void Update()
    {
        UpdateCanvasPosition();
        //CheckIfKillPlayer();
        SavePlayerLastSeenPosition();
        UpdateRotationEnemy();
        CheckIsAtInitialPosition();
        CheckIsAtPlayerLastSeenPosition();

        if (_isChasing)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_positionChased);
        }
        else if (_isRotating)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateCanvasPosition()
    {
        Vector3 differenceMovement = transform.position - _initialPositionSelf;

        _CanvasObject.transform.position = _initialPositionCanvas + differenceMovement;
    }

    private void CheckIfKillPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer < _killPlayerDistance)
        {
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < _basicEnemyFOV.GetFov() / 2)
            {
                GameManager.Instance.EndGame();
            }
        }
    }

    private void SavePlayerLastSeenPosition()
    {
        if (GetFOVState() == FOVState.isSeeing)
        {
            _playerLastSeenPosition = _player.position;
            GameEventsEnemy.OnSeeingPlayer?.Invoke(transform, _player);
        }
    }

    private void UpdateRotationEnemy()
    {
        _basicEnemyFOV.SetAimDirection(transform.rotation.eulerAngles);
    }

    public void ChasePlayer()
    {
        StopRotating();
        _isChasing = true;
        _positionChased = _player.position;
    }

    public void ChasePosition(Vector3 newPosition)
    {
        StopRotating();
        _isChasing = true;
        _positionChased = newPosition;
    }

    public void ChasePlayerLastSeenPosition()
    {
        StopRotating();
        _isChasing = true;
        _positionChased = _playerLastSeenPosition;
    }

    public void ChaseInitialPosition()
    {
        StopRotating();
        _isChasing = true;
        _positionChased = _initialPosition;
    }

    private void StopChasing()
    {
        _isChasing = false;
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

    private void CheckIsAtInitialPosition()
    {
        if (Vector3.Distance(transform.position, _initialPosition) < 0.1f)
        {
            isAtInitialPosition = true;
        }
        else
        {
            isAtInitialPosition = false;
        }
    }

    private void CheckIsAtPlayerLastSeenPosition()
    {
        if (Vector3.Distance(transform.position, _playerLastSeenPosition) < 0.1f)
        {
            isAtPlayerLastSeenPosition = true;
        }
        else
        {
            isAtPlayerLastSeenPosition = false;
        }
    }

    public bool getIsDetected()
    {
        return _basicEnemyVision.PlayerDetected;
    }

    public FOVState GetFOVState()
    {
        return _basicEnemyFOV.GetFOVState();
    }

    private void CheckIfFollowPlayer(Transform enemySenderPosition, Transform playerLastSeenPosition)
    {
        float distanceToEnemySender = Vector3.Distance(transform.position, enemySenderPosition.position);

        if (distanceToEnemySender <= _maxDistanceToNearEnemyPartner)
        {
            _basicEnemyVision.PlayerDetected = true;
            _playerLastSeenPosition = playerLastSeenPosition.position;
        }
    }

    public float GetTimeToLookForPlayer()
    {
        return _timeToLookForPlayer;
    }

    void OnDrawGizmos()
    {
        if (GetFOVState() == FOVState.isSeeing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _maxDistanceToNearEnemyPartner);
        }
    }
}