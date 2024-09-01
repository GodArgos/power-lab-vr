using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPlayerNetworked : NetworkBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [SerializeField] private IKTargetFollowVRRig ikRigController;
    
    [Header("Rig Parts")]
    [Space(10)]
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    #region Multiplayer Callback
    //public override void OnStartLocalPlayer()
    //{
    //    base.OnStartLocalPlayer();
    //    ikRigController.head.vrTarget = head;
    //    ikRigController.leftHand.vrTarget = leftHand;
    //    ikRigController.rightHand.vrTarget = rightHand;
    //}

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    if (!isLocalPlayer)
    //    {
    //        ikRigController.enabled = false;
    //    }
    //}
    #endregion


    #region Rig Movement
    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            UpdatePart(root, VRRigReferences.instance.root);
            UpdatePart(head, VRRigReferences.instance.head);
            UpdatePart(leftHand, VRRigReferences.instance.leftHand);
            UpdatePart(rightHand, VRRigReferences.instance.rightHand);
        }
    }

    private void UpdatePart(Transform part, Transform instancePart)
    {
        part.position = instancePart.position;
        part.rotation = instancePart.rotation;
    }

    #endregion
}
