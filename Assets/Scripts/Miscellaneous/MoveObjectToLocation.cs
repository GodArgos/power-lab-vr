using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToLocation : MonoBehaviour
{
    public Vector3 targetPosition; // The target position to move to
    public Vector3 targetRotation; // The target rotation in Euler angles
    public float moveSpeed = 1.0f; // Speed of movement
    public float rotationSpeed = 1.0f; // Speed of rotation
    public bool moveOnAwake = false;
    private bool isMoving = false; // Flag to check if the object is moving

    private void Awake()
    {
        if (moveOnAwake)
        {
            MoveTo(targetPosition, targetRotation);
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Rotate towards the target rotation
            Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);

            // Check if the object has reached the target position and rotation
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f && Quaternion.Angle(transform.rotation, targetQuaternion) < 0.1f)
            {
                isMoving = false; // Stop moving
            }
        }
    }

    // Call this method to start moving the object
    public void MoveTo(Vector3 position, Vector3 rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
        isMoving = true; // Start moving
    }
}
