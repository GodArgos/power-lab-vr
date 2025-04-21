using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserLogic : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserDistance = 8f;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private VoiceTriggerNetworked VoiceTriggerNetworked;

    private RaycastHit rayHit;
    private Ray ray;

    private void Start()
    {
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        ray = new(transform.position, transform.forward);
        if (Physics.Raycast(ray, out rayHit, laserDistance, ~ignoreMask))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, rayHit.point);

            // Verificar si el objeto impactado está en la capa "Player"
            if (rayHit.collider != null && rayHit.collider.CompareTag("Player"))
            {
                SpawnManager.Instance.OnHitPlayer?.Invoke(); // Llamar al evento si se detecta el jugador
                if (VoiceTriggerNetworked != null)
                    VoiceTriggerNetworked.CmdHandleVoiceTrigger();
            }
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }
}
