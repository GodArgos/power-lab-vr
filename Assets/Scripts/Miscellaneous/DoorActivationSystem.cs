using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class DoorActivationSystem : NetworkBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [HideInInspector] public bool isActive = false;
    [SerializeField] private OpenDoor[] doors = new OpenDoor[2];
    [SerializeField] private HandScannerLogic[] handScannerLogics = new HandScannerLogic[2];
    [SerializeField] private SoundPlayer m_SoundPlayer;
    private bool doorsOpened = false;
    private bool soundPlayed = false;

    [SerializeField] private bool deactiveScannersOnStart = false;
    [SerializeField] private UnityEvent OnDoorsOpened;

    public override void OnStartClient()
    {
        if (deactiveScannersOnStart)
        {
            CmdUpdateHandScannersState(false);
        }
    }

    public void ActivateHandScanners()
    {
        CmdUpdateHandScannersState(true);
        m_SoundPlayer.CmdPlaySoundForAll("scanner_beep");
    }

    private void Update()
    {
        if (!isServer) return; // Ensure only the server handles the logic

        if (GetHandScannerCompletion() && !doorsOpened)
        {
            foreach (var door in doors)
            {
                door.TriggerOpenDoor();
            }

            if (!soundPlayed)
            {
                m_SoundPlayer.CmdPlaySoundForAll("open_door_basic");
                soundPlayed = true;
            }

            CmdHandleDoorsOpenedEvent();
            doorsOpened = true;
        }
    }

    private bool GetHandScannerCompletion()
    {
        foreach (var scanner in handScannerLogics)
        {
            if (!scanner.completed)
                return false;
        }
        return true;
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateHandScannersState(bool status)
    {
        RpcUpdateHandScannersState(status);
    }

    [ClientRpc]
    private void RpcUpdateHandScannersState(bool status)
    {
        foreach (var scanner in handScannerLogics)
        {
            scanner.enabled = status;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleDoorsOpenedEvent()
    {
        RpcHandleDoorsOpenedEvent();
    }

    [ClientRpc]
    private void RpcHandleDoorsOpenedEvent()
    {
        OnDoorsOpened?.Invoke();
    }
}
