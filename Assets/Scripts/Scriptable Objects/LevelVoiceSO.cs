using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelVoice", menuName = "Voice/Level Voice")]
public class LevelVoiceSO : ScriptableObject
{
    public List<VoiceClipSO> startClips;
    public List<VoiceClipSO> interventionClips;
    public List<VoiceClipSO> finalClips;

    public VoiceClipSO GetClip(VoiceType type, int index)
    {
        List<VoiceClipSO> list = type switch
        {
            VoiceType.Start => startClips,
            VoiceType.Intervention => interventionClips,
            VoiceType.Final => finalClips,
            _ => null
        };

        if (list == null || index < 0 || index >= list.Count)
            return null;

        return list[index];
    }

    public VoiceClipSO[] GetClipsByType(VoiceType type)
    {
        return type switch
        {
            VoiceType.Start => startClips.ToArray(),
            VoiceType.Intervention => interventionClips.ToArray(),
            VoiceType.Final => finalClips.ToArray(),
            _ => new VoiceClipSO[0]
        };
    }
}
