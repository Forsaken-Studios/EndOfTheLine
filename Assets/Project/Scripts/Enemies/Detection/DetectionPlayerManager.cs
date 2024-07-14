using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils.CustomLogs;

public class DetectionPlayerManager : MonoBehaviour
{
    [Header("FOV Properties")]
    [SerializeField] private float _FOVAngle = 40f;
    [SerializeField] private float _viewDistance = 5f;
    [SerializeField] private LayerMask _detectionLayerMask;
    [SerializeField] private float _maxDistanceToNearEnemyPartner = 5f;
    private int _rayCount = 15;

    [Header("Detection Bar Properties")]
    [SerializeField] private float _detectionIncreaseRate = 0.4f;
    [SerializeField] private float _detectionDecreaseRate = 0.4f;

    [Header("External scripts")]
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private GameObject _detectionBarObject;
    private DetectionUI _detectionUI;
    private Transform _playerTransform;
    
    private float _detectionLevel = 0f;
    private List<RaycastHit2D> _raycastHitsList = new List<RaycastHit2D>();
    private Mesh _mesh;
    private float _currentAngle;
    
    private Vector3 _origin;
    private Vector3 _meshPivot;

    public EnemyStates.FOVState currentState { get; private set; }
    public bool isPlayerDetected { get; private set; }
    public Vector3 playerLastSeenPosition { get; private set; }

    void Awake()
    {
        EnemyEvents.OnForgetPlayer += StopPlayerDetected;
        EnemyEvents.OnSeenPlayer += CheckIfFollowPlayer;
        EnemyEvents.OnIsAtPlayerLastSeenPosition += ActivateIsAtPlayerLastSeenPosition;
    }

    void OnDestroy()
    {
        EnemyEvents.OnForgetPlayer -= StopPlayerDetected;
        EnemyEvents.OnSeenPlayer -= CheckIfFollowPlayer;
        EnemyEvents.OnIsAtPlayerLastSeenPosition -= ActivateIsAtPlayerLastSeenPosition;
    }

    void Start()
    {        
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _origin = _bodyTransform.position;
        _meshPivot = _bodyTransform.position;

        _detectionUI = _detectionBarObject.GetComponent<DetectionUI>();

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        currentState = EnemyStates.FOVState.isWatching;
        isPlayerDetected = false;

        _detectionBarObject.SetActive(false);
    }

    void Update()
    {
        _origin = _bodyTransform.position;

        UpdateFieldOfViewMesh();
        UpdateDetection();
        CheckDetectionBarActive();
        UpdatePlayerLastSeenPosition();
        _detectionUI.UpdateDetectionBar(_detectionLevel);

        CheckLogs();
    }

    private void CheckLogs()
    {
        if (isPlayerDetected)
        {
            LogManager.Log("ALREADY DETECTED", FeatureType.FieldOfView);
        }

        if (currentState == EnemyStates.FOVState.isSeeing)
        {
            LogManager.Log("SEEING", FeatureType.FieldOfView);
        }else if (currentState == EnemyStates.FOVState.isWatching)
        {
            LogManager.Log("WATCHING", FeatureType.FieldOfView);
        }else if (currentState == EnemyStates.FOVState.isDetecting)
        {
            LogManager.Log("DETECTING", FeatureType.FieldOfView);
        }

    }

    private void UpdateDetection()
    {
        bool playerInSight = CheckPlayerInSight();

        if (playerInSight)
        {
            IncreaseDetection();
        }
        else if(!playerInSight && !isPlayerDetected)
        {
            DecreaseDetection();
        }
        else
        {
            currentState = EnemyStates.FOVState.isWatching;
        }
    }

    private void UpdatePlayerLastSeenPosition()
    {
        if(currentState == EnemyStates.FOVState.isSeeing)
        {
            playerLastSeenPosition = _playerTransform.position;
            EnemyEvents.OnSeenPlayer?.Invoke(_bodyTransform.position, playerLastSeenPosition);
        }
    }

    private void CheckIfFollowPlayer(Vector3 enemySenderPosition, Vector3 newPlayerLastSeenPosition)
    {
        float distanceToEnemySender = Vector3.Distance(_bodyTransform.position, enemySenderPosition);

        if (distanceToEnemySender <= _maxDistanceToNearEnemyPartner)
        {
            isPlayerDetected = true;
            _detectionLevel = 1f;
            playerLastSeenPosition = newPlayerLastSeenPosition;
        }
    }

    private void CheckDetectionBarActive()
    {
        if(_detectionLevel > 0)
        {
            _detectionBarObject.SetActive(true);
        }
        else
        {
            _detectionBarObject.SetActive(false);
        }
    }

    private bool CheckPlayerInSight()
    {
        foreach (var raycast in _raycastHitsList)
        {
            if (raycast.collider != null)
            {
                if (raycast.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private Vector3 GetDirectionFromAngle(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }

    private void IncreaseDetection()
    {
        _detectionLevel += _detectionIncreaseRate * Time.deltaTime;

        currentState = EnemyStates.FOVState.isSeeing;

        if (_detectionLevel >= 1f)
        {
            _detectionLevel = 1f;
            isPlayerDetected = true;
        }
        else
        {
            currentState = EnemyStates.FOVState.isDetecting;
        }
    }

    private void DecreaseDetection()
    {
        if (_detectionLevel > 0f)
        {
            _detectionLevel -= _detectionDecreaseRate * Time.deltaTime;
            if (_detectionLevel <= 0f)
            {
                _detectionLevel = 0f;
            }
            else
            {
                currentState = EnemyStates.FOVState.isDetecting;
            }
        }
        else
        {
            currentState = EnemyStates.FOVState.isWatching;
        }
    }

    public void StopDetection()
    {
        isPlayerDetected = false;
        _detectionLevel = 0f;
    }

    public EnemyStates.FOVState GetCurrentState()
    {
        return currentState;
    }

    private void UpdateFieldOfViewMesh()
    {
        _raycastHitsList.Clear();
        _currentAngle = _bodyTransform.eulerAngles.z + _FOVAngle/2;
        float angle = _currentAngle;
        float angleIncrease = _FOVAngle / _rayCount;

        Vector3[] vertices = new Vector3[_rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_rayCount * 3];

        Vector3 adjustedOrigin = _origin - _meshPivot;

        vertices[0] = adjustedOrigin;
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= _rayCount; i++)
        {
            Vector3 vertex = Vector3.back;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(_origin, GetDirectionFromAngle(angle), _viewDistance, _detectionLayerMask);
            _raycastHitsList.Add(raycastHit2D);
            if (raycastHit2D.collider == null)
            {
                vertex = _origin + GetDirectionFromAngle(angle) * _viewDistance;
            }
            else
            {
                vertex = raycastHit2D.point;
            }

            vertices[vertexIndex] = vertex - _meshPivot;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
    }

    public float GetFOVAngle()
    {
        return _FOVAngle;
    }

    public void ActivatePlayerDetected()
    {
        isPlayerDetected = true;
    }

    public void SetPlayerLastSeenPosition(Vector3 newPlayerLastSeenPosition)
    {
        playerLastSeenPosition = newPlayerLastSeenPosition;
    }

    private void StopPlayerDetected()
    {
        isPlayerDetected = false;
    }

    public void StartPlayerDetected()
    {
        isPlayerDetected = true;
    }

    public float GetMaxDistanceToNearEnemyPartner()
    {
        return _maxDistanceToNearEnemyPartner;
    }

    private void ActivateIsAtPlayerLastSeenPosition()
    {
        playerLastSeenPosition = _bodyTransform.position;
    }
}