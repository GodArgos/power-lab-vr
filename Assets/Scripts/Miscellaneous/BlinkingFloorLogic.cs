using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingFloorLogic : MonoBehaviour
{
    private MeshRenderer floorRenderer;
    public BoxCollider triggerCollider;
    public BoxCollider floorCollider;
    private Color originalColor;
    public Color blinkColor = Color.red; // Color when blinking
    public float blinkDuration = 1.0f;   // How long it blinks
    public float resetDelay = 5.0f;      // Time before reactivation
    public float fadeDuration = 1.0f;     // Duración de la transición de opacidad
    private bool isActive = true;
    private Material initialMaterial;

    // Nuevo material para el efecto de parpadeo
    [SerializeField] private Material blinkMaterial;

    private void Start()
    {
        floorRenderer = GetComponent<MeshRenderer>();
        originalColor = floorRenderer.material.color;
        initialMaterial = floorRenderer.material;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.gameObject.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            StartCoroutine(BlinkAndDeactivate());
        }
    }

    private IEnumerator BlinkAndDeactivate()
    {
        isActive = false;

        // Cambiar al material de parpadeo
        floorRenderer.material = blinkMaterial;

        // Blink effect
        for (float t = 0; t < blinkDuration; t += Time.deltaTime)
        {
            // Adjust opacity based on time
            float alpha = Mathf.PingPong(t * 5, 1);
            Color newColor = new Color(blinkColor.r, blinkColor.g, blinkColor.b, alpha);
            floorRenderer.material.color = newColor;

            yield return null;
        }

        // Disable the MeshRenderer and Collider
        floorRenderer.enabled = false;
        floorCollider.enabled = false;
        triggerCollider.enabled = false;

        // Wait for reset delay before reactivating
        yield return new WaitForSeconds(resetDelay);

        // Re-enable the MeshRenderer and Collider
        floorRenderer.enabled = true;
        floorCollider.enabled = true;
        triggerCollider.enabled = true;

        // Transición de transparente a visible
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            floorRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Reset material color
        floorRenderer.material = initialMaterial;
        floorRenderer.material.color = originalColor;

        isActive = true;
    }
}
