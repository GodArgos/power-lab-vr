using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector] public int spawnOrder;
    private Collider coll;

    private void Update()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SpawnManager.Instance.UpdateCurrentSpawnPoint(spawnOrder);
            coll.enabled = false;
        }
    }
}
