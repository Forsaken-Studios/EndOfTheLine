using UnityEngine;

public class FollowMovement : MonoBehaviour
{
    public GameObject targetObject;  // El objeto que quieres seguir
    private Vector3 lastTargetPosition;

    private void Start()
    {
        // Inicializa la posici�n del target
        if (targetObject != null)
        {
            lastTargetPosition = targetObject.transform.position;
        }
    }

    private void Update()
    {
        if (targetObject != null)
        {
            // Calcula el cambio en la posici�n del target
            Vector3 targetDelta = targetObject.transform.position - lastTargetPosition;

            // Usa Translate para mover el objeto
            transform.Translate(targetDelta);

            // Actualiza la posici�n anterior
            lastTargetPosition = targetObject.transform.position;
        }
    }

}
