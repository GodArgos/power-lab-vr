using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<LevelVoiceSO> levels;
    [SerializeField] private List<AudioClip> reconnectPhrases;
    public SubtitleRunner subRunner;

    [Header("Parameters")]
    [Range(0.1f, 5f)]
    [SerializeField] private float minDelayTime = 0.5f;
    [Range(0.1f, 5f)]
    [SerializeField] private float maxDelayTime = 1.0f;

    private Queue<VoiceClipSO> audioQueue = new();
    private VoiceClipSO currentClip;
    private bool isPlaying = false;
    private bool isInterrupting = false;
    private Queue<VoiceClipSO> tempQueue = new();

    private Action onSequenceComplete;
    private bool executingSingleClip = false;

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
        else if (!isPlaying && audioQueue.Count == 0 && onSequenceComplete != null && !executingSingleClip)
        {
            onSequenceComplete.Invoke();
            onSequenceComplete = null;
        }
    }

    public void PlayClip(string levelName, VoiceType type, int index, Action onComplete = null)
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
            onSequenceComplete = onComplete;
            executingSingleClip = true;
            EnqueueClip(clip);
        }
        else
        {
            Debug.LogWarning($"[VoiceManager] Clip no encontrado en '{levelName}' [{type}] con índice {index}");
        }
    }

    public void PlayClipSequence(string levelName, VoiceType voiceType, int startIndex, int endIndex = -1, Action onComplete = null)
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

        int finalIndex = (endIndex == -1) ? clips.Length - 1 : Mathf.Clamp(endIndex, startIndex, clips.Length - 1);
        //int finalIndex = (endIndex >= startIndex && endIndex < clips.Length) ? endIndex : startIndex;

        onSequenceComplete = onComplete;
        executingSingleClip = false;

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

        tempQueue = new Queue<VoiceClipSO>(audioQueue);
        audioQueue.Clear();

        // Omitimos reconectar frase por simplicidad

        yield return StartCoroutine(PlayClipRoutine(newClip, 0f));

        foreach (var clip in tempQueue)
            audioQueue.Enqueue(clip);

        tempQueue.Clear();
        isInterrupting = false;
    }

    private void PlayNextInQueue()
    {
        if (audioQueue.Count == 0) return;

        var nextClip = audioQueue.Dequeue();
        float delay = isInterrupting ? 0f : UnityEngine.Random.Range(minDelayTime, maxDelayTime);
        StartCoroutine(PlayClipRoutine(nextClip, delay));
    }

    private IEnumerator PlayClipRoutine(VoiceClipSO clip, float delay)
    {
        isPlaying = true;
        yield return new WaitForSeconds(delay);

        currentClip = clip;
        audioSource.clip = clip.audioClip;
        audioSource.Play();

        subRunner.UpdateStatus(true);

        subRunner.RunText("L.I.S.A\n" + clip.subtitle, clip.audioClip.length - (delay * 1.5f));

        yield return new WaitForSeconds(clip.audioClip.length);

        subRunner.UpdateStatus(false);

        currentClip = null;
        isPlaying = false;

        if (executingSingleClip && audioQueue.Count == 0 && onSequenceComplete != null)
        {
            onSequenceComplete.Invoke();
            onSequenceComplete = null;
        }
    }

    private VoiceClipSO GetRandomReconnectClip()
    {
        if (reconnectPhrases == null || reconnectPhrases.Count == 0)
            return null;

        var audio = reconnectPhrases[UnityEngine.Random.Range(0, reconnectPhrases.Count)];

        var reconnectClip = ScriptableObject.CreateInstance<VoiceClipSO>();
        reconnectClip.audioClip = audio;
        reconnectClip.subtitle = "...";
        reconnectClip.canBeInterrupted = false;
        reconnectClip.canBeSkipped = true;

        return reconnectClip;
    }
}
