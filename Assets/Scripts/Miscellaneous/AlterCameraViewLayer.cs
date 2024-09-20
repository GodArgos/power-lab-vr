using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterCameraViewLayer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask cullingLayers;
    private LayerMask initialLayerMask;

    private void Start()
    {
        initialLayerMask = cam.cullingMask;
    }

    public void OnVisor()
    {
        if (cam != null)
        {
            cam.cullingMask = cullingLayers;
        }
    }

    public void OnVisorOff()
    {
        if (cam != null)
        {
            cam.cullingMask = initialLayerMask;
        }
    }
}
