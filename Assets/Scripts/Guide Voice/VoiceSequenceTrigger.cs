using UnityEngine;

public class VoiceSequenceTrigger : MonoBehaviour
{
    [Header("Datos del nivel y tipo de voz")]
    public string levelName;
    public VoiceType voiceType = VoiceType.Intervention;

    [Header("Secuencia")]
    public int startIndex = 0;
    public int endIndex = -1; // Si es menor o igual que startIndex, se ignora

    [Tooltip("¿Reproducir automáticamente al iniciar el objeto?")]
    public bool playOnStart = false;

    private void Start()
    {
        if (playOnStart)
        {
            TriggerSequence();
        }
    }

    public void TriggerSequence()
    {
        VoiceManager.Instance?.PlayClipSequence(levelName, voiceType, startIndex, endIndex);
    }
}
