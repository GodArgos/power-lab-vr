using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldCanvasVRController : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private float m_Height;
    [SerializeField] private float m_Distance;

    private Quaternion initialRotation;

    private void Awake()
    {
        if (m_Target == null)
        {
            m_Target = Camera.main.transform;
        }

        initialRotation = transform.rotation;
    }

    private void Update()
    {
        PositionRelativeToTarget();
        FaceTarget();
    }

    private void FaceTarget()
    {
        Vector3 directionToTarget = transform.position - m_Target.position;
        directionToTarget.y = 0; // Solo en plano horizontal

        if (directionToTarget != Vector3.zero)
        {
            directionToTarget.Normalize();
            Quaternion rotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = rotation;
        }
    }

    private void PositionRelativeToTarget()
    {
        // Queremos solo la dirección horizontal
        Vector3 forward = m_Target.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 position = m_Target.position + forward * m_Distance;
        Vector3 fixedPosition = new(position.x, m_Height, position.z);
        transform.position = fixedPosition;
    }
}
