using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabbableAuthority : NetworkBehaviour
{
    [SyncVar]
    public bool isGrabbed = false; 
    private NetworkIdentity m_objectIdentity;
    private XRGrabInteractable m_grabInteractable;

    private void Start()
    {
        m_objectIdentity = GetComponent<NetworkIdentity>();
        m_grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void OnGrabbed()
    {
        if (!isGrabbed)
        {
            if (!isOwned && isClient && NetworkClient.ready)
            {
                // If the player doesn't have authority, request it
                CmdRequestAuthority();
            }
        }
        else
        {
            Debug.Log("Object is already grabbed by another player.");
        }
    }

    // Command to request authority on the server
    [Command(requiresAuthority = false)]
    private void CmdRequestAuthority(NetworkConnectionToClient sender = null)
    {
        if (!isGrabbed)
        {
            // Assign authority and mark the object as grabbed
            isGrabbed = true;
            //DisableGrabForOthers();

            if (sender != netIdentity.connectionToClient)
            {
                netIdentity.RemoveClientAuthority();
                netIdentity.AssignClientAuthority(sender);
            }
        }
        else
        {
            Debug.Log("Grab request denied: object is already grabbed.");
        }
    }

    // This method is called when the object is released
    public void OnReleased()
    {
        if (isOwned && isClient && NetworkClient.ready)
        {
            // If the player has authority and releases the object, release authority
            CmdReleaseAuthority();
        }
    }

    // Command to release authority on the server
    [Command(requiresAuthority = false)]
    private void CmdReleaseAuthority()
    {
        if (isGrabbed)
        {
            // Mark the object as not grabbed
            isGrabbed = false;

            // Re-enable the grab interactable for others
            //EnableGrabForOthers();

            // Release the authority from the client
            m_objectIdentity.RemoveClientAuthority();
        }
    }

    //// Disable XRGrabInteractable for other players
    //[ClientRpc(includeOwner = false)]
    //private void DisableGrabForOthers()
    //{
    //    if (!isOwned)
    //    {
    //        // Disable the grab interactable to prevent other players from grabbing it
    //        m_grabInteractable.enabled = false;
    //    }
    //}

    //// Enable XRGrabInteractable when the object is released
    //[ClientRpc]
    //private void EnableGrabForOthers()
    //{
    //    // Enable the grab interactable so that other players can grab it again
    //    m_grabInteractable.enabled = true;
    //}
}
