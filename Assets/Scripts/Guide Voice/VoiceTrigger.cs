using UnityEngine;

public enum VoiceType { Start, Intervention, Final }

public class VoiceTrigger : MonoBehaviour
{
    public string levelName;
    public VoiceType type;
    public int clipIndex;

    public void PlayVoice()
    {
        if (levelName == null || clipIndex < 0) return;

        VoiceManager.Instance?.PlayClip(levelName, type, clipIndex);
    }
}
