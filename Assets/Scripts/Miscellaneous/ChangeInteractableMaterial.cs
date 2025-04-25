using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class ChangeInteractableMaterial : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> meshes = new List<MeshRenderer>();
    [SerializeField] private Material initialMaterial;
    [SerializeField] private Material changeToMaterial;
    private XRSimpleInteractable interactable;

    private void OnEnable()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (changeToMaterial != null && meshes != null)
        {
            foreach (var mesh in meshes)
            {
                mesh.material = changeToMaterial;
            }
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (initialMaterial != null && meshes != null)
        {
            foreach (var mesh in meshes)
            {
                mesh.material = initialMaterial;
            }
        }
    }
}
