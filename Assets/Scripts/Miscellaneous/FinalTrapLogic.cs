using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FinalTrapLogic : NetworkBehaviour
{
    [SerializeField] private List<GameObject> m_affectedObjects;
    [SerializeField] private List<GameObject> m_forceField;
    [SerializeField] private float m_timeForceField = 2f;
    [SerializeField] private SoundPlayer forceSoundPlayer;
    private bool m_activated = false;

    // SyncVar con un hook para cuando el valor de numberOfPlayers cambie
    [SyncVar(hook = nameof(OnNumberOfPlayersChanged))]
    public int numberOfPlayers = 0;
    public bool alreadyEntered = false;

    [Header("Test Activation")]
    [SyncVar] public bool testActivate = false;

    // HashSet para almacenar jugadores que ya han entrado
    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();


    private void Start()
    {
        foreach (GameObject go in m_forceField)
        {
            go.SetActive(false);
        }
    }

    private void Update()
    {
        if ((!m_activated && alreadyEntered && numberOfPlayers >= 2) || (testActivate && !m_activated))
        {
            if (isServer)
            {
                StartCoroutine(ActivateTrap());
            }
            else
            {
                CmdHandleActivateTrap();
            }
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

    // Hook que se llama cuando el número de jugadores cambia
    private void OnNumberOfPlayersChanged(int oldNumber, int newNumber)
    {
        if (newNumber >= 2)
        {
            CmdHandleActivateTrap();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleActivateTrap()
    {
        StartCoroutine(ActivateTrap());
    }

    private IEnumerator ActivateTrap()
    {
        forceSoundPlayer.CmdPlayPausableSoundForAll("forcefield");
        ActivateForceFields();

        yield return new WaitForSeconds(m_timeForceField);

        EverythingFalls();

        m_activated = true;

        yield return null;
    }

    private void ActivateForceFields()
    {
        foreach (GameObject go in m_forceField)
        {
            go.SetActive(true);
        }
    }

    private void EverythingFalls()
    {
        foreach (GameObject go in m_affectedObjects)
        {
            // Añadimos un Rigidbody si no tiene uno
            Rigidbody rb = go.AddComponent<Rigidbody>();

            // Aplicamos una pequeña fuerza inicial para simular una caída
            Vector3 randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), -1, Random.Range(-0.5f, 0.5f));
            rb.AddForce(randomDirection * 5f, ForceMode.Impulse);
        }
    }
}
