using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (!soundPlayer.audioSource.isPlaying)
        {
            soundPlayer.CmdPlaySoundForAll("object_collision");
        }
    }
}
