using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Transform m_elevatorTransform; // El transform del elevador
    [SerializeField] private float m_targetPoint; // Punto objetivo en el eje Y
    [SerializeField] private float m_speed = 2f; // Velocidad de movimiento
    [SerializeField] private bool m_activateElevator; // Activador para iniciar el movimiento

    private void FixedUpdate()
    {
        if (m_activateElevator)
        {
            // Obtener la posición local actual del elevador
            Vector3 currentLocalPosition = m_elevatorTransform.localPosition;
            // Definir la posición objetivo con el valor de m_targetPoint en el eje Y
            Vector3 targetLocalPosition = new Vector3(currentLocalPosition.x, m_targetPoint, currentLocalPosition.z);

            // Mover el elevador suavemente hacia la posición objetivo local
            m_elevatorTransform.localPosition = Vector3.MoveTowards(currentLocalPosition, targetLocalPosition, m_speed * Time.fixedDeltaTime);

            // Detener el movimiento si ya hemos alcanzado la posición objetivo
            if (Mathf.Abs(m_elevatorTransform.localPosition.y - m_targetPoint) < 0.01f)
            {
                m_activateElevator = false; // Desactivar el movimiento cuando llegue al objetivo
            }
        }
    }

    public void ActivateElevator()
    {
        m_activateElevator = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
