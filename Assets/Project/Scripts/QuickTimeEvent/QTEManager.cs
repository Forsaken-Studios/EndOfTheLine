using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    [Header("Push properties")]
    [SerializeField] private float _pushForce = 5f;
    [SerializeField] private float _pushDistance = 5f;
    [SerializeField] private float _timeStunned = 3f;

    [Header("Slider properties")]
    [SerializeField] private float _initialSliderValue = 0.25f;
    [SerializeField] private float _sliderDecreaseRate = 0.4f;
    [SerializeField] private float _summedDifficultySlider = 0.5f;
    [SerializeField] private float _sliderIncreaseValue = 0.2f;
    [SerializeField] private float _qteDuration = 5f;
    int QTETimes = 0;

    [Header("Configuration")]
    [SerializeField] GameObject _playerObject;
    [SerializeField] GameObject _QTE_canvas;
    [SerializeField] Slider _QTE_slider;

    private Animator _enemyAnimator;
    private Transform _enemyTransform;

    private bool _isInQTE = false;
    private bool _challengePassed = false;

    public static QTEManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one MapGenerator! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ActivateQTE(Animator enemyAnimator, Transform enemyTransform)
    {
        if (_isInQTE)
            return;

        EnemyEvents.OnIsOnQTE?.Invoke();
        _challengePassed = false;

        _enemyAnimator = enemyAnimator;
        _enemyTransform = enemyTransform;

        PrepareChallenge();
        StartCoroutine(HandleQTE());
    }

    private void PrepareChallenge()
    {
        // Se desactiva el movimiento del jugador
        _playerObject.GetComponent<PlayerController>().enabled = false;
        // Se desactivan los agentes navmesh de todos los enemigos.
        EnemyEvents.OnDeactivateNMAgent?.Invoke();
        // Se hace zoom al player.
        CameraSingleton.CameraSingletonInstance.ZoomCameraOnInventory();

        // El jugador rota hacia el enemigo que ha generado el QTE.
        RotateParentTowardEnemy();

        // Se activa el canvas del QTE
        _QTE_canvas.SetActive(true);
        _QTE_slider.value = _initialSliderValue;
    }

    private void RotateParentTowardEnemy()
    {
        Vector3 directionToSound = (_enemyTransform.position - _playerObject.transform.position).normalized;
        directionToSound.z = 0;
        float angle = Mathf.Atan2(directionToSound.y, directionToSound.x) * Mathf.Rad2Deg;
        _playerObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator HandleQTE()
    {
        _isInQTE = true;
        float timeElapsed = 0f;

        while (timeElapsed < _qteDuration)
        {
            // Baja el slider poco a poco.
            _QTE_slider.value -= (_sliderDecreaseRate + QTETimes * _summedDifficultySlider) * Time.deltaTime;

            // Detecta la tecla E para subir el slider.
            if (Input.GetKeyDown(KeyCode.F))
            {
                _QTE_slider.value += _sliderIncreaseValue;
            }

            // Si el slider llega al máximo, gana el desafío.
            if (_QTE_slider.value >= 1f)
            {
                _challengePassed = true;
                break;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        EndQTE();
    }

    private void EndQTE()
    {
        CameraSingleton.CameraSingletonInstance.UnZoomToNormalPosition();
        if (_challengePassed)
        {
            SaveYourself();
        }
        else
        {
            Die();
        }

        _QTE_canvas.SetActive(false);
    }

    private void SaveYourself()
    {
        QTETimes++;
        _playerObject.GetComponent<PlayerController>().enabled = true;
        _isInQTE = false;
        EnemyEvents.OnActivateNMAgent?.Invoke(_pushDistance, _pushForce, _timeStunned);
    }

    private void Die()
    {
        SoundManager.Instance.ActivateSoundByName(SoundAction.Enemy_Hit, null, true);
        _enemyAnimator.SetTrigger("attackTrigger");
        Debug.Log("Jugador muerto");
        GameManager.Instance.EndGame();
    }
}
