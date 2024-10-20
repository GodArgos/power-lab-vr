using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CheckVisorGrabbability : MonoBehaviour
{
    [SerializeField] private int m_visorIndex;
    private XRGrabInteractable m_interactable;
    private InteractionLayerMask m_initialLayerMask;

    private void Start()
    {
        m_interactable = GetComponent<XRGrabInteractable>();
        m_initialLayerMask = m_interactable.interactionLayers;
    }

    public void OnEnterHover()
    {
        if (UserDataManager.Instance.pickedVisor && m_visorIndex != UserDataManager.Instance.visorPicked)
        {
            m_interactable.interactionLayers = LayerMask.GetMask("Nothing");
        }
    }


    public void OnEnterSelect()
    {
        UserDataManager.Instance.pickedVisor = true;
        UserDataManager.Instance.visorPicked = m_visorIndex;
    }
}
