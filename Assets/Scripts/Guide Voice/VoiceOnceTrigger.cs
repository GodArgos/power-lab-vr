using UnityEngine;

public class VoiceOnceTrigger : VoiceTrigger
{
    public int clipIndex;

    public override void PlayVoice()
    {
        if (levelName == null || clipIndex < 0) return;

        VoiceManager.Instance?.PlayClip(levelName, voiceType, clipIndex, () => { OnVoiceClipEnds?.Invoke(); });
    }
}
