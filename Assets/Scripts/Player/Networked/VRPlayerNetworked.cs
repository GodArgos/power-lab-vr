using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.PlayerLoop;

public class VRPlayerNetworked : NetworkBehaviour
{
    [Header("Rig Parts")]
    [Space(10)]
    public Transform[] root = new Transform[2];
    public Transform[] head = new Transform[2];
    public Transform[] leftHand = new Transform[2];
    public Transform[] rightHand = new Transform[2];
    public Transform triggerBody;

    [Space(15)]

    [Header("Player Avatars")]
    [SerializeField] private GameObject[] models;

    [Space(15)]

    [Header("Sync Variables")]
    [Space(10)]
    [Tooltip("B.O.R.I.S. = 0 | M.E.C.H.A. = 1")]
    [SyncVar(hook = nameof(SetSelectedAvatar))] public int avatarSelected;

    [Space(15)]

    [Header("Network Related")]
    [Space(10)]
    [SerializeField] private NetworkTransformReliable[] networkHeads;
    [SerializeField] private NetworkTransformReliable[] networkLeftHands;
    [SerializeField] private NetworkTransformReliable[] networkRightHands;
    [SerializeField] private NetworkAnimator[] networkAnimators;

    #region Multiplayer Callback
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalPlayerSetup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isLocalPlayer)
        {
            RemotePlayerSetup(this.avatarSelected);
        }
    }
    #endregion

    #region Rig Movement
    private void LateUpdate()
    {
        if (isLocalPlayer)
        { 
            UpdatePart(root[avatarSelected], avatarSelected == 0 ? VRRigReferencesBORIS.Instance.root : VRRigReferencesMECHA.Instance.root);
            UpdatePart(head[avatarSelected], avatarSelected == 0 ? VRRigReferencesBORIS.Instance.head : VRRigReferencesMECHA.Instance.head);
            UpdatePart(leftHand[avatarSelected], avatarSelected == 0 ? VRRigReferencesBORIS.Instance.leftHand : VRRigReferencesMECHA.Instance.leftHand);
            UpdatePart(rightHand[avatarSelected], avatarSelected == 0 ? VRRigReferencesBORIS.Instance.rightHand : VRRigReferencesMECHA.Instance.rightHand);
            UpdatePart(triggerBody, avatarSelected == 0 ? VRRigReferencesBORIS.Instance.root : VRRigReferencesMECHA.Instance.root);
        }
    }

    private void UpdatePart(Transform part, Transform instancePart)
    {
        part.position = instancePart.position;
        part.rotation = instancePart.rotation;
    }

    #endregion

    #region Local Player Set-up
    private void LocalPlayerSetup()
    {
        int _avatarIndex = (UserDataManager.Instance.avatar == UserDataManager.Avatar.BORIS) ? 0 : 1;
        CmdSelectAvatar(_avatarIndex);
        ArrageNetworkComponents(_avatarIndex);
        AvatarSelection(_avatarIndex);
        
        DisableLocalMeshes();

        UserDataManager.Instance.localIdentity = GetComponent<NetworkIdentity>();
    }

    private void AvatarSelection(int index)
    {
        for (int i = 0; i < this.models.Length; i++)
        {
            if (index != i)
            {
                models[i].SetActive(false);
            }
            else
            {
                models[i].SetActive(true);
            }
        }
    }

    private void DisableLocalMeshes()
    {
        foreach (var model in models)
        {
            model.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }

    private void ArrageNetworkComponents(int index)
    {
        for (int i = 0; i < this.networkHeads.Length; i++)
        {
            if (index != i)
            {
                networkHeads[i].enabled = false;
            }
            else
            {
                networkHeads[i].enabled = true;
            }
        }

        for (int i = 0; i < this.networkLeftHands.Length; i++)
        {
            if (index != i)
            {
                networkLeftHands[i].enabled = false;
            }
            else
            {
                networkLeftHands[i].enabled= true;
            }
        }

        for (int i = 0; i < this.networkRightHands.Length; i++)
        {
            if (index != i)
            {
                networkRightHands[i].enabled = false;
            }
            else
            {
                networkRightHands[i].enabled = true;
            }
        }

        for (int i = 0; i < this.networkAnimators.Length; i++)
        {
            if (index != i)
            {
                networkAnimators[i].enabled = false;

            }
            else
            {
                networkAnimators[i].enabled = true;
                networkAnimators[i].Reset();
            }
        }
    }
    #endregion

    #region Remote Player Set-up
    private void RemotePlayerSetup(int index)
    {
        for (int i = 0; i < models.Length; i++)
        {
            if (index != i)
            {
                models[i].SetActive(false);
            }
            else
            {
                models[i].SetActive(true);
            }
        }
    }
    #endregion

    #region SyncVar Hooks
    private void SetSelectedAvatar(int oldIndex, int newIndex)
    {
        this.avatarSelected = newIndex;

        RemotePlayerSetup(newIndex);
    }
    #endregion

    #region Player Commands
    [Command]
    public void CmdSelectAvatar(int index)
    {
        this.avatarSelected = index;
    }
    #endregion
}
