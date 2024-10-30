using UnityEngine;

public class FollowMovement : MonoBehaviour
{
    public GameObject targetObject;  // El objeto que quieres seguir
    private Vector3 lastTargetPosition;

    private void Start()
    {
        // Inicializa la posición del target
        if (targetObject != null)
        {
            lastTargetPosition = targetObject.transform.position;
        }
    }

    private void Update()
    {
        if (targetObject != null)
        {
            // Calcula el cambio en la posición del target
            Vector3 targetDelta = targetObject.transform.position - lastTargetPosition;

            // Mueve este objeto en la misma cantidad
            transform.position += targetDelta;

            // Actualiza la posición anterior
            lastTargetPosition = targetObject.transform.position;
        }
    }
}
