using System.Collections;
using System.Collections.Generic;
using LootSystem;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyActions : MonoBehaviour
{
    [Header("Adjsutable properties")]
    [SerializeField] private float _usualMovementSpeed = 1f;
    [SerializeField] private float _chasingMovementSpeed = 2f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float _killPlayerDistance = 0.75f;
    [SerializeField] private float _timeToLookForPlayer = 5f;

    [Header("Patrol")]
    [SerializeField] private bool _isFullCircle;
    [SerializeField] private float _timeWaitEndPatrol = 2f;
    private List<Transform> _patrolPoints;
    private float _timerWaitEndPatrol = 2f;
    private bool _isChangingPatrolPoint;
    private bool _isMovingForward = true;
    private int _newPatrolIndex = 0;

    [Header("External scripts")]
    [SerializeField] private DetectionPlayerManager _basicEnemyDetection;

    [Header("Animaciones")]
    private Animator _animator;

    private bool _isRotating = false;
    private Transform _player;
    private Vector3 _positionChased;
    private NavMeshAgent _agent;
    private Vector3 _initialPositionSelf;
    private bool _isDead = false;
    public bool IsNearWallAbility = false;
    public bool isInQTE = false;

    public bool isAtPlayerLastSeenPosition { get; private set; }
    public bool isAtInitialPosition { get; private set; }
    [HideInInspector] public float timerLookForPlayer;

    private void Awake()
    {
        EnemyEvents.OnDeactivateNMAgent += StopChasing;
        EnemyEvents.OnActivateNMAgent += ActivateAgent;
        EnemyEvents.OnIsOnQTE += ActivateIsInQTE;
    }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        InitializePatrolPoints();

        _isChangingPatrolPoint = true;
        timerLookForPlayer = _timeToLookForPlayer;

        _initialPositionSelf = transform.position;
        _positionChased = _initialPositionSelf;

        isAtPlayerLastSeenPosition = false;
        isAtInitialPosition = true;

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = 0;
        SetMovementSpeed(_usualMovementSpeed);

        _player = GameObject.FindWithTag("Player").transform;

        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            StopChasing();
        }
    }

    void Update()
    {
        if (!(_agent.isActiveAndEnabled && _agent.isOnNavMesh))
        {
            return;
        }

        if (_isDead)
        {
            return;
        }

        if (IsNearWallAbility)
        {
            return;
        }

        FixXYAxis();

        CheckIfKillPlayer();
        CheckIsAtInitialPosition();
        CheckIsAtPlayerLastSeenPosition();

        _agent.SetDestination(_positionChased);

        if (_isRotating)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        if (!IsNearWallAbility)
        {
            WalkAnimation();
        }
    }

    private void InitializePatrolPoints()
    {
        _patrolPoints = new List<Transform>();
        Transform patrolPointsParent = gameObject.transform.parent.Find("PatrolPoints");
        for (int i = 0; i < patrolPointsParent.childCount; i++)
        {
            _patrolPoints.Add(patrolPointsParent.GetChild(i));
        }
    }

    private void WalkAnimation()
    {
        if (isInQTE)
        {
            _animator.SetBool("isWalking", false);
            return;
        }

        _animator.SetBool("isWalking", !_agent.isStopped);
    }

    private void SetMovementSpeed(float movementSpeed)
    {
        _agent.speed = movementSpeed;
    }

    private void FixXYAxis()
    {
        if (_basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
        {
            // Direcci�n hacia el objetivo
            Vector3 direction = _player.position - transform.position;
            direction.z = 0; // Asegurarse de que la direcci�n est� en el plano XY

            // Calcular el �ngulo en el plano XY
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Aplicar la rotaci�n solo en el eje Z
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Mantener las rotaciones X e Y fijas en 0
            Vector3 fixedRotation = transform.rotation.eulerAngles;
            fixedRotation.x = 0;
            fixedRotation.y = 0;
            transform.rotation = Quaternion.Euler(fixedRotation);

            // Rotar el agente para seguir la direcci�n del movimiento
            if (_agent.velocity != Vector3.zero)
            {
                float angle = Mathf.Atan2(_agent.velocity.y, _agent.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    private void CheckIfKillPlayer()
    {
        if (isInQTE)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer < _killPlayerDistance)
        {
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

            if (angleToPlayer < _basicEnemyDetection.GetFOVAngle() / 2)
            {
                SoundManager.Instance.ActivateSoundByName(SoundAction.Enemy_Hit, null, true);
                _animator.SetTrigger("attackTrigger");
                QTEManager.Instance.ActivateQTE(_animator, transform);
            }
        }
    }

    public void ChasePlayerLastSeenPosition()
    {
        if (isInQTE)
            return;
        if (IsNearWallAbility)
        {
            StopChasing();
            return;
        }
        SetMovementSpeed(_chasingMovementSpeed);
        StopRotating();
        _agent.isStopped = false;
        _positionChased = _basicEnemyDetection.playerLastSeenPosition;
    }

    public void ChaseInitialPosition()
    {
        if (isInQTE)
            return;
        if (IsNearWallAbility)
        {
            StopChasing();
            return;
        }
        SetMovementSpeed(_usualMovementSpeed);
        StopRotating();
        _agent.isStopped = false;
        _positionChased = _initialPositionSelf;
        EnemyEvents.OnIsAtPlayerLastSeenPosition?.Invoke(gameObject.transform.parent.gameObject, gameObject.transform.position, true);
    }

    public void StopChasing()
    {
        _agent.isStopped = true;
    }

    private void ActivateAgent(float specifiedDistance, float pushForce, float timeStunned)
    {
        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance <= specifiedDistance)
        {
            KnockBack(pushForce);
            StartCoroutine(ReactivateAgentAfterDelay(timeStunned));
        }
        else
        {
            _agent.isStopped = false;
            isInQTE = false;
        }

    }

    private IEnumerator ReactivateAgentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_isDead && _agent != null)
        {
            _agent.isStopped = false;
            isInQTE = false;
        }
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float pushForce, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.position += direction * pushForce * Time.deltaTime;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void KnockBack(float pushForce)
    {
        // Calcula la dirección del knockback
        Vector3 knockbackDirection = (transform.position - _player.transform.position).normalized;

        StartCoroutine(KnockbackCoroutine(knockbackDirection, pushForce, 0.5f));
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
        if (isInQTE)
            return;
        if (IsNearWallAbility)
        {
            StopChasing();
            return;
        }
        if (_patrolPoints.Count == 0)
        {
            RotateInPlace();
            return;
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
                    RotateInPlace();

                    _timerWaitEndPatrol -= Time.deltaTime;
                }
                else
                {
                    StopRotating();
                    _agent.isStopped = false;

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

    public void GetKilled()
    {
        //Desactivaci�n de componentes del Body y de KnockDownZone.
        _isDead = true;
        gameObject.GetComponent<BasicEnemyAI>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.transform.Find("KnockDownZone").gameObject.SetActive(false);

        //Activate Loot
        gameObject.GetComponent<LooteableObject>().enabled = true;
        gameObject.transform.Find("LooteableZone").gameObject.SetActive(true);

        // Desactivaci�n de FOV_Visualization, CanvasInWorld y PatrolPoints si hay.
        Transform parent = gameObject.transform.parent;
        parent.Find("FOV_Visualization").gameObject.SetActive(false);
        parent.Find("CanvasInWorld").gameObject.SetActive(false);
        Transform patrol = parent.Find("PatrolPoints");
        if (patrol != null)
        {
            patrol.gameObject.SetActive(false);
        }

        // Activaci�n de la animaci�n.
        _animator.SetBool("isDead", true);
    }

    private void ActivateIsInQTE()
    {
        isInQTE = true;
    }

    public bool GetIsDead()
    {
        return _isDead;
    }
}