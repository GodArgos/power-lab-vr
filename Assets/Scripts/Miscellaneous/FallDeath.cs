using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDeath : MonoBehaviour
{
    [SerializeField] private VoiceTriggerNetworked VoiceTriggerNetworked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SpawnManager.Instance.OnHitPlayer?.Invoke();

            if (VoiceTriggerNetworked != null)
                VoiceTriggerNetworked.CmdHandleVoiceTrigger();
        }
    }
}
