using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ValveRotationSync : NetworkBehaviour
{
    private XRKnob knob;
    private float initialValue = 0f;
    [HideInInspector] public bool brokenValve = false;
    [SyncVar(hook = nameof(OnValueChanged))]
    public float syncedValue;
    [SyncVar]
    public bool isBeingHeld; // Nuevo SyncVar para saber si la válvula está siendo manipulada

    [Header("TEST")]
    [SerializeField] private bool activator = false;
    [SyncVar]
    [SerializeField] private bool enableTestMode = false;
    [SerializeField] private float testRotation = 10f;
    private SoundPlayer soundPlayer;
    private bool soundPlayed = false;


    private void Start()
    {
        knob = GetComponent<XRKnob>();
        // Suscribirse al evento de cambio de valor del knob
        knob.onValueChange.AddListener(OnKnobValueChanged);
        soundPlayer = GetComponent<SoundPlayer>();
    }

    private void Update()
    {
        if (activator && !enableTestMode)
        {
            enableTestMode = true;
        }
        else if (!activator && enableTestMode)
        {
            enableTestMode = false;
        }
        
        if (enableTestMode)
        {
            if (isServer)
            {
                syncedValue = Mathf.PingPong(Time.time * testRotation, 1.0f);
            }
            else
            {
                var val = Mathf.PingPong(Time.time * testRotation, 1.0f);
                CmdSetValue(val);
            }
        }

        if (!brokenValve)
        {
            // Verificar si el valor llegó al máximo para detener el sonido
            if (syncedValue >= 1.0f && !soundPlayed)
            {
                soundPlayer.CmdStopSoundForAll();
                soundPlayed = true;
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Asegurarse de que el knob tenga el valor sincronizado al unirse
        knob.SetValueWithoutNotify(syncedValue);
    }

    // Método llamado cuando el valor del knob cambia localmente
    private void OnKnobValueChanged(float value)
    {
        // Si el cliente tiene autoridad sobre este objeto, enviamos el valor al servidor
        if (isOwned && !enableTestMode)
        {
            CmdSetValue(value);
        }
    }

    // Comando que se ejecuta en el servidor cuando es llamado por un cliente
    [Command(requiresAuthority = false)]
    private void CmdSetValue(float value)
    {
        var oldValue = syncedValue;
        // Actualizamos el valor sincronizado
        syncedValue = value;

        // Reproducir sonido cuando la válvula comienza a girar
        if (syncedValue != oldValue && isBeingHeld && !soundPlayed && !soundPlayer.audioSource.isPlaying)
        {
            soundPlayer.CmdPlayPausableSoundForAll("valve_turn");
        }
        else if (syncedValue == oldValue && soundPlayer.audioSource.isPlaying)
        {
            soundPlayer.CmdPauseSoundForAll();
        }
    }

    // Método que se llama en los clientes cuando el valor sincronizado cambia
    private void OnValueChanged(float oldValue, float newValue)
    {
        // Actualizamos el valor del knob sin notificar para evitar bucles
        knob.SetValueWithoutNotify(newValue);
    }

    // Método para obtener el valor sincronizado desde otros scripts
    public float GetSyncedValue()
    {
        return syncedValue;
    }

    private void OnGrabbed()
    {
        isBeingHeld = true; // La válvula está siendo manipulada
        CmdSetBeingHeld(true);
    }

    private void OnReleased()
    {
        isBeingHeld = false; // La válvula ha dejado de ser manipulada
        CmdSetBeingHeld(false);

        // Pausar sonido cuando deja de girar
        soundPlayer.CmdPauseSoundForAll();
    }

    [Command(requiresAuthority = false)]
    private void CmdSetBeingHeld(bool value)
    {
        isBeingHeld = value;
    }

    // Método para reiniciar la válvula a su valor y rotación inicial
    public void ResetValve()
    {
        if (isServer)
        {
            //syncedValue = initialValue;
            StartCoroutine(ResetValveValue());
        }
        else
        {
            CmdResetValve();
        }
    }

    private IEnumerator ResetValveValue()
    {
        while (syncedValue != initialValue)
        {
            syncedValue -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        
        if (!brokenValve)
        {
            soundPlayer.CmdStopSoundForAll();
        }  
    }

    [Command(requiresAuthority = false)]
    private void CmdResetValve()
    {
        ResetValve();
    }
}
