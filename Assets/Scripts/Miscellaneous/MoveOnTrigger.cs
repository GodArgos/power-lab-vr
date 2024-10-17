using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : NetworkBehaviour
{
    [SerializeField] private Vector3 m_targetPosition;
    [SerializeField] private float m_speed;
    private float startTime = 0f;
    private float journeyLength;
    [SyncVar] public bool allowMove = false;

    private void Update()
    {
        if (allowMove)
        {
            if (startTime == 0)
            {
                startTime = Time.time;
                journeyLength = Vector3.Distance(transform.position, m_targetPosition);
            }
            
            float distCovered = (Time.time - startTime) * m_speed;
            float fractionOfJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, m_targetPosition, fractionOfJourney);

            if (Vector3.Distance(transform.position, m_targetPosition) <= 0f)
            {
                allowMove = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            allowMove = true;
        }
    }
}
