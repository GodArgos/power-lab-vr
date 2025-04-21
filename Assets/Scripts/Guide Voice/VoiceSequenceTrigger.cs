using UnityEngine;

public class VoiceSequenceTrigger : VoiceTrigger
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

    public override void PlayVoice()
    {
        VoiceManager.Instance?.PlayClipSequence(levelName, voiceType, startIndex, endIndex, () => { OnVoiceClipEnds?.Invoke(); });
    }
}
