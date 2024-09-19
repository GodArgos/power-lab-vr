using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDetectOnCollision : MonoBehaviour
{
    [SerializeField] private HandScannerLogic scanner;

    private void OnCollisionStay(Collision collision)
    {
        if (scanner != null && collision.gameObject.CompareTag("Player"))
        {
            scanner.tryActivate = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (scanner != null && collision.gameObject.CompareTag("Player"))
        {
            scanner.tryActivate = false;
        }
    }
}
