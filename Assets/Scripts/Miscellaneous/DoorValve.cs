using UnityEngine;
using Mirror;
using UnityEngine.XR.Content.Interaction;

public class DoorValve : NetworkBehaviour
{
    public Transform door; // El objeto de la puerta
    public float doorSpeed = 2.0f; // Velocidad de movimiento de la puerta
    public float maxHeight = 5.0f; // Altura máxima que la puerta puede alcanzar
    public bool doubleWork = true;
    public SetObjectToSocket valve1; // Referencia a la primera válvula
    public SetObjectToSocket valve2; // Referencia a la segunda válvula
    public float valveReturnSpeed = 1.0f; // Velocidad a la que las válvulas vuelven al valor 0

    // Variables para el modo de prueba
    public bool enableTestMode = false; // Habilitar el modo de prueba
    public float testRotationSpeed = 30.0f; // Velocidad de rotación para el modo de prueba

    [SyncVar]
    private float prevRotation1 = 0.0f;
    [SyncVar]
    private float prevRotation2 = 0.0f;
    [SyncVar]
    public bool isDoorAtTarget = false;

    void Update()
    {
        if (doubleWork)
        {
            if (valve1.connectedValve != null && valve2.connectedValve != null && !isDoorAtTarget)
            {
                float rotation1 = 0.0f;
                float rotation2 = 0.0f;

                if (enableTestMode)
                {
                    rotation1 = Mathf.PingPong(Time.time * testRotationSpeed, 1.0f);
                    rotation2 = Mathf.PingPong(Time.time * testRotationSpeed, 1.0f);
                }
                else
                {
                    ValveRotationSync knobSync1 = valve1.connectedValve.GetComponent<ValveRotationSync>();
                    ValveRotationSync knobSync2 = valve2.connectedValve.GetComponent<ValveRotationSync>();

                    if (knobSync1 != null && knobSync2 != null)
                    {
                        rotation1 = knobSync1.GetSyncedValue();
                        rotation2 = knobSync2.GetSyncedValue();
                    }
                }

                // Verificar si ambas válvulas están girando
                bool bothClockwise = rotation1 > 0.01f && rotation2 > 0.01f;

                // Verificar si alguna válvula dejó de girar
                bool valve1Stopped = Mathf.Approximately(rotation1, prevRotation1);
                bool valve2Stopped = Mathf.Approximately(rotation2, prevRotation2);

                if (bothClockwise && !isDoorAtTarget && !valve1Stopped && !valve2Stopped)
                {
                    // Subir la puerta
                    door.Translate(Vector3.up * doorSpeed * Time.deltaTime);

                    // Verificar si la puerta ha alcanzado la altura máxima
                    if (door.localPosition.y >= maxHeight)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, maxHeight, door.localPosition.z);
                        LockValves();
                    }
                }
                else
                {
                    // Bajar la puerta
                    door.Translate(Vector3.down * doorSpeed * Time.deltaTime);

                    // Limitar la puerta a la posición mínima
                    if (door.localPosition.y <= 0)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, 0, door.localPosition.z);
                    }
                }

                // Actualizar las rotaciones previas
                prevRotation1 = rotation1;
                prevRotation2 = rotation2;
            }
        }
        else // Solo una válvula
        {
            if (valve1.connectedValve != null)
            {
                float rotation1 = 0.0f;

                if (enableTestMode)
                {
                    rotation1 = Mathf.PingPong(Time.time * testRotationSpeed, 1.0f);
                }
                else
                {
                    ValveRotationSync knobSync1 = valve1.connectedValve.GetComponent<ValveRotationSync>();

                    if (knobSync1 != null)
                    {
                        rotation1 = knobSync1.GetSyncedValue();
                    }
                }

                bool clockwise = rotation1 > 0.01f;
                bool valve1Stopped = Mathf.Approximately(rotation1, prevRotation1);

                if (clockwise && !valve1Stopped)
                {
                    // Subir la puerta
                    door.Translate(Vector3.up * doorSpeed * Time.deltaTime);

                    if (door.localPosition.y >= maxHeight)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, maxHeight, door.localPosition.z);
                        // LockValves(); // Si es necesario
                    }
                }
                else
                {
                    // Bajar la puerta
                    door.Translate(Vector3.down * doorSpeed * Time.deltaTime);

                    if (door.localPosition.y <= 0)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, 0, door.localPosition.z);
                    }
                }

                prevRotation1 = rotation1;
            }
        }
    }

    void LockValves()
    {
        isDoorAtTarget = true;

        if (isServer)
        {
            RpcLockValves();
        }
        else if (enableTestMode)
        {
            // En modo de prueba, también bloqueamos las válvulas localmente
            if (valve1.connectedValve != null)
            {
                XRKnob knob1 = valve1.connectedValve.GetComponent<XRKnob>();
                if (knob1 != null)
                {
                    knob1.enabled = false;
                }
            }

            if (valve2.connectedValve != null)
            {
                XRKnob knob2 = valve2.connectedValve.GetComponent<XRKnob>();
                if (knob2 != null)
                {
                    knob2.enabled = false;
                }
            }
        }
    }

    [ClientRpc]
    void RpcLockValves()
    {
        if (valve1.connectedValve != null)
        {
            XRKnob knob1 = valve1.connectedValve.GetComponent<XRKnob>();
            if (knob1 != null)
            {
                knob1.enabled = false;
            }
        }

        if (valve2.connectedValve != null)
        {
            XRKnob knob2 = valve2.connectedValve.GetComponent<XRKnob>();
            if (knob2 != null)
            {
                knob2.enabled = false;
            }
        }
    }
}


