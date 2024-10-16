using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : MonoBehaviour
{
    [SerializeField] private Vector3 m_targetPosition;
    [SerializeField] private float m_speed;
    private float startTime = 0f;
    private float journeyLength;
    private bool allowMove = false;

    private void Update()
    {
        if (allowMove)
        {
            if (startTime == 0)
            {
                startTime = Time.time;
                journeyLength = Vector3.Distance(transform.localPosition, m_targetPosition);
            }
            
            float distCovered = (Time.time - startTime) * m_speed;
            float fractionOfJourney = distCovered / journeyLength;

            transform.localPosition = Vector3.Lerp(transform.localPosition, m_targetPosition, fractionOfJourney);
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
