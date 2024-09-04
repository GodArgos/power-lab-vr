using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferencesMECHA : MonoBehaviour
{
    public static VRRigReferencesMECHA Instance;

    public Camera cam;
    public AudioListener audioListener;
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private void Awake()
    {
        Instance = this;
    }
}
