using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingFloorLogic : MonoBehaviour
{
    private MeshRenderer floorRenderer;
    private Collider floorCollider;
    private Color originalColor;
    public Color blinkColor = Color.red; // Color when blinking
    public float blinkDuration = 1.0f;   // How long it blinks
    public float resetDelay = 5.0f;      // Time before reactivation
    public float fadeDuration = 1.0f;     // Duración de la transición de opacidad

    private bool isActive = true;

    // Debug variable to trigger blinking manually
    public bool debugTrigger = false;

    private void Start()
    {
        floorRenderer = GetComponent<MeshRenderer>();
        floorCollider = GetComponent<BoxCollider>();
        originalColor = floorRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            StartCoroutine(BlinkAndDeactivate());
        }
    }

    private void Update()
    {
        // Check if debugTrigger is set to true
        if (debugTrigger)
        {
            debugTrigger = false; // Reset to prevent continuous triggering
            if (isActive)
            {
                StartCoroutine(BlinkAndDeactivate());
            }
        }
    }

    private IEnumerator BlinkAndDeactivate()
    {
        isActive = false;



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

        // Wait for reset delay before reactivating
        yield return new WaitForSeconds(resetDelay);

        // Re-enable the MeshRenderer and Collider
        floorRenderer.enabled = true;
        floorCollider.enabled = true;

        // Transición de transparente a visible
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            floorRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Reset material color
        floorRenderer.material.color = originalColor;

        isActive = true;
    }
}
