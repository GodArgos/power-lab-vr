using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorWithTrigger : OpenDoor
{
    [SerializeField] private float neededPlayers = 2;
    [SerializeField] private SoundPlayer m_SoundPlayer;
    [SerializeField] private string soundID;
    private bool soundPlayed = false;

    // SyncVar con un hook para cuando el valor de numberOfPlayers cambie
    [SyncVar] public bool allowAccess = true;
    [SyncVar(hook = nameof(OnNumberOfPlayersChanged))]
    public int numberOfPlayers = 0;

    // HashSet para almacenar jugadores que ya han entrado
    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!allowAccess) return;
        
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
        TriggerOpenDoor();
    }

    [Command(requiresAuthority = false)]
    public void CmdForceOpenDoors()
    {
        if (!soundPlayed)
        {
            m_SoundPlayer.CmdPlaySoundForAll(soundID);
            soundPlayed = true;
        }

        CmdHandleNumberAchieved();
    }
}
