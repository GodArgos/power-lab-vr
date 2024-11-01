using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : NetworkBehaviour
{
    [SerializeField] private Vector3 m_targetPosition;
    [SerializeField] private float m_speed;
    [SerializeField] private SoundPlayer soundPlayer;
    private float startTime = 0f;
    private float journeyLength;
    [SyncVar] public bool allowMove = false;
    private float tolerance = 0.01f;

    // SyncVar con un hook para cuando el valor de numberOfPlayers cambie
    [SyncVar(hook = nameof(OnNumberOfPlayersChanged))]
    public int numberOfPlayers = 0;

    // HashSet para almacenar jugadores que ya han entrado
    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    private void Update()
    {
        if (allowMove)
        {
            if (!soundPlayer.audioSource.isPlaying)
            {
                soundPlayer.CmdPlayPausableSoundForAll("celebration");
            }
            
            if (startTime == 0)
            {
                startTime = Time.time;
                journeyLength = Vector3.Distance(transform.position, m_targetPosition);
            }
            
            float distCovered = (Time.time - startTime) * m_speed;
            float fractionOfJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, m_targetPosition, fractionOfJourney);

            if (Vector3.Distance(transform.position, m_targetPosition) <= tolerance)
            {
                allowMove = false;
            }
        }
    }

    // Hook que se llama cuando el número de jugadores cambia
    private void OnNumberOfPlayersChanged(int oldNumber, int newNumber)
    {
        if (newNumber >= 2)
        {
            if (!soundPlayer.audioSource.isPlaying)
            {
                soundPlayer.CmdPlaySoundForAll("metaldoor_close");
            }

            CmdUpdateActivation(true);
        }
    }

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

    [Command(requiresAuthority = false)]
    private void CmdUpdateActivation(bool state)
    {
        allowMove = state;
    }
}
