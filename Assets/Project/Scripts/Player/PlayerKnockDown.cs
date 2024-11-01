using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockDown : MonoBehaviour
{
    [Header("Detection Properties")]
    [SerializeField] private float _detectionWidth = 2f;
    [SerializeField] private float _detectionHeight = 2f;
    [SerializeField] private float _detectionDistance = 2f;
    [SerializeField] private LayerMask _detectionLayerMask;

    [Header("UI")]
    [SerializeField] private GameObject _HotKeyPrefab;
    [SerializeField] private bool _isDrawingGizmos = true;

    private Transform _bodyTransform;
    private bool _isEnemyKillable = false;
    private GameObject _enemyGameObject;
    private GameObject _HotKeyInstance;
    private bool _isOnBack;

    void Awake()
    {
        EnemyEvents.OnIsOnBack += SetIsOnBack;
    }

    void OnDestroy()
    {
        EnemyEvents.OnIsOnBack -= SetIsOnBack;
    }

    void Start()
    {
        _bodyTransform = transform;
        _isOnBack = false;
    }

    void Update()
    {
        UpdateRangeEnemy();

        if (_isEnemyKillable)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                KnockDownEnemy();
                Destroy(_HotKeyInstance);
                _HotKeyInstance = null;
            }
        }
        else
        {
            if (_HotKeyInstance != null)
            {
                Destroy(_HotKeyInstance);
                _HotKeyInstance = null;
            }
        }
    }

    private void UpdateRangeEnemy()
    {
        Vector2 detectionCenter = (Vector2)_bodyTransform.position + (Vector2)(_bodyTransform.right * _detectionDistance);
        Vector2 detectionSize = new Vector2(_detectionWidth, _detectionHeight);

        // Obtener los objetos dentro del cuadro
        Collider2D[] hits = Physics2D.OverlapBoxAll(detectionCenter, detectionSize, _bodyTransform.eulerAngles.z, _detectionLayerMask);

        _isEnemyKillable = false;
        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject)
            {
                if (hit.CompareTag("Enemy") && _isOnBack)
                {
                    _enemyGameObject = hit.gameObject;
                    _isEnemyKillable = true;
                    if(_HotKeyInstance == null)
                    {
                        _HotKeyInstance = Instantiate(_HotKeyPrefab, _enemyGameObject.transform.position, Quaternion.identity);
                        _HotKeyInstance.transform.SetParent(_enemyGameObject.transform);
                    }
                    break;
                }
            }
        }
    }

    private void KnockDownEnemy()
    {
        SoundManager.Instance.ActivateSoundByName(SoundAction.Player_Hit, null, true);
        _enemyGameObject.GetComponent<BasicEnemyActions>().GetKilled();
    }

    void OnDrawGizmos()
    {
        if (_isDrawingGizmos)
        {
            if (_bodyTransform == null)
                _bodyTransform = transform;

            Gizmos.color = Color.red;

            Vector2 detectionCenter = (Vector2)_bodyTransform.position + (Vector2)(_bodyTransform.right * _detectionDistance);
            Vector2 detectionSize = new Vector2(_detectionWidth, _detectionHeight);

            // Guardar la matriz de transformación actual
            Gizmos.matrix = Matrix4x4.TRS(detectionCenter, _bodyTransform.rotation, Vector3.one);

            // Dibujar el cuadro
            Gizmos.DrawWireCube(Vector2.zero, detectionSize);
        }
    }

    private void SetIsOnBack(bool result)
    {
        _isOnBack = result;
    }
}
