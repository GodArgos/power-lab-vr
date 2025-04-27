using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasVRController : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private float m_Height;
    [SerializeField] private float m_Distance;

    private Quaternion initialRotation;

    private void Awake()
    {
        initialRotation = transform.rotation;


    }
}
