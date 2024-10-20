using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AlterCameraViewLayer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask cullingLayers;
    private LayerMask initialLayerMask;
    private XRSocketInteractor m_socket;

    private void Start()
    {
        initialLayerMask = cam.cullingMask;
        m_socket = GetComponent<XRSocketInteractor>();
    }

    public void OnVisor()
    {
        if (cam != null)
        {
            cam.cullingMask = cullingLayers;
            //var target = m_socket.GetOldestInteractableSelected().transform.gameObject;
            //target.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void OnVisorOff()
    {
        if (cam != null)
        {
            cam.cullingMask = initialLayerMask;
            //var target = m_socket.GetOldestInteractableSelected().transform.gameObject;
            //target.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
