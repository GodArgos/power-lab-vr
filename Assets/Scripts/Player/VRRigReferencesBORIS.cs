using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferencesBORIS : MonoBehaviour
{
    public static VRRigReferencesBORIS Instance;

    public Camera cam;
    public AudioListener audioListener;
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public SubtitleRunner subtitleRunner;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        VoiceManager.Instance.subRunner = subtitleRunner;
    }
}
