using Mirror;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SliderValueSync : NetworkBehaviour
{
    private XRSlider slider;

    [SyncVar(hook = nameof(OnValueChanged))]
    public float syncedValue = 0.5f; // Valor inicial en 0.5
    [SyncVar(hook = nameof(OnIsBeingHeldChanged))]
    public bool isBeingHeld = false; // Nuevo SyncVar para saber si la v�lvula est� siendo manipulada
    [SerializeField] private bool movingSlide = false;

    [Header("TEST")]
    [SerializeField] private bool activator = false;
    [SyncVar]
    [SerializeField] private bool enableTestMode = false;
    // [Range(0f, 1f)]
    [SerializeField] private float testValue = 0.5f;

    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private string audioID;

    private float lastSyncedValue;
    private float changeThreshold = 0.0005f;

    private bool slideAtSide = false;

    private void Start()
    {
        slider = GetComponent<XRSlider>();
        // Suscribirse al evento de cambio de valor del knob
        slider.onValueChange.AddListener(OnSliderValueChanged);
        lastSyncedValue = syncedValue; // Inicializar el valor anterior al valor inicial
    }

    private void Update()
    {
        if (activator && !enableTestMode)
        {
            enableTestMode = true;
        }

        if (enableTestMode)
        {
            if (isServer)
            {
                syncedValue = testValue;
            }
            else
            {
                CmdSetValue(testValue);
            }
        }

        if (movingSlide)
        {
            if (isBeingHeld)
            {
                if (Mathf.Abs(syncedValue - lastSyncedValue) >= changeThreshold)
                {
                    if (!soundPlayer.audioSource.isPlaying)
                    {
                        soundPlayer.CmdPlayPausableSoundForAll(audioID);
                    }
                    lastSyncedValue = syncedValue; // Actualizar el �ltimo valor registrado
                }
                else
                {
                    if (soundPlayer.audioSource.isPlaying)
                    {
                        soundPlayer.CmdPauseSoundForAll();
                    }
                }
            }
            else
            {
                if (soundPlayer.audioSource.isPlaying)
                {
                    soundPlayer.CmdPauseSoundForAll();
                }
            }

            // Pausar sonido cuando el slider est� en los valores extremos
            if (syncedValue <= 0 || syncedValue >= 1.0f)
            {
                soundPlayer.CmdPauseSoundForAll();
            }
        }
        else
        {
            // Reproducir el sonido solo una vez al llegar a los extremos (0 o 1)
            if ((syncedValue <= 0 || syncedValue >= 1.0f) && !slideAtSide)
            {
                if (!soundPlayer.audioSource.isPlaying)
                {
                    soundPlayer.CmdPlaySoundForAll(audioID);
                }
                slideAtSide = true; // Marcar que ya se reprodujo el sonido en el extremo
            }
            else if (syncedValue > 0 && syncedValue < 1.0f)
            {
                slideAtSide = false; // Resetear cuando el slider se aleja de los extremos
            }
        }
    }

    // M�todo llamado cuando el valor del slider cambia localmente
    private void OnSliderValueChanged(float value)
    {
        // Si el cliente tiene autoridad sobre este objeto, enviamos el valor al servidor
        if (isOwned && !enableTestMode)
        {
            CmdSetValue(value);
        }
    }

    private void OnIsBeingHeldChanged(bool oldHeld, bool newHeld)
    {
        isBeingHeld = newHeld;
    }

    // Comando que se ejecuta en el servidor cuando es llamado por un cliente
    [Command(requiresAuthority = false)]
    private void CmdSetValue(float value)
    {
        syncedValue = value;
    }

    // M�todo para obtener el valor sincronizado desde otros scripts
    public float GetSyncedValue()
    {
        return syncedValue;
    }

    // M�todo que se llama en los clientes cuando el valor sincronizado cambia
    private void OnValueChanged(float oldValue, float newValue)
    {
        // Actualizar el valor del slider sin notificar para evitar bucles
        slider.SetValueWithoutNotify(newValue);
    }

    // M�todo para reiniciar el valor desde el servidor
    [Server]
    public void ResetValue(float value)
    {
        syncedValue = value;
        if (movingSlide) { soundPlayer.CmdStopSoundForAll(); }
    }

    public void OnGrabbed()
    {
        if (isServer)
        {
            isBeingHeld = true;
        }
        else
        {
            CmdSetBeingHeld(true);
        }
    }

    public void OnReleased()
    {
        if (isServer)
        {
            isBeingHeld = false;
        }
        else
        {
            CmdSetBeingHeld(false);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetBeingHeld(bool value)
    {
        isBeingHeld = value;
    }
}
