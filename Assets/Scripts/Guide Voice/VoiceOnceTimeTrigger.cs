using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOnceTimeTrigger : VoiceTimeTrigger
{
    [SerializeField] private int clipIndex;

    public override IEnumerator PlayVoiceWithDelay()
    {
        yield return new WaitForSeconds(delayTime);

        if (levelName != null && clipIndex >= 0)
        {
            VoiceManager.Instance?.PlayClip(levelName, voiceType, clipIndex, () => { OnVoiceClipEnds?.Invoke(); });
        }
    }
}
