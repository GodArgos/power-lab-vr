using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDetectOnCollision : MonoBehaviour
{
    [SerializeField] private HandScannerLogic scanner;

    private void OnTriggerStay(Collider other)
    {
        if (scanner != null && other.gameObject.CompareTag("PlayerHand"))
        {
            scanner.tryActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (scanner != null && other.gameObject.CompareTag("PlayerHand"))
        {
            scanner.tryActivate = false;
        }
    }
}
