using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ValveRotationSync : NetworkBehaviour
{
    private XRKnob knob;

    [SyncVar(hook = nameof(OnValueChanged))]
    public float syncedValue;

    [Header("TEST")]
    [SerializeField] private bool activator = false;
    [SyncVar]
    [SerializeField] private bool enableTestMode = false;
    [SerializeField] private float testRotation = 10f;

    private void Start()
    {
        knob = GetComponent<XRKnob>();

        // Suscribirse al evento de cambio de valor del knob
        knob.onValueChange.AddListener(OnKnobValueChanged);
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
                syncedValue = Mathf.PingPong(Time.time * testRotation, 1.0f);
            }
            else
            {
                var val = Mathf.PingPong(Time.time * testRotation, 1.0f);
                CmdSetValue(val);
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
        // Actualizamos el valor sincronizado
        syncedValue = value;
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
}
