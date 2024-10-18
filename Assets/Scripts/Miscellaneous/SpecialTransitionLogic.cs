using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using Mirror;

public class SpecialTransitionLogic : NetworkBehaviour
{
    [SerializeField] private OpenDoor[] m_doors = new OpenDoor[2];
    [SerializeField] private List<GameObject> m_postParticles;
    [SerializeField] private GameObject m_explosionPrefab;
    [SerializeField] private Transform positionA;  // Posición A
    [SerializeField] private Transform positionB;  // Posición B

    private bool startedTrans = false;
    private XROrigin XROrigin;
    private VignetteController vignette;
    private Camera cam;
    private Vector3 startPosition;
    private float originalApertureSize;
    private float originalFeatheringEffect;

    private void Start()
    {
        XROrigin = FindObjectOfType<XROrigin>();
        vignette = XROrigin.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<VignetteController>();
        vignette.enabled = false;

        foreach (GameObject go in m_postParticles)
        {
            go.SetActive(false);
        }

        // Guardar el estado original del vignette
        originalApertureSize = vignette.apertureSize;
        originalFeatheringEffect = vignette.featheringEffect;
    }

    private void Update()
    {
        if (CheckDoors() && !startedTrans)
        {
            if (isServer)
            {
                RpcStartTransition();
            }
            
            startedTrans = true;
        }
    }

    [ClientRpc]
    private void RpcStartTransition()
    {
        StartCoroutine(HandleTransition());
    }

    private IEnumerator HandleTransition()
    {
        HandlePlayerMovement(false);

        // Activar explosión
        GameObject exp = Instantiate(m_explosionPrefab, XROrigin.transform.position, Quaternion.identity);

        Debug.Log("Explosion Activated");

        vignette.enabled = true;

        // Editar el vignette
        yield return StartCoroutine(EditVignette(true));

        Debug.Log("Vignette Changed");

        // Teletransportar a posiciones
        TeleportPlayer();

        Debug.Log("Players Teleported");

        // Activar objetos de partículas
        foreach (GameObject go in m_postParticles)
        {
            go.SetActive(true);
        }

        // Esperar un par de segundos
        yield return new WaitForSeconds(4);

        Destroy(exp);

        // Volver a los valores originales del vignette
        yield return StartCoroutine(EditVignette(false));

        Debug.Log("Vignette Changed");

        // Desactivar el vignette
        vignette.enabled = false;


        // Permitir el movimiento del jugador (según tu lógica de movimiento)
        HandlePlayerMovement(true);
    }

    private IEnumerator EditVignette(bool reduce)
    {
        if (reduce)
        {
            // Reducir los valores gradualmente
            while (vignette.apertureSize > 0 && vignette.featheringEffect > 0)
            {
                vignette.apertureSize = Mathf.Max(vignette.apertureSize - 0.1f, 0);
                vignette.featheringEffect = Mathf.Max(vignette.featheringEffect - 0.1f, 0);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            // Restaurar los valores gradualmente
            while (vignette.apertureSize < originalApertureSize || vignette.featheringEffect < originalFeatheringEffect)
            {
                vignette.apertureSize = Mathf.Min(vignette.apertureSize + 0.05f, originalApertureSize);
                vignette.featheringEffect = Mathf.Min(vignette.featheringEffect + 0.05f, originalFeatheringEffect);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private void TeleportPlayer()
    {
        if (isServer)
        {
            XROrigin.transform.position = positionA.position;
        }
        else
        {
            XROrigin.transform.position = positionB.position;
            CameraDisconnection();
        }
    }

    private void HandlePlayerMovement(bool status)
    {
        XROrigin.GetComponent<CharacterController>().enabled = status;
    }

    private bool CheckDoors()
    {
        foreach (var door in m_doors)
        {
            if (!door.openDoor)
                return false;
        }
        return true;
    }

    private void CameraDisconnection()
    {
        XROrigin.Camera.cullingMask = 0;
        XROrigin.Camera.clearFlags = CameraClearFlags.SolidColor;
    }
}
