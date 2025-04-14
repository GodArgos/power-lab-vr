using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLift : NetworkBehaviour
{
    public float liftForce = 10f;  // Force applied upwards
    private CharacterController characterController;
    [SerializeField] private List<PlatformCornerMove> m_cornerWindows;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private SoundPlayer force_soundPlayer;
    private bool alreadyOpened = false;
    private bool firstUsed = false;
    [HideInInspector] public Collider playerCollider;
    [SerializeField] private List<GameObject> forceField;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isServer)
        {
            CmdCloseEverything();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other == playerCollider)
        {
            if (!alreadyOpened)
            {
                characterController = other.GetComponent<CharacterController>();
                CmdOpenEverything();
            } 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Apply upward movement to CharacterController
        if (other.CompareTag("Player") && other == playerCollider)
        {
            if (!firstUsed)
            {
                soundPlayer.CmdPlaySoundForAll("hydraulic_open");
                CmdOpenCorners();
                firstUsed = true;
            }
            
            if (characterController != null)
            {
                Vector3 liftDirection = Vector3.up * liftForce * Time.deltaTime;
                characterController.Move(liftDirection);
            }
        }
    }

    public void CloseEverything()
    {
        if (characterController != null)
        {
            characterController = null;
            soundPlayer.CmdPlaySoundForAll("hydraulic_close");
            CmdCloseCorners();
            CmdCloseEverything();

            alreadyOpened = true;
        }
    }

    private void HandleForceFields(bool state)
    {
        foreach (GameObject go in forceField)
        {
            go.SetActive(state);
        }

        if (state)
        {
            force_soundPlayer.CmdPlayPausableSoundForAll("forcefield");
        }
        else
        {
            force_soundPlayer.CmdStopSoundForAll();
        }
    }

    [ClientRpc]
    private void RpcHandleForceFields(bool state)
    {
        HandleForceFields(state);
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleForceFields(bool state)
    {
        foreach (GameObject go in forceField)
        {
            go.SetActive(state);
        }

        if (state)
        {
            force_soundPlayer.CmdPlayPausableSoundForAll("forcefield");
        }
        else
        {
            force_soundPlayer.CmdStopSoundForAll();
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

    [Command(requiresAuthority = false)]
    private void CmdOpenEverything()
    {
        RpcHandleForceFields(true); // Server calls the RPC to sync force field state on all clients
    }

    [Command(requiresAuthority = false)]
    private void CmdCloseEverything()
    {
        RpcHandleForceFields(false); // Server calls the RPC to sync force field state on all clients
    }
}
