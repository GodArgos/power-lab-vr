using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingMaterial : MonoBehaviour
{
    [SerializeField] private Material m_blinkMaterial;
    [SerializeField] private Color blinkColor = Color.white;
    [SerializeField] private float m_blinkDuration;
    private float m_fadeDuration;
    private bool m_isBlinking = false;
    private MeshRenderer m_renderer;
    private Material m_initialMaterial;

    private void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_initialMaterial = m_renderer.material;
        StartBlinking();
    }

    public void StartBlinking()
    {
        m_isBlinking = true;
        StartCoroutine(Blink());
    }

    public void StopBlinking()
    {
        Debug.Log("Botón Presionada");
        m_isBlinking = false;
        m_renderer.material = m_initialMaterial; // Volver al material original
        StopCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (m_isBlinking)
        {
            // Cambiar al material de parpadeo
            m_renderer.material = m_blinkMaterial;

            // Efecto de parpadeo
            for (float t = 0; t < m_blinkDuration; t += Time.deltaTime)
            {
                if (!m_isBlinking) // Salir del ciclo si se detiene el parpadeo
                    yield break;

                // Ajustar la opacidad según el tiempo
                float alpha = Mathf.PingPong(t / m_fadeDuration, 1);
                Color newColor = new Color(blinkColor.r, blinkColor.g, blinkColor.b, alpha);
                m_renderer.material.color = newColor;

                yield return null; // Esperar hasta el siguiente frame
            }

            // Volver al material original después de cada ciclo
            m_renderer.material = m_initialMaterial;

            yield return new WaitForSeconds(m_blinkDuration); // Esperar entre parpadeos
        }
    }
}
