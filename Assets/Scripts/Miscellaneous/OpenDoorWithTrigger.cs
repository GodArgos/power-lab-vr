using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorWithTrigger : OpenDoor
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            openDoor = true;
        }
    }
}
