using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FollowMarker : NetworkBehaviour
{
    [SerializeField] private Transform m_marker;
    public Transform m_target = null;
    public Transform m_cam = null;
    private bool author = false;

    [SyncVar] public bool mapped = false;
    [SyncVar] public Vector3 syncedPosition;
    [SyncVar] public Quaternion syncedRotation;

    private Vector3 initPos;
    private Quaternion initRot;

    private void Start()
    {
        initPos = transform.position;
        initRot = transform.rotation;
    }

    private void Update()
    {
        if (mapped)
        {
            if (!author)
            {
                Debug.Log("SOY SERVER");
                transform.position = syncedPosition;
                transform.rotation = syncedRotation;
            }
            else
            {
                Debug.Log("SOY CLIENTE");
                // Actualizar la posición del marcador basado en el jugador local
                Vector3 updatedPosition = new Vector3(m_target.position.x, initPos.y, m_target.position.z);
                transform.position = updatedPosition;

                // Rotación basada en la cámara del jugador
                Quaternion targetRotation = Quaternion.Euler(-90f, m_cam.eulerAngles.y - 180f, initRot.z);
                transform.rotation = targetRotation;

                // Sincronizar los movimientos con el servidor
                CmdSyncMarkerData(transform.position, transform.rotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !mapped)
        {
            // Obtener la referencia a la cámara y al transform del jugador
            m_cam = other.GetComponent<XROrigin>().Camera.transform;
            m_target = other.transform;
            CmdSyncMapped(true);
            author = true;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSyncMapped(bool state)
    {
        mapped = state;
    }

    // Comando para sincronizar los datos en el servidor
    [Command(requiresAuthority = false)]
    void CmdSyncMarkerData(Vector3 position, Quaternion rotation)
    {
        syncedPosition = position;
        syncedRotation = rotation;
    }
}
