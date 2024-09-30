using Mirror;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SliderValueSync : NetworkBehaviour
{
    private XRSlider slider;

    [SyncVar(hook = nameof(OnValueChanged))]
    public float syncedValue = 0.5f; // Valor inicial en 0.5

    [Header("TEST")]
    [SerializeField] private bool activator = false;
    [SyncVar]
    [SerializeField] private bool enableTestMode = false;
    [Range(0f, 1f)]
    [SerializeField] private float testValue = 0.5f;

    private void Start()
    {
        slider = GetComponent<XRSlider>();

        // Suscribirse al evento de cambio de valor del knob
        slider.onValueChange.AddListener(OnSliderValueChanged);
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
    }

    // Método llamado cuando el valor del slider cambia localmente
    private void OnSliderValueChanged(float value)
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
        syncedValue = value;
    }

    // Método para obtener el valor sincronizado desde otros scripts
    public float GetSyncedValue()
    {
        return syncedValue;
    }

    // Método que se llama en los clientes cuando el valor sincronizado cambia
    private void OnValueChanged(float oldValue, float newValue)
    {
        // Actualizar el valor del slider sin notificar para evitar bucles
        slider.SetValueWithoutNotify(newValue);
    }

    // Método para reiniciar el valor desde el servidor
    [Server]
    public void ResetValue(float value)
    {
        syncedValue = value;
    }
}
