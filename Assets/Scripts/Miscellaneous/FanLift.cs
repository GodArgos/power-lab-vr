using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLift : MonoBehaviour
{
    public float liftForce = 10f;  // Force applied upwards
    private Rigidbody objectRb;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has a Rigidbody (so we can apply forces)
        objectRb = other.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
            // Disable gravity to simulate flight and start lifting the object
            objectRb.useGravity = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuously apply an upward force while the object is in the fan trigger
        if (objectRb != null)
        {
            objectRb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Re-enable gravity when the object exits the fan area, if it's still within range
        if (objectRb != null)
        {
            objectRb.useGravity = true;
            objectRb = null;
        }
    }
}
