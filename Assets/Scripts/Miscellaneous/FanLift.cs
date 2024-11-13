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

    private void Start()
    {
        if (isServer)
        {
            HandleForceFields(false);
        }
        else
        {
            CmdHandleForceFields(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other == playerCollider)
        {
            if (!alreadyOpened)
            {
                characterController = other.GetComponent<CharacterController>();

                //if (isServer)
                //{
                //    HandleForceFields(true);
                //}
                //else
                //{
                //    CmdHandleForceFields(true);
                //}

                RpcHandleForceFields(true);
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

            //if (isServer)
            //{
            //    HandleForceFields(false);
            //}
            //else
            //{
            //    CmdHandleForceFields(false);
            //}

            RpcHandleForceFields(false);

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
        HandleForceFields(state);
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
