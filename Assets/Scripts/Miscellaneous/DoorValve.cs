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
    [SerializeField] private SoundPlayer soundPlayer;
    private bool goingDown = true;
    private bool goingUp = false;
    [SerializeField] private VoiceTriggerNetworked VoiceTriggerNetworked;

    [SyncVar]
    public bool isDoorAtTarget = false;

    void Update()
    {
        if (doubleWork)
        {
            if (valve1.connectedValve != null && valve2.connectedValve != null && !isDoorAtTarget)
            {
                float rotation1 = GetValveRotation(valve1);
                float rotation2 = GetValveRotation(valve2);

                // Comprobar si ambas válvulas están siendo manipuladas
                bool bothAtMaxRot = rotation1 >= 1 && rotation2 >= 1;

                // Verificar si alguna válvula ha dejado de ser manipulada
                bool valve1Stopped = !valve1.connectedValve.GetComponent<ValveRotationSync>().isBeingHeld;
                bool valve2Stopped = !valve2.connectedValve.GetComponent<ValveRotationSync>().isBeingHeld;

                if (bothAtMaxRot && !valve1Stopped && !valve2Stopped && !isDoorAtTarget)
                {
                    if (goingDown && !goingUp)
                    {
                        soundPlayer.CmdStopSoundForAll();
                        soundPlayer.CmdPlaySoundForAll("metaldoor_open");
                        goingUp = true;
                        goingDown = false;
                    }
                    
                    // Subir la puerta
                    door.Translate(Vector3.up * doorSpeed * Time.deltaTime);

                    // Verificar si la puerta ha alcanzado la altura máxima
                    if (door.localPosition.y >= maxHeight)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, maxHeight, door.localPosition.z);
                        LockValves();

                        if (soundPlayer.audioSource.isPlaying)
                        {
                            soundPlayer.CmdStopSoundForAll();
                        }

                        if (VoiceTriggerNetworked != null)
                            VoiceTriggerNetworked.CmdHandleVoiceTrigger();
                    }
                }
                else
                {
                    if (goingUp && !goingDown)
                    {
                        soundPlayer.CmdStopSoundForAll();
                        soundPlayer.CmdPlaySoundForAll("metaldoor_close");
                        goingDown = true;
                        goingUp = false;
                    }

                    // Bajar la puerta lentamente para permitir correcciones si es necesario
                    door.Translate(Vector3.down * (doorSpeed / 2) * Time.deltaTime);

                    // Limitar la puerta a la posición mínima
                    if (door.localPosition.y <= 0)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, 0, door.localPosition.z);

                        if (soundPlayer.audioSource.isPlaying)
                        {
                            soundPlayer.CmdStopSoundForAll();
                        }
                    }

                    // Reiniciar las válvulas si alguna ha dejado de ser manipulada
                    if (valve1Stopped)
                    {
                        valve1.connectedValve.GetComponent<ValveRotationSync>().ResetValve();
                    }

                    if (valve2Stopped)
                    {
                        valve2.connectedValve.GetComponent<ValveRotationSync>().ResetValve();
                    }
                }
            }
        }
        else // Solo una válvula
        {
            if (valve1.connectedValve != null)
            {
                float rotation1 = GetValveRotation(valve1);

                bool atMaxRot = rotation1 >= 1;
                bool valve1Stopped = !valve1.connectedValve.GetComponent<ValveRotationSync>().isBeingHeld;

                if (atMaxRot && !valve1Stopped)
                {
                    if (goingDown && !goingUp)
                    {
                        soundPlayer.CmdStopSoundForAll();
                        soundPlayer.CmdPlaySoundForAll("metaldoor_open");
                        goingUp = true;
                        goingDown = false;
                    }

                    // Subir la puerta
                    door.Translate(Vector3.up * doorSpeed * Time.deltaTime);

                    if (door.localPosition.y >= maxHeight)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, maxHeight, door.localPosition.z);

                        if (soundPlayer.audioSource.isPlaying)
                        {
                            soundPlayer.CmdStopSoundForAll();
                        }

                        if (VoiceTriggerNetworked != null)
                            VoiceTriggerNetworked.CmdHandleVoiceTrigger();
                    }
                }
                else
                {
                    if (goingUp && !goingDown)
                    {
                        soundPlayer.CmdStopSoundForAll();
                        soundPlayer.CmdPlaySoundForAll("metaldoor_close");
                        goingDown = true;
                        goingUp = false;
                    }

                    // Bajar la puerta
                    door.Translate(Vector3.down * (doorSpeed / 2) * Time.deltaTime);

                    if (door.localPosition.y <= 0)
                    {
                        door.localPosition = new Vector3(door.localPosition.x, 0, door.localPosition.z);

                        if (soundPlayer.audioSource.isPlaying)
                        {
                            soundPlayer.CmdStopSoundForAll();
                        }
                    }

                    // Reiniciar la válvula si se ha dejado de manipular
                    if (valve1Stopped)
                    {
                        valve1.connectedValve.GetComponent<ValveRotationSync>().ResetValve();
                    }
                }
            }
        }
    }

    private float GetValveRotation(SetObjectToSocket valve)
    {
        ValveRotationSync knobSync = valve.connectedValve.GetComponent<ValveRotationSync>();
        if (knobSync != null)
        {
            // Obtener el valor sincronizado y limitarlo entre 0 y 1
            return Mathf.Clamp(knobSync.GetSyncedValue(), 0.0f, 1.0f);
        }
        return 0.0f;
    }

    void LockValves()
    {
        isDoorAtTarget = true;

        if (isServer)
        {
            RpcLockValves();
        }
    }

    [ClientRpc]
    void RpcLockValves()
    {
        DisableValveInteraction(valve1);
        DisableValveInteraction(valve2);
    }

    private void DisableValveInteraction(SetObjectToSocket valve)
    {
        if (valve.connectedValve != null)
        {
            XRKnob knob = valve.connectedValve.GetComponent<XRKnob>();
            if (knob != null)
            {
                knob.enabled = false;
            }
        }
    }
}

