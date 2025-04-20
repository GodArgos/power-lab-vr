using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VoiceManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<LevelVoiceSO> levels;
    [SerializeField] private List<AudioClip> reconnectPhrases;

    [Header("Parameters")]
    [Range(0.1f, 5f)]
    [SerializeField] private float minDelayTime = 0.5f;
    [Range(0.1f, 5f)]
    [SerializeField] private float maxDelayTime = 1.0f;

    // Variables
    private Queue<VoiceClipSO> audioQueue = new();
    private VoiceClipSO currentClip;
    private bool isPlaying = false;
    private bool isInterrupting = false;
    private Queue<VoiceClipSO> tempQueue = new(); // Cola temporal durante la interrupción

    public static VoiceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (!isPlaying && audioQueue.Count > 0)
        {
            PlayNextInQueue();
        }
    }

    public void PlayClip(string levelName, VoiceType type, int index)
    {
        var level = levels.FirstOrDefault(l => l.name == levelName);
        if (level == null)
        {
            Debug.LogWarning($"[VoiceManager] Nivel '{levelName}' no encontrado.");
            return;
        }

        VoiceClipSO clip = level.GetClip(type, index);

        if (clip != null)
        {
            EnqueueClip(clip);
        }
        else
        {
            Debug.LogWarning($"[VoiceManager] Clip no encontrado en '{levelName}' [{type}] con índice {index}");
        }
    }

    public void PlayClipSequence(string levelName, VoiceType voiceType, int startIndex, int endIndex = -1)
    {
        var level = levels.FirstOrDefault(l => l.name == levelName);
        if (level == null)
        {
            Debug.LogWarning($"[VoiceManager] Nivel '{levelName}' no encontrado.");
            return;
        }

        var clips = level.GetClipsByType(voiceType);
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"[VoiceManager] No hay clips del tipo {voiceType} en el nivel '{levelName}'.");
            return;
        }

        int finalIndex = (endIndex > startIndex) ? Mathf.Min(endIndex, clips.Length - 1) : clips.Length - 1;

        for (int i = startIndex; i <= finalIndex; i++)
        {
            if (i >= 0 && i < clips.Length && clips[i] != null)
            {
                EnqueueClip(clips[i]);
            }
        }
    }

    private void EnqueueClip(VoiceClipSO clip)
    {
        if (isInterrupting)
        {
            tempQueue.Enqueue(clip);
            return;
        }

        if (!isPlaying)
        {
            audioQueue.Enqueue(clip);
        }
        else if (currentClip != null && currentClip.canBeInterrupted && !clip.canBeSkipped)
        {
            StartCoroutine(HandleInterruption(clip));
        }
        else if (!clip.canBeSkipped)
        {
            audioQueue.Enqueue(clip);
        }
    }

    private IEnumerator HandleInterruption(VoiceClipSO newClip)
    {
        isInterrupting = true;

        // Almacenar la cola actual
        tempQueue = new Queue<VoiceClipSO>(audioQueue);
        audioQueue.Clear();

        // Reproducir frase conectora
        VoiceClipSO reconnect = GetRandomReconnectClip();
        yield return StartCoroutine(PlayClipRoutine(reconnect, 0f)); // sin delay

        // Reproducir el nuevo clip
        yield return StartCoroutine(PlayClipRoutine(newClip, 0f)); // sin delay

        // Restaurar cola original
        foreach (var clip in tempQueue)
            audioQueue.Enqueue(clip);

        tempQueue.Clear();
        isInterrupting = false;
    }

    private void PlayNextInQueue()
    {
        if (audioQueue.Count == 0) return;

        var nextClip = audioQueue.Dequeue();
        float delay = isInterrupting ? 0f : Random.Range(minDelayTime, maxDelayTime);
        StartCoroutine(PlayClipRoutine(nextClip, delay));
    }

    private IEnumerator PlayClipRoutine(VoiceClipSO clip, float delay)
    {
        isPlaying = true;
        yield return new WaitForSeconds(delay);

        currentClip = clip;
        audioSource.clip = clip.audioClip;
        audioSource.Play();

        // Aquí podrías emitir subtítulos: clip.subtitle

        yield return new WaitForSeconds(clip.audioClip.length);
        currentClip = null;
        isPlaying = false;
    }

    private VoiceClipSO GetRandomReconnectClip()
    {
        if (reconnectPhrases == null || reconnectPhrases.Count == 0)
            return null;

        var audio = reconnectPhrases[Random.Range(0, reconnectPhrases.Count)];

        var reconnectClip = ScriptableObject.CreateInstance<VoiceClipSO>();
        reconnectClip.audioClip = audio;
        reconnectClip.subtitle = "...";
        reconnectClip.canBeInterrupted = false;
        reconnectClip.canBeSkipped = true;

        return reconnectClip;
    }
}
