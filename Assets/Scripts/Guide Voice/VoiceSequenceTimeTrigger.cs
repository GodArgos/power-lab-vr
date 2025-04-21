using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSequenceTimeTrigger : VoiceTimeTrigger
{
    [SerializeField] private int startIndex = 0;
    [SerializeField] private int endIndex = -1;
    [SerializeField] private bool playOnStart = false;

    private void Start()
    {
        if (playOnStart)
        {
            PlayVoice();
        }
    }

    public override IEnumerator PlayVoiceWithDelay()
    {
        yield return new WaitForSeconds(delayTime);

        VoiceManager.Instance?.PlayClipSequence(levelName, voiceType, startIndex, endIndex, () => { OnVoiceClipEnds?.Invoke(); });
    }
}
