using Mirror;
using UnityEngine;

public class HandDetectOnCollision : NetworkBehaviour
{
    [SerializeField] private HandScannerLogic scanner;

    private void OnTriggerStay(Collider other)
    {
        if (!scanner.completed && other.gameObject.CompareTag("PlayerHand"))
        {
            CmdSetTryActivate(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!scanner.completed && other.gameObject.CompareTag("PlayerHand"))
        {
            CmdSetTryActivate(false);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetTryActivate(bool state)
    {
        if (scanner != null)
        {
            scanner.SetTryActivate(state);
        }
    }
}

