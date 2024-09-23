using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorValve : MonoBehaviour
{
    [SerializeField] private Transform doorTransform; // La puerta que se moverá
    [SerializeField] private float maxDoorHeight = 5.0f; // Altura máxima de la puerta
    [SerializeField] private float doorSpeed = 2.0f; // Velocidad de la puerta

    private float targetDoorHeight = 0.0f; // Altura objetivo de la puerta
    private bool valvesLocked = false; // Si las válvulas están bloqueadas

    private ValveSteering[] valves = new ValveSteering[2]; // Asumimos que hay exactamente dos válvulas
    private int valveCount = 0; // Contador de válvulas registradas

    private void Update()
    {
        // No hacer nada si las válvulas están bloqueadas o no hay dos válvulas registradas
        if (valvesLocked || valveCount < 2)
            return;

        // Mover la puerta según la rotación de las válvulas
        MoveDoor();

        // Bloquear las válvulas si la puerta ha alcanzado la altura máxima
        if (doorTransform.localPosition.y >= maxDoorHeight)
        {
            LockValves(); // Bloquear las válvulas para que no se puedan girar más
        }
    }

    public void RegisterValve(ValveSteering valve)
    {
        if (valveCount >= 2) return; // Solo queremos dos válvulas

        valves[valveCount] = valve;
        valveCount++;

        // Suscribirse al evento OnWheelRotated de cada válvula
        valve.OnWheelRotated.AddListener((rotation) => OnValveRotated());
    }

    // Método para manejar la rotación de las válvulas
    private void OnValveRotated()
    {
        // Comprobar la rotación de ambas válvulas
        float rotation1 = NormalizeRotation(valves[0].currentAngle);
        float rotation2 = NormalizeRotation(valves[1].currentAngle);

        if (rotation1 > 0 && rotation2 > 0)
        {
            // Ambas válvulas giran en sentido horario: subir la puerta
            targetDoorHeight = Mathf.Min(maxDoorHeight, targetDoorHeight + Time.deltaTime * doorSpeed);
        }
        else if (rotation1 < 0 && rotation2 < 0)
        {
            // Ambas válvulas giran en sentido antihorario: bajar la puerta
            targetDoorHeight = Mathf.Max(0, targetDoorHeight - Time.deltaTime * doorSpeed);
        }
        else
        {
            // Si no giran en el mismo sentido, la puerta baja
            targetDoorHeight = Mathf.Max(0, targetDoorHeight - Time.deltaTime * doorSpeed);
        }
    }

    // Método para normalizar la rotación entre -360 y 360 grados
    private float NormalizeRotation(float angle)
    {
        while (angle > 360) angle -= 360;
        while (angle < -360) angle += 360;
        return angle;
    }

    // Método para mover la puerta
    private void MoveDoor()
    {
        // Aplicar el movimiento de la puerta
        Vector3 currentPosition = doorTransform.localPosition;
        float newY = Mathf.Lerp(currentPosition.y, targetDoorHeight, Time.deltaTime * doorSpeed);
        doorTransform.localPosition = new Vector3(currentPosition.x, newY, currentPosition.z);
    }

    // Método para bloquear las válvulas
    private void LockValves()
    {
        valvesLocked = true; // Bloquear las válvulas globalmente

        foreach (ValveSteering valve in valves)
        {
            // Deshabilitar la interacción con las válvulas
            valve.LockValve(); // Usar un método específico en ValveSteering para bloquearla
        }
    }
}
