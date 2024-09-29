using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private int level = 0;
    [SerializeField] public int order = 0;
    private Collider coll;
    private bool registered = false;

    private void Start()
    {
        coll = GetComponent<Collider>();
        SpawnManager.Instance.AddSpawnPoint(level, order, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SpawnManager.Instance.UpdateCurrentSpawnPoint(order);
            coll.enabled = false;
        }
    }
}
