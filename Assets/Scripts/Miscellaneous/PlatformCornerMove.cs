using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCornerMove : MonoBehaviour
{
    [SerializeField] private float m_moveOnXCoord;
    [SerializeField] private float m_moveOnZCoord;
    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private bool m_activate = false;

    private Vector3 endPosition;
    private float startTime = 0f;
    private float journeyLenght;

    private void Start()
    {
        endPosition = new Vector3(m_moveOnXCoord, transform.localPosition.y, m_moveOnZCoord);
    }

    private void Update()
    {
        if (m_activate)
        {
            if (startTime == 0)
            {
                startTime = Time.time;
                journeyLenght = Vector3.Distance(transform.localPosition, endPosition);
            }

            float distCovered = (Time.time - startTime) / m_speed;

            float fractionOfJourney = distCovered / journeyLenght;

            transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, fractionOfJourney);
        }
    }
}
