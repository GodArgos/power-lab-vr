using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [SerializeField] private Transform door;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private float desiredDuration = 3f;

    [Space(15)]

    [Header("Test Variables")]
    [Space(10)]
    public bool openDoor = false;

    // Non-Serialized Variables
    private Vector3 startPosition;
    private float elapsedTime;

    private void Start()
    {
        startPosition = door.position;    
    }

    void Update()
    {
        if (openDoor)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            door.position = Vector3.Lerp(startPosition, targetPos, percentageComplete);
        }
    }
}
