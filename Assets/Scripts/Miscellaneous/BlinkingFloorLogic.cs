using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingFloorLogic : NetworkBehaviour
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

    [SyncVar]
    public bool networkIsActive = true;

    private SoundPlayer soundPlayer;

    private void Start()
    {
        floorRenderer = GetComponent<MeshRenderer>();
        originalColor = floorRenderer.material.color;
        initialMaterial = floorRenderer.material;
        soundPlayer = transform.GetChild(0).GetComponent<SoundPlayer>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (networkIsActive && other.gameObject.CompareTag("Player"))
        {
            CmdPlayerTouchedFloor();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayerTouchedFloor()
    {
        HandleFloorTouched();
    }

    private void HandleFloorTouched()
    {
        if (networkIsActive)
        {
            StartCoroutine(BlinkAndDeactivate());
        }
    }

    private IEnumerator BlinkAndDeactivate()
    {
        networkIsActive = false;

        // Notificar a todos los clientes para que actualicen el visual
        RpcStartBlinking();

        // Esperar el tiempo de parpadeo
        yield return new WaitForSeconds(blinkDuration);

        soundPlayer.CmdPlaySoundForAll("fall_apart");

        // Desactivar el piso en todos los clientes
        RpcDeactivateFloor();

        // Esperar el tiempo antes de reactivar
        yield return new WaitForSeconds(resetDelay);

        // Reactivar el piso en todos los clientes
        RpcReactivateFloor();

        networkIsActive = true;
    }

    [ClientRpc]
    void RpcStartBlinking()
    {
        if (floorRenderer == null)
            floorRenderer = GetComponent<MeshRenderer>();

        // Cambiar al material de parpadeo
        floorRenderer.material = blinkMaterial;

        // Iniciar el efecto de parpadeo
        //StopAllCoroutines(); // Asegurar que no haya otras corrutinas ejecutándose
        StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        for (float t = 0; t < blinkDuration; t += Time.deltaTime)
        {
            // Adjust opacity based on time
            float alpha = Mathf.PingPong(t * 5, 1);
            Color newColor = new Color(blinkColor.r, blinkColor.g, blinkColor.b, alpha);
            floorRenderer.material.color = newColor;

            yield return null;
        }
    }

    [ClientRpc]
    void RpcDeactivateFloor()
    {
        floorRenderer.enabled = false;
        floorCollider.enabled = false;
        triggerCollider.enabled = false;

        // Iniciar la corrutina para reactivar el piso
        //StartCoroutine(ReactivateFloor());
    }

    [ClientRpc]
    void RpcReactivateFloor()
    {
        // Rehabilitar el renderer y colliders
        floorRenderer.enabled = true;
        floorCollider.enabled = true;
        triggerCollider.enabled = true;

        // Iniciar la transición de opacidad
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // Transición de transparente a visible
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            floorRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Resetear el material y el color
        floorRenderer.material = initialMaterial;
        floorRenderer.material.color = originalColor;
    }
}
