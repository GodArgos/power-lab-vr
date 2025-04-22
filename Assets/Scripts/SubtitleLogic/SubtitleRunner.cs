using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleRunner : MonoBehaviour
{
    public Image textBackground;
    public TMP_Text subtitleBox;
    public CanvasGroup alphaGroup;
    [SerializeField] private TypeWriterEffect typeWriter;
    [SerializeField] private float fadeFactor = 0.1f;

    private void Awake()
    {
        textBackground.enabled = false;
        subtitleBox.enabled = false;
    }

    public void UpdateStatus(bool status)
    {
        StartCoroutine(FadeCanvasRoutine(status));
    }

    private IEnumerator FadeCanvasRoutine(bool status)
    {
        if (status)
        {
            textBackground.enabled = true;
            subtitleBox.enabled = true;
        }

        yield return StartCoroutine(FadeCanvas(status));

        if (!status)
        {
            textBackground.enabled = false;
            subtitleBox.enabled = false;
        }
    }

    private IEnumerator FadeCanvas(bool status)
    {
        float targetAlpha = status ? 1f : 0f;

        while (!Mathf.Approximately(alphaGroup.alpha, targetAlpha))
        {
            // Calcula la dirección (subida o bajada)
            float direction = status ? 1f : -1f;

            // Ajusta el alpha usando el tiempo real
            alphaGroup.alpha += direction * fadeFactor * Time.deltaTime;

            // Clamp para que no se pase del rango [0,1]
            alphaGroup.alpha = Mathf.Clamp01(alphaGroup.alpha);

            yield return null;
        }

        // Asegura el valor final
        alphaGroup.alpha = targetAlpha;
    }

    public void RunText(string text, float audioLength)
    {
        subtitleBox.text = ""; // Por si acaso limpia antes
        typeWriter.StartTyping(text, audioLength);
    }
}
