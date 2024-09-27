using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorWithTrigger : OpenDoor
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("asdasdasd");
        if (other.gameObject.CompareTag("Player") && openDoor == false)
        {
            openDoor = true;
        }
    }
}
