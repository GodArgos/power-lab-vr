using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoiceTimeTrigger : VoiceTrigger
{
    public float delayTime;

    public override void PlayVoice()
    {
        StartCoroutine(PlayVoiceWithDelay());
    }

    public abstract IEnumerator PlayVoiceWithDelay();
}
