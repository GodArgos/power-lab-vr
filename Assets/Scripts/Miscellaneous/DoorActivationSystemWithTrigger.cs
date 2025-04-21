using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorActivationSystemWithTrigger : NetworkBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [HideInInspector] public bool isActive = false;
    [SerializeField] private OpenDoor[] doors = new OpenDoor[2];
    [SerializeField] private float neededPlayers = 2;

    // Sound Related
    [SerializeField] private SoundPlayer m_SoundPlayer;
    [SerializeField] private string soundID;
    private bool soundPlayed = false;
    
    [SyncVar(hook = nameof(OnNumberOfPlayersChanged))]
    public int numberOfPlayers = 0;

    // HashSet para almacenar jugadores que ya han entrado
    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    [SerializeField] private UnityEvent OnDoorsOpened;

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

    // Hook que se llama cuando el número de jugadores cambia
    private void OnNumberOfPlayersChanged(int oldNumber, int newNumber)
    {
        if (newNumber == neededPlayers)
        {
            if (!soundPlayed)
            {
                m_SoundPlayer.CmdPlaySoundForAll(soundID);
                soundPlayed = true;
            }

            CmdHandleNumberAchieved();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleNumberAchieved()
    {
        foreach (var door in doors) 
        { 
            door.TriggerOpenDoor();
        }

        RpcHandleDoorsOpenedEvent();
    }

    [ClientRpc]
    private void RpcHandleDoorsOpenedEvent()
    {
        OnDoorsOpened?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdHandleColliderStatus(bool status)
    {
        RpcHandleColliderStatu(status);
    }

    [ClientRpc]
    private void RpcHandleColliderStatu(bool status)
    {
        GetComponent<Collider>().enabled = status;
    }
}
