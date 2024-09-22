using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableWeaponDestruction : MonoBehaviour
{
    [SerializeField] private string desiredTag;
    private string currentTag;
    private Vector3 lastPos;
    [HideInInspector] public Vector3 velocity;
    private bool grabbed = false;

    void Start()
    {
        currentTag = gameObject.tag;
    }

    private void Update()
    {
        if (grabbed)
        {
            Vector3 currentPos = transform.position;
            velocity = (currentPos - lastPos) / Time.deltaTime;
            lastPos = currentPos;
        }
    }

    public void OnGrabWeapon()
    {
        gameObject.tag = desiredTag;
        grabbed = true;
    }

    public void OnLeaveWeapon()
    {
        gameObject.tag = currentTag;
        grabbed = false;
    }
}
