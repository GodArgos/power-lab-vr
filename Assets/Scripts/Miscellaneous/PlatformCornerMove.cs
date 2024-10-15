using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformCornerMove : MonoBehaviour
{
    [SerializeField] private float m_moveOnXCoord;
    [SerializeField] private float m_moveOnZCoord;
    [SerializeField] private float m_speed = 1.0f;

    private Vector3 positionA;  // The initial position (a)
    private Vector3 positionB;  // The initial position (a)
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float startTime = 0f;
    private float journeyLength;
    private bool isMoving = false;

    private void Start()
    {
        // Set initial positions (either A or B)
        positionA = transform.localPosition;
        startPosition = positionA;
        positionB = new Vector3(m_moveOnXCoord, transform.localPosition.y, m_moveOnZCoord);
        endPosition = positionB;
    }

    private void Update()
    {
        if (isMoving)
        {
            float distCovered = (Time.time - startTime) * m_speed;
            float fractionOfJourney = distCovered / journeyLength;

            transform.localPosition = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            if (fractionOfJourney >= 1.0f)
            {
                isMoving = false;  // Stop moving once the destination is reached
            }
        }
    }

    // Method to start movement from A to B
    public void MoveToPositionB()
    {
        startPosition = transform.localPosition;
        endPosition = positionB;
        StartMovement();
    }

    // Method to start movement from B to A
    public void MoveToPositionA()
    {
        startPosition = transform.localPosition;
        endPosition = positionA;
        StartMovement();
    }

    // Helper method to initialize movement variables
    private void StartMovement()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
        isMoving = true;
    }
}
