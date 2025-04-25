using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChangeInteractableOutline : MonoBehaviour
{
    public bool isSelected = false;
    [SerializeField] private Outline objectOutline;
    [SerializeField] private float hoverSize;
    [SerializeField] private float selectedSize;
    private XRSimpleInteractable interactable;

    private void OnEnable()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.activated.AddListener(OnActivatedTried);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.activated.RemoveListener(OnActivatedTried);

    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (objectOutline != null && !isSelected)
        {
            objectOutline.OutlineWidth = hoverSize;
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (objectOutline != null && !isSelected)
        {
            objectOutline.OutlineWidth = 0f;
        }
    }

    private void OnActivatedTried(ActivateEventArgs args)
    {
        if (objectOutline != null)
        {
            isSelected = !isSelected;
            objectOutline.OutlineWidth = isSelected ? selectedSize : hoverSize;
        }
    }
}
