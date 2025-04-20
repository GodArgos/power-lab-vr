using UnityEngine;

[CreateAssetMenu(fileName = "VoiceClip", menuName = "Voice/Voice Clip")]
public class VoiceClipSO : ScriptableObject
{
    public AudioClip audioClip;
    public bool canBeInterrupted = true;
    public bool canBeSkipped = false;
    [TextArea(2, 5)] public string subtitle;
}
