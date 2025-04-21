using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum VoiceType { Start, Intervention, Final }

public abstract class VoiceTrigger : MonoBehaviour
{
    [Header("Parameters")]
    public string levelName;
    public VoiceType voiceType = VoiceType.Intervention;

    [Header("Events")]
    public UnityEvent OnVoiceClipEnds;

    public abstract void PlayVoice();
}
