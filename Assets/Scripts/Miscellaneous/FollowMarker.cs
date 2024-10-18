using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FollowMarker : NetworkBehaviour
{
    [SerializeField] private Transform m_marker;
    private Transform m_target;
    private Transform m_cam;
    private bool mapped = false;

    private void Update()
    {
        if (isLocalPlayer)
        {
            m_marker.position = new Vector3(m_target.position.x, m_marker.position.y, m_target.position.z);

            Quaternion targetRotation = Quaternion.Euler(m_marker.eulerAngles.x, m_cam.eulerAngles.y - 180f, m_marker.eulerAngles.z);
            m_marker.rotation = targetRotation;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !mapped)
        {
            m_cam = other.GetComponent<XROrigin>().Camera.transform;
            m_target = other.transform;
            mapped = true;
        }
    }
}
