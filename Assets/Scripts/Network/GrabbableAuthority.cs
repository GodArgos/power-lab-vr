using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabbableAuthority : NetworkBehaviour
{
    public bool canBeGrabbed = true;
    [SyncVar(hook = nameof(OnIsGrabbedChanged))] public bool isGrabbed = false; 
    private NetworkIdentity m_objectIdentity;
    private XRBaseInteractable m_grabInteractable;
    private DelegateAuthority m_delegateAuthority;

    private void Start()
    {
        m_objectIdentity = GetComponent<NetworkIdentity>();
    }

    private void OnEnable()
    {
        if (!canBeGrabbed) return;

        m_grabInteractable = GetComponent<XRBaseInteractable>();
        m_delegateAuthority = GetComponent<DelegateAuthority>();

        m_grabInteractable.selectEntered.AddListener(OnGrabbed);
        m_grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        m_grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        m_grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!isGrabbed && m_grabInteractable.isActiveAndEnabled)
        {
            if (!isOwned)
            {
                m_delegateAuthority.CmdRequestAuthority(m_objectIdentity);
            }

            CmdUpdateGrabState(true);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        m_delegateAuthority.CmdRequestAuthority(m_objectIdentity);
        CmdUpdateGrabState(false);
    }

    #region Commands
    [Command(requiresAuthority = false)]
    private void CmdUpdateGrabState(bool state)
    {
        this.isGrabbed = state;
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateXRGrabbableState(bool state)
    {
        RpcUpdateXRGrabbableState(state);
    }
    #endregion

    #region Hooks
    public void OnIsGrabbedChanged(bool oldValue, bool newValue)
    {
        CmdUpdateXRGrabbableState(!newValue);
    }
    #endregion

    #region Remote Client Calls
    [ClientRpc(includeOwner = false)]
    private void RpcUpdateXRGrabbableState(bool state)
    {
        m_grabInteractable.enabled = state;

        //foreach (Collider col in m_grabInteractable.colliders)
        //{
        //    col.enabled = state;
        //}
    }
    #endregion

}
