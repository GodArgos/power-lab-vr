using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : NetworkBehaviour
{
    public AudioSource audioSource; // Asigna un AudioSource en el Inspector
    public List<SoundAsset> soundAssets;  // Asigna la base de datos en el Inspector
    private Dictionary<string, AudioClip> soundDictionary;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f; // Sonido 3D
        }

        InitializeSoundDictionary();
    }

    public void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, AudioClip>();

        foreach (var asset in soundAssets)
        {
            if (asset != null && !soundDictionary.ContainsKey(asset.soundID))
                soundDictionary.Add(asset.soundID, asset.audioClip);
        }
    }

    public AudioClip GetAudioClipByID(string soundID)
    {
        soundDictionary.TryGetValue(soundID, out var clip);
        return clip;
    }

    #region Local Client Only
    public void PlaySoundForClient(string soundID)
    {
        PlayOneShotLocal(soundID);
    }

    private void PlayOneShotLocal(string soundID)
    {
        var clip = GetAudioClipByID(soundID);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound ID '{soundID}' not found in SoundDatabase.");
        }
    }

    public void PlayPausableSoundLocal(string soundID)
    {
        var clip = GetAudioClipByID(soundID);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound ID '{soundID}' not found in SoundDatabase.");
        }
    }

    public void StopSoundForClient()
    {
        StopLocalSound();
    }

    private void StopLocalSound()
    {
        audioSource.Stop();
    }

    public void PauseSoundForClient()
    {
        PauseLocalSound();
    }

    private void PauseLocalSound()
    {
        audioSource.Pause();
    }

    #endregion

    #region All Clients
    [Command(requiresAuthority = false)]
    public void CmdPlaySoundForAll(string soundID)
    {
        RpcPlaySoundForAll(soundID);
    }

    [ClientRpc]
    private void RpcPlaySoundForAll(string soundID)
    {
        PlayOneShotLocal(soundID);
    }

    // Métodos para detener el sonido
    [Command(requiresAuthority = false)]
    public void CmdStopSoundForAll()
    {
        RpcStopSoundForAll();
    }

    [ClientRpc]
    private void RpcStopSoundForAll()
    {
        StopLocalSound();
    }

    // Métodos para detener el sonido
    [Command(requiresAuthority = false)]
    public void CmdPauseSoundForAll()
    {
        RpcPauseSoundForAll();
    }

    [ClientRpc]
    private void RpcPauseSoundForAll()
    {
        PauseLocalSound();
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayPausableSoundForAll(string soundID)
    {
        RpcPlayPausablSoundForAll(soundID);
    }

    [ClientRpc]
    private void RpcPlayPausablSoundForAll(string soundID)
    {
        PlayPausableSoundLocal(soundID);
    }
    #endregion
}
