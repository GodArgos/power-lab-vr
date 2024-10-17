using Mirror;
using UnityEngine;

public class DoorActivationSystem : NetworkBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [HideInInspector] public bool isActive = false;
    [SerializeField] private OpenDoor[] doors = new OpenDoor[2];
    [SerializeField] private HandScannerLogic[] handScannerLogics = new HandScannerLogic[2];

    private void Update()
    {
        if (!isServer) return; // Ensure only the server handles the logic

        if (GetHandScannerCompletion())
        {
            foreach (var door in doors)
            {
                door.TriggerOpenDoor();
                isActive = true;
            }
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
}
