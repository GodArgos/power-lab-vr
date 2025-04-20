using UnityEngine;
using UnityEditor;
using System.IO;

public class VoiceClipCreator : EditorWindow
{
    private AudioClip selectedAudio;
    private bool canBeInterrupted = true;
    private bool canBeSkipped = false;
    private string subtitleText = "";

    [MenuItem("Assets/Create VoiceClip from Audio", true)]
    private static bool ValidateCreateVoiceClip()
    {
        return Selection.activeObject is AudioClip;
    }

    [MenuItem("Assets/Create VoiceClip from Audio")]
    private static void Init()
    {
        AudioClip selected = Selection.activeObject as AudioClip;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Selecciona un AudioClip para crear el VoiceClip.", "OK");
            return;
        }

        VoiceClipCreator window = CreateInstance<VoiceClipCreator>();
        window.selectedAudio = selected;
        window.titleContent = new GUIContent("Crear VoiceClip");
        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Label("Crear VoiceClip", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Audio:", selectedAudio.name);

        GUILayout.Space(5);
        canBeInterrupted = EditorGUILayout.Toggle("¿Puede ser interrumpido?", canBeInterrupted);
        canBeSkipped = EditorGUILayout.Toggle("¿Puede ser skipeado?", canBeSkipped);
        
        GUILayout.Space(5);
        GUILayout.Label("Subtítulo:");
        subtitleText = EditorGUILayout.TextArea(subtitleText, GUILayout.Height(50));

        
        GUILayout.Space(10);
        if (GUILayout.Button("Crear VoiceClipSO"))
        {
            CreateVoiceClipSO();
            Close();
        }
    }

    private void CreateVoiceClipSO()
    {
        string path = AssetDatabase.GetAssetPath(selectedAudio);
        string folderPath = Path.GetDirectoryName(path);
        string assetName = selectedAudio.name + "_VoiceClip";

        VoiceClipSO newClip = ScriptableObject.CreateInstance<VoiceClipSO>();
        newClip.audioClip = selectedAudio;
        newClip.canBeInterrupted = canBeInterrupted;
        newClip.canBeSkipped = canBeSkipped;
        newClip.subtitle = subtitleText;

        string assetPath = Path.Combine(folderPath, assetName + ".asset");
        AssetDatabase.CreateAsset(newClip, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newClip;
    }
}