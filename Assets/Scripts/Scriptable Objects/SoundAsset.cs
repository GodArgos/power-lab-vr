using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Asset", menuName = "Audio/Sound Asset")]
public class SoundAsset : ScriptableObject
{
    public string soundID;   // ID único para el sonido
    public AudioClip audioClip;
}
