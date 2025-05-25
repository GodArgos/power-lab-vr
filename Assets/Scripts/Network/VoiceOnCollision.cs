using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOnCollision : NetworkBehaviour
{
    [SerializeField] private VoiceTriggerNetworked VoiceTriggerNetworked;
    [SerializeField] private float neededPlayers = 2;
    [SyncVar(hook = nameof(OnNumberOfPlayersChanged))]
    public int numberOfPlayers = 0;

    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // Buscar el NetworkIdentity del jugador en los objetos padres
        NetworkIdentity networkIdentity = other.GetComponentInParent<NetworkIdentity>();

        if (networkIdentity != null && other.gameObject.CompareTag("PlayerNetwork") && !playersInTrigger.Contains(networkIdentity.gameObject))
        {
            playersInTrigger.Add(networkIdentity.gameObject); // Agregar jugador si no ha sido contado antes
            numberOfPlayers++;  // Actualizar el número de jugadores, esto activará el hook
        }
    }

    private void OnNumberOfPlayersChanged(int oldNumber, int newNumber)
    {
        if (newNumber == numberOfPlayers)
        {
            VoiceTriggerNetworked.CmdHandleVoiceTrigger();
        }
    }
}
