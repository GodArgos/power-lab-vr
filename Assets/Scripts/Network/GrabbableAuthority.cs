using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabbableAuthority : NetworkBehaviour
{
    [SyncVar]
    public bool isGrabbed = false; 
    private NetworkIdentity m_objectIdentity;
    private XRGrabInteractable m_grabInteractable;
    private bool author = false;

    private void Start()
    {
        m_objectIdentity = GetComponent<NetworkIdentity>();
        m_grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void OnHoverEnter()
    {
        if (!isGrabbed && !author)
        {
            if (!isOwned && NetworkClient.ready)
            {
                CmdRequestAuthority(netIdentity.connectionToClient);
                author = true;
            }
        }
    }

    public void OnGrabbed()
    {
        if (!isGrabbed && !author)
        {
            if (!isOwned && NetworkClient.ready)
            {
                CmdRequestAuthority(netIdentity.connectionToClient);
                author = true;
            }
        }
    }

    // Command to request authority on the server
    [Command(requiresAuthority = false)]
    private void CmdRequestAuthority(NetworkConnectionToClient sender)
    {
        if (!isGrabbed)
        {
            // Assign authority and mark the object as grabbed
            isGrabbed = true;
            //DisableGrabForOthers();

            if (sender != netIdentity.connectionToClient)
            {
                // Remove existing authority and assign it to the new client
                if (netIdentity.connectionToClient != null)
                {
                    netIdentity.RemoveClientAuthority();
                }
                netIdentity.AssignClientAuthority(sender);
            }
        }
        else
        {
            Debug.Log("Grab request denied: object is already grabbed.");
        }
    }

    // Called when the object is no longer hovered (hover exited)
    public void OnHoverExited()
    {
        if (!isGrabbed && isOwned && NetworkClient.ready)
        {
            // If the object was hovered but not grabbed, release authority
            CmdReleaseAuthority();
            author = false;
        }
    }

    // This method is called when the object is released
    public void OnReleased()
    {
        if (isGrabbed && isOwned && NetworkClient.ready)
        {
            // If the player has authority and releases the object, release authority
            CmdReleaseAuthority();
            author = false;
        }
    }

    // Command to release authority on the server
    [Command(requiresAuthority = false)]
    private void CmdReleaseAuthority()
    {
        isGrabbed = false;
    }
}
