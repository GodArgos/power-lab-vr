using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VRPlayerNetworked : NetworkBehaviour
{
    [Header("Rig Parts")]
    [Space(10)]
    public Transform[] root = new Transform[2];
    public Transform[] head = new Transform[2];
    public Transform[] leftHand = new Transform[2];
    public Transform[] rightHand = new Transform[2];

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
    [SerializeField] private NetworkTransformReliable networkHead;
    [SerializeField] private NetworkTransformReliable networkLeftHand;
    [SerializeField] private NetworkTransformReliable networkRightHand;

    #region Multiplayer Callback
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalPlayerSetup();
    }
    #endregion

    #region Rig Movement
    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            UpdatePart(root[avatarSelected], VRRigReferences.instance.root);
            UpdatePart(head[avatarSelected], VRRigReferences.instance.head);
            UpdatePart(leftHand[avatarSelected], VRRigReferences.instance.leftHand);
            UpdatePart(rightHand[avatarSelected], VRRigReferences.instance.rightHand);
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

        ArrageNetworkTransforms(_avatarIndex);
        AvatarSelection(_avatarIndex);
        CmdSelectAvatar(_avatarIndex);
        DisableLocalMeshes();
    }

    private void AvatarSelection(int index)
    {
        for (int i = 0; i < models.Length; i++)
        {
            if (index != i)
            {
                models[i].SetActive(false);
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

    private void ArrageNetworkTransforms(int index)
    {
        networkHead.target = head[index].transform;
        networkLeftHand.target = leftHand[index].transform;
        networkRightHand.target = rightHand[index].transform;
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
