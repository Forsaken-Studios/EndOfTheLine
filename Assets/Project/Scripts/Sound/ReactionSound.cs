using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionSound : MonoBehaviour
{
    private BasicEnemyActions _enemyActions;
    [SerializeField] private DetectionPlayerManager _detectionPlayer; 

    void Start()
    {
        _enemyActions = GetComponentInParent<BasicEnemyActions>();
        if (_enemyActions == null)
        {
            Debug.LogError("No se encontró el componente BasicEnemyActions en el objeto padre.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Sound") && _enemyActions != null && collision.gameObject.layer == LayerMask.NameToLayer("Sound"))
        {
            // Se recoge la posición del sonido en el momento del impacto y se hace girar al enemigo hacia este.
            Vector3 soundPosition = collision.transform.position;
            RotateParentTowardsSound(soundPosition);
        }
    }

    private void RotateParentTowardsSound(Vector3 soundPosition)
    {
        // Se calcula la dirección hacia el sonido.
        Vector3 directionToSound = (soundPosition - _enemyActions.transform.position).normalized;

        // Se calcula el ángulo hacia el sonido.
        directionToSound.z = 0;
        float angle = Mathf.Atan2(directionToSound.y, directionToSound.x) * Mathf.Rad2Deg;

        // Se aplica la rotación al Body del enemigo.
        _enemyActions.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
