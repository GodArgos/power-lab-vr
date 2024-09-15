using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActivationSystem : MonoBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [SerializeField] private OpenDoor[] doors = new OpenDoor[2];
    [SerializeField] private HandScannerLogic[] handScannerLogic = new HandScannerLogic[2];

    private void Update()
    {
        if (GetHandScannerCompletition())
        {
            foreach (var door in doors)
            {
                door.openDoor = true;
            }
        }
    }

    private bool GetHandScannerCompletition()
    {
        return handScannerLogic[0].completed == true && handScannerLogic[1].completed == true ? true : false;
    }
}
