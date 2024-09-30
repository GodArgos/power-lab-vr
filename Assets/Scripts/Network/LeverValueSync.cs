using Mirror;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class LeverValueSync : NetworkBehaviour
{
    [HideInInspector]
    public XRLever lever;

    [SyncVar(hook = nameof(OnValueChanged))]
    public bool syncedValue;

    [Header("TEST")]
    [SerializeField] private bool activator = false;
    [SyncVar]
    [SerializeField] private bool enableTestMode = false;
    [SerializeField] private bool testValue = false;

    private void Start()
    {
        lever = GetComponent<XRLever>();

        syncedValue = lever.value;

        lever.onLeverActivate.AddListener(OnLeverActivated);
        lever.onLeverDeactivate.AddListener(OnLeverDeactivated);
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
                syncedValue = testValue;
            }
            else
            {
                CmdSetValue(testValue);
            }
        }
    }

    private void OnLeverActivated()
    {
        if (isOwned && !enableTestMode)
        {
            CmdSetValue(true);
        }
    }

    private void OnLeverDeactivated()
    {
        if (isOwned && !enableTestMode)
        {
            CmdSetValue(false);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetValue(bool value)
    {
        syncedValue = value;
    }

    private void OnValueChanged(bool oldValue, bool newValue)
    {
        lever.SetValueWithoutNotify(newValue);
    }

    public bool GetSyncedValue()
    {
        return syncedValue;
    }
}
