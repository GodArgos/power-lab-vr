using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SideLiftController : MonoBehaviour
{
    [SerializeField]
    private float pointA; // Punto A
    [SerializeField]
    private float pointB; // Punto B
    [SerializeField]
    private float maxSpeed = 10f; // Velocidad máxima cuando el slider está al máximo valor
    [SerializeField]
    private XRSlider sliderA; // Referencia al slider A
    [SerializeField]
    private XRSlider sliderB; // Referencia al slider B
    [SerializeField]
    private Axis movementAxis = Axis.X; // Eje en el cual se moverá el objeto

    private float currentSpeed = 0f; // Velocidad actual del objeto
    private float targetPosition; // Objetivo de la posición a moverse
    private bool movingTowardsB = false; // Indica si el objeto se está moviendo hacia B o hacia A

    private enum Axis
    {
        X,
        Y,
        Z
    }

    void Update()
    {
        // Revisamos el valor del slider A y B para determinar el destino y la velocidad
        if (sliderA.value <= 0.45f)
        {
            // Si sliderA está entre 0 y 0.45, el objeto regresa a A con velocidad proporcional
            movingTowardsB = false;
            targetPosition = pointA;
            currentSpeed = maxSpeed * (0.45f - sliderA.value) / 0.45f; // Velocidad creciente al acercarse a 0
        }
        else if (sliderA.value >= 0.66f)
        {
            // Si sliderA está entre 0.66 y 1, el objeto va a B con velocidad proporcional
            movingTowardsB = true;
            targetPosition = pointB;
            currentSpeed = maxSpeed * (sliderA.value - 0.66f) / 0.34f; // Velocidad creciente al acercarse a 1
        }
        else if (sliderB.value <= 0.45f)
        {
            // Si sliderB está entre 0 y 0.45, el objeto regresa a B con velocidad proporcional
            movingTowardsB = false;
            targetPosition = pointB;
            currentSpeed = maxSpeed * (0.45f - sliderB.value) / 0.45f; // Velocidad creciente al acercarse a 0
        }
        else if (sliderB.value >= 0.66f)
        {
            // Si sliderB está entre 0.66 y 1, el objeto va a A con velocidad proporcional
            movingTowardsB = true;
            targetPosition = pointA;
            currentSpeed = maxSpeed * (sliderB.value - 0.66f) / 0.34f; // Velocidad creciente al acercarse a 1
        }
        else
        {
            // Si cualquiera de los sliders está entre 0.46 y 0.65, el objeto se detiene
            currentSpeed = 0f;
        }

        // Mover el objeto si la velocidad es mayor que cero y no ha alcanzado el destino
        if (currentSpeed > 0 && !HasReachedTarget())
        {
            MoveObject();
        }

        // Revisamos si ha llegado al objetivo (A o B), y si es así, reiniciamos los sliders
        if (HasReachedTarget())
        {
            ResetSliders();
        }
    }

    // Función que mueve el objeto hacia el objetivo
    void MoveObject()
    {
        // Obtener la posición actual en el eje seleccionado
        float currentPosition = GetCurrentPosition();

        // Calcular la nueva posición usando Mathf.MoveTowards
        float newPosition = Mathf.MoveTowards(currentPosition, targetPosition, currentSpeed * Time.deltaTime);

        // Actualizar la posición del objeto en el eje correspondiente
        SetCurrentPosition(newPosition);
    }

    // Función para verificar si el objeto ha alcanzado su objetivo
    bool HasReachedTarget()
    {
        float currentPosition = GetCurrentPosition();
        return Mathf.Approximately(currentPosition, targetPosition);
    }

    // Obtener la posición actual en el eje seleccionado (X, Y o Z)
    float GetCurrentPosition()
    {
        switch (movementAxis)
        {
            case Axis.X:
                return transform.localPosition.x;
            case Axis.Y:
                return transform.localPosition.y;
            case Axis.Z:
                return transform.localPosition.z;
            default:
                return 0f;
        }
    }

    // Establecer la nueva posición en el eje seleccionado
    void SetCurrentPosition(float newPosition)
    {
        Vector3 localPos = transform.localPosition;

        // Asignar el nuevo valor de la posición en el eje correspondiente
        switch (movementAxis)
        {
            case Axis.X:
                localPos.x = newPosition;
                break;
            case Axis.Y:
                localPos.y = newPosition;
                break;
            case Axis.Z:
                localPos.z = newPosition;
                break;
        }

        // Actualizar la posición local del objeto
        transform.localPosition = localPos;
    }

    // Función para reiniciar los sliders cuando el objeto llega a un punto
    void ResetSliders()
    {
        sliderA.value = 0.5f;
        sliderB.value = 0.5f;
    }
}
