using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class RepairZone : MonoBehaviour
{
    [SerializeField] private LayerMask cullingLayers;
    [SerializeField] private FanLift fan;
    private bool alreadyRepaired = false;
    [SerializeField] private VoiceTriggerNetworked VoiceTriggerNetworked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyRepaired)
        {
            if (VoiceTriggerNetworked != null)
                VoiceTriggerNetworked.CmdHandleVoiceTrigger();

            var cam = other.GetComponent<XROrigin>().Camera;
            if (cam.cullingMask == 0)
            {
                cam.cullingMask = cullingLayers;
                cam.clearFlags = CameraClearFlags.Skybox;
                alreadyRepaired = true;

                fan.CloseEverything();
            }
        }
    }
}
