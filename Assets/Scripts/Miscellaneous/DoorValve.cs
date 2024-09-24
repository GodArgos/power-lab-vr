using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class DoorValve : MonoBehaviour
{
    public Transform door; // El objeto de la puerta
    public float doorSpeed = 2.0f; // Velocidad de movimiento de la puerta
    public float maxHeight = 5.0f; // Altura máxima que la puerta puede alcanzar
    public SetObjectToSocket valve1; // Referencia a la primera válvula
    public SetObjectToSocket valve2; // Referencia a la segunda válvula
    public float valveReturnSpeed = 1.0f; // Velocidad a la que las válvulas vuelven al valor 0

    // Variables para el modo de prueba
    public bool enableTestMode = false; // Habilitar el modo de prueba
    public float testRotationSpeed = 30.0f; // Velocidad de rotación para el modo de prueba

    private float prevRotation1 = 0.0f;
    private float prevRotation2 = 0.0f;
    private bool isDoorAtTarget = false;

    void Update()
    {
        if (valve1.connectedValve != null && valve2.connectedValve != null && !isDoorAtTarget)
        {
            // Variables de rotación de las válvulas
            float rotation1, rotation2;

            // Modo de prueba activado
            if (enableTestMode)
            {
                // Simular rotación incrementando el valor con testRotationSpeed
                rotation1 = Mathf.PingPong(Time.deltaTime * testRotationSpeed, 1.0f); // Simula rotación entre 0 y 1
                rotation2 = Mathf.PingPong(Time.deltaTime * testRotationSpeed, 1.0f); // Simula rotación entre 0 y 1
            }
            else
            {
                // Obtener las rotaciones actuales de ambas válvulas en modo normal
                rotation1 = valve1.connectedValve.GetComponent<XRKnob>().value;
                rotation2 = valve2.connectedValve.GetComponent<XRKnob>().value;
            }

            // Verificar si las válvulas están girando en sentido horario (rotación > 0.5)
            bool bothClockwise = rotation1 > 0.01f && rotation2 > 0.01;

            // Verificar si alguna válvula dejó de girar (comparando rotaciones actuales con las anteriores)
            bool valve1Stopped = Mathf.Approximately(rotation1, prevRotation1);
            bool valve2Stopped = Mathf.Approximately(rotation2, prevRotation2);

            if (bothClockwise && !isDoorAtTarget && !valve1Stopped && !valve2Stopped)
            {
                // Subir la puerta si ambas válvulas están girando en sentido horario y ninguna se ha detenido
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
                // Bajar la puerta si alguna válvula dejó de girar o si alguna gira en sentido antihorario
                door.Translate(Vector3.down * doorSpeed * Time.deltaTime);

                // Limitar la puerta a la posición mínima (suelo)
                if (door.localPosition.y <= 0)
                {
                    door.localPosition = new Vector3(door.localPosition.x, 0, door.localPosition.z);
                }

                // Retroceder las válvulas gradualmente hacia el valor 0 si se detuvieron
                if (!enableTestMode && valve1Stopped)
                {
                    rotation1 = Mathf.MoveTowards(rotation1, 0, valveReturnSpeed * Time.deltaTime);
                    valve1.connectedValve.GetComponent<XRKnob>().SetValueWithoutNotify(rotation1);
                }

                if (!enableTestMode && valve2Stopped)
                {
                    rotation2 = Mathf.MoveTowards(rotation2, 0, valveReturnSpeed * Time.deltaTime);
                    valve2.connectedValve.GetComponent<XRKnob>().SetValueWithoutNotify(rotation2);
                }
            }

            // Actualizar las rotaciones previas para la siguiente verificación
            prevRotation1 = rotation1;
            prevRotation2 = rotation2;
        }
    }

    void LockValves()
    {
        isDoorAtTarget = true;
        // Desactivar la rotación de ambas válvulas
        valve1.connectedValve.GetComponent<XRKnob>().enabled = false;
        valve2.connectedValve.GetComponent<XRKnob>().enabled = false;
    }
}
