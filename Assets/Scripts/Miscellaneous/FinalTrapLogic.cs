using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FinalTrapLogic : NetworkBehaviour
{
    [SerializeField] private List<GameObject> m_affectedObjects;
    [SerializeField] private List<GameObject> m_forceField;
    [SerializeField] private float m_timeForceField = 2f;
    private bool m_activated = false;

    [SyncVar]
    public int numberOfPlayers = 0;
    public bool alreadyEntered = false;

    [Header("Test Activation")]
    [SyncVar] public bool testActivate = false;

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
        if (other.gameObject.CompareTag("Player") && !alreadyEntered)
        {
            numberOfPlayers++;
            alreadyEntered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && alreadyEntered)
        {
            numberOfPlayers--;
            alreadyEntered = false;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleActivateTrap()
    {
        StartCoroutine(ActivateTrap());
    }

    private IEnumerator ActivateTrap()
    {
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
