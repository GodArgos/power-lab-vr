using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakVisorLayer : MonoBehaviour
{
    [SerializeField] private int designedSpawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpawnPoint") && other.gameObject.GetComponent<SpawnPoint>().order == designedSpawnPoint)
        {
            Destroy(gameObject);
        }
    }
}
