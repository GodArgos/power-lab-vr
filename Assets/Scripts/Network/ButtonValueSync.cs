using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ButtonValueSync : NetworkBehaviour
{
    [HideInInspector]
    public XRPushButton button;

    [SyncVar(hook = nameof(OnValueChanged))]
    public bool isPressed;

    [SerializeField]
    private ElevatorController elevatorController;
    private BlinkingMaterial blinkingMaterial;

    [Header("TEST")]
    [SerializeField] private bool activator = false;
    [SyncVar]
    [SerializeField] private bool enableTestMode = false;
    [SerializeField] private bool testValue = false;

    [SerializeField] private SoundPlayer soundPlayer;

    private void Start()
    {
        button = GetComponent<XRPushButton>();
        blinkingMaterial = transform.GetChild(0).GetComponent<BlinkingMaterial>();

        // Initialize isPressed with the button's initial value
        isPressed = false;

        // Subscribe to the button's events
        button.onPress.AddListener(OnButtonPressed);
        button.onRelease.AddListener(OnButtonReleased);
    }

    private void Update()
    {
        if (activator && !enableTestMode)
        {
            enableTestMode = true;
        }

        if (enableTestMode)
        {
            if (isServer)
            {
                isPressed = testValue;
            }
            else
            {
                CmdSetPressed(testValue);
            }
        }

        if (isPressed && soundPlayer.audioSource.isPlaying)
        {
            soundPlayer.CmdStopSoundForAll();
        }
    }

    // Called when the button is pressed locally
    private void OnButtonPressed()
    {
        if (isOwned && !enableTestMode)
        {
            CmdSetPressed(true);
        }
    }

    // Called when the button is released locally
    private void OnButtonReleased()
    {
        if (isOwned && !enableTestMode)
        {
            CmdSetPressed(false);
        }
    }

    // Command to set the pressed state on the server
    [Command(requiresAuthority = false)]
    private void CmdSetPressed(bool pressed)
    {
        isPressed = pressed;
    }

    // Hook called on clients when isPressed changes
    private void OnValueChanged(bool oldValue, bool newValue)
    {
        // Update the button's visual state if necessary

        if (newValue && !oldValue)
        {
            // Button was pressed
            // Activate the elevator
            if (elevatorController != null)
            {
                elevatorController.ActivateElevator();
            }

            // Stop the blinking effect
            if (blinkingMaterial != null)
            {
                blinkingMaterial.StopBlinking();
            }

            GetComponent<XRPushButton>().enabled = false;
        }
    }

    // Method to get the synchronized pressed state
    public bool GetIsPressed()
    {
        return isPressed;
    }
}
