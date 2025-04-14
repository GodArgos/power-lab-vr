using Mirror;
using UnityEngine;

public class DelegateAuthority : NetworkBehaviour
{
    [Command(requiresAuthority = false)]
    public void CmdRequestAuthority(NetworkIdentity item, NetworkConnectionToClient sender = null)
    {
        if (sender != null)
        {
            item.RemoveClientAuthority();
            item.AssignClientAuthority(sender);
        }
        else
        {
            Debug.Log("There's no sender");
        }
    }

}
