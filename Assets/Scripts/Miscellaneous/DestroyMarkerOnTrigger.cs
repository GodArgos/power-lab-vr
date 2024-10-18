using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class DestroyMarkerOnTrigger : NetworkBehaviour
{
    [SerializeField] private FollowMarker m_followMarker;
    [SerializeField] private GameObject m_marker;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            CmdTriggerMarkerDestroy();
            triggered = true;
        }
    }

    // Comando que se ejecuta en el servidor
    [Command(requiresAuthority = false)]
    private void CmdTriggerMarkerDestroy()
    {
        // Llamar al ClientRpc para sincronizar en todos los clientes
        RpcDestroyMarker();
    }

    // ClientRpc que se ejecuta en todos los clientes
    [ClientRpc]
    private void RpcDestroyMarker()
    {
        // Desactivar el FollowMarker en todos los clientes
        m_followMarker.enabled = false;

        // Destruir el marcador en todos los clientes
        Destroy(m_marker);
    }
}
