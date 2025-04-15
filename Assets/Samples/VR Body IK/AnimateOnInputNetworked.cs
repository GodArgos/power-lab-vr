using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateOnInputNetworked : NetworkBehaviour
{
    public List<AnimationInput> animationInputs;
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        foreach (var item in animationInputs)
        {
            float actionValue = item.action.action.ReadValue<float>();
            animator.SetFloat(item.animationPropertyName, actionValue);
        }
    }
}
