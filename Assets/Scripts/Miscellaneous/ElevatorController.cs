using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : NetworkBehaviour
{
    [SerializeField] private Transform m_elevatorTransform; // El transform del elevador
    [SerializeField] private float m_targetPoint; // Punto objetivo en el eje Y
    [SerializeField] private float m_speed = 2f; // Velocidad de movimiento
    [SyncVar]
    [SerializeField] private bool m_activateElevator; // Activador para iniciar el movimiento

    private Transform previousPlayerTransform;
    [SerializeField] private SoundPlayer m_soundPlayer;
    private void FixedUpdate()
    {
        if (m_activateElevator)
        {
            if (!m_soundPlayer.audioSource.isPlaying)
            {
                m_soundPlayer.CmdPlayPausableSoundForAll("elevator_song");
            }
            
            // Obtener la posición local actual del elevador
            Vector3 currentLocalPosition = m_elevatorTransform.localPosition;
            // Definir la posición objetivo con el valor de m_targetPoint en el eje Y
            Vector3 targetLocalPosition = new Vector3(currentLocalPosition.x, m_targetPoint, currentLocalPosition.z);

            // Mover el elevador suavemente hacia la posición objetivo local
            m_elevatorTransform.localPosition = Vector3.MoveTowards(currentLocalPosition, targetLocalPosition, m_speed * Time.fixedDeltaTime);

            // Detener el movimiento si ya hemos alcanzado la posición objetivo
            if (Mathf.Abs(m_elevatorTransform.localPosition.y - m_targetPoint) < 0.01f)
            {
                m_soundPlayer.CmdStopSoundForAll();
                m_activateElevator = false; // Desactivar el movimiento cuando llegue al objetivo
            }
        }
    }
    
    // Public method to activate the elevator
    public void ActivateElevator()
    {
        if (isServer)
        {
            m_activateElevator = true;
        }
        else
        {
            CmdActivateElevator();
        }
    }

    // Command to activate the elevator on the server
    [Command(requiresAuthority = false)]
    public void CmdActivateElevator()
    {
        m_activateElevator = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            previousPlayerTransform = other.transform.parent;
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(previousPlayerTransform);
        }
    }
}
