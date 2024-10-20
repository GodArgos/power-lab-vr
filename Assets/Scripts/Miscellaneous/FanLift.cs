using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLift : NetworkBehaviour
{
    public float liftForce = 10f;  // Force applied upwards
    private CharacterController characterController;
    [SerializeField] private List<PlatformCornerMove> m_cornerWindows;
    private bool alreadyOpened = false;
    private bool alreadyUsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyUsed)
        {
            characterController = other.GetComponent<CharacterController>();

            if (!alreadyOpened)
            {
                CmdOpenCorners();
            }

            alreadyUsed = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Apply upward movement to CharacterController
        if (other.CompareTag("Player"))
        {
            if (characterController != null)
            {
                Vector3 liftDirection = Vector3.up * liftForce * Time.deltaTime;
                characterController.Move(liftDirection);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Clear the CharacterController reference when exiting the trigger
            if (characterController != null)
            {
                characterController = null;
                CmdCloseCorners();
                alreadyOpened = true;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdOpenCorners()
    {
        foreach (var corner in m_cornerWindows)
        {
            corner.MoveToPositionB();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdCloseCorners()
    {
        foreach (var corner in m_cornerWindows)
        {
            corner.MoveToPositionA();
        }
    }
}
