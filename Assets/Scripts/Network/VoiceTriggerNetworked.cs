using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceTriggerNetworked : NetworkBehaviour
{
    [Header("Dependencies")]
    public VoiceTrigger relatedVoiceTrigger;

    [Header("Synchronization Variables")]
    [SyncVar] public bool alreadyPlayed = false;

    [Command(requiresAuthority = false)]
    public void CmdHandleVoiceTrigger()
    {
        if (alreadyPlayed) return;

        if (relatedVoiceTrigger != null)
        {
            RpcHandleVoiceTrigger();

            alreadyPlayed = true;
        }
    }

    [ClientRpc]
    private void RpcHandleVoiceTrigger()
    {
        relatedVoiceTrigger.PlayVoice();
    }
}
