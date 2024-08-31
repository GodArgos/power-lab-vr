using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GetHeight : MonoBehaviour
{
    public float maxRaycastDistance = 100f; // Distancia máxima para el raycast
    public LayerMask groundLayer; // Capa que representa el suelo
    public XROrigin playerRig;

    public void Start()
    {
        // Realizar el raycast hacia abajo desde la posición actual del objeto
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxRaycastDistance, groundLayer))
        {
            // Calcular la altura del objeto respecto al suelo
            float height = hit.distance;
            Debug.Log("Altura desde el suelo: " + height);

            playerRig.CameraYOffset = height;
        }
        else
        {
            Debug.Log("No se detectó el suelo dentro del rango.");
        }
    }

    private void Update()
    {
        
    }
}
