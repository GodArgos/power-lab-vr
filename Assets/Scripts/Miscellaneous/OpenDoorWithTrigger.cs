using Mirror;
using UnityEngine;

public class OpenDoorWithTrigger : OpenDoor
{
    [SyncVar]
    public int numberOfPlayers = 0;
    public bool alreadyEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !alreadyEntered)
        {
            numberOfPlayers++;

            if (numberOfPlayers >= 2)
            {
                CmdHandleNumberAchieved();
            }

            alreadyEntered = true;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleNumberAchieved()
    {
        TriggerOpenDoor();
    }
}
