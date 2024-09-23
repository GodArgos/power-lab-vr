    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ValveSteering : XRBaseInteractable
{
    [SerializeField] private Transform wheelTransform;
    [SerializeField] private float rotationSensitivity = 2.0f; // Factor de sensibilidad, ajustable desde el inspector
    [SerializeField] public bool testMode = false; // Activar la rotación de prueba
    [SerializeField] public float testRotationSpeed = 10.0f; // Velocidad de rotación para la prueba

    public UnityEvent<float> OnWheelRotated;

    public float currentAngle = 0.0f;

    private bool isLocked = false; // Estado de bloqueo

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (isLocked) return; // No permitir interacción si está bloqueada
        base.OnSelectEntered(args);
        currentAngle = FindWheelAngle();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (isLocked) return; // No permitir interacción si está bloqueada
        base.OnSelectExited(args);
        currentAngle = FindWheelAngle();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (testMode)
            {
                RotateWheelTest();
            }
            else if (isSelected)
            {
                RotateWheel();
            }
        }
    }

    private void RotateWheel()
    {
        float totalAngle = FindWheelAngle();

        // Calcular la diferencia de ángulo usando Mathf.DeltaAngle
        float angleDifference = Mathf.DeltaAngle(currentAngle, totalAngle);

        // Aplicar la rotación
        wheelTransform.Rotate(transform.forward, -angleDifference, Space.World);

        // Almacenar el ángulo para el siguiente proceso
        currentAngle = totalAngle;
        Debug.Log(angleDifference);
        OnWheelRotated?.Invoke(angleDifference);
    }

    private void RotateWheelTest()
    {
        // Aplicar una rotación constante a la válvula si está en modo de prueba
        float testAngle = testRotationSpeed * Time.deltaTime;
        wheelTransform.Rotate(transform.forward, testAngle, Space.World);

        // Invocar el evento como si la válvula hubiera sido girada
        OnWheelRotated?.Invoke(testAngle);
    }

    private float FindWheelAngle()
    {
        float totalAngle = 0;

        // Combinar direcciones de los interactores actuales
        foreach (IXRSelectInteractor interactor in interactorsSelecting)
        {
            Vector2 direction = FindLocalPoint(interactor.transform.position);
            totalAngle += ConvertToAngle(direction) * FindRotationSensitivity();
        }

        return totalAngle;
    }

    private Vector2 FindLocalPoint(Vector3 position)
    {
        // Convertir las posiciones de las manos a local para calcular el ángulo
        return transform.InverseTransformPoint(position).normalized;
    }

    private float ConvertToAngle(Vector2 direction)
    {
        // Usar una dirección consistente para encontrar el ángulo
        return Vector2.SignedAngle(Vector2.up, direction);
    }

    private float FindRotationSensitivity()
    {
        // Usar una menor sensibilidad de rotación con dos manos
        return 1.0f / interactorsSelecting.Count;
    }

    // Método para bloquear la válvula
    public void LockValve()
    {
        isLocked = true; // Cambiar el estado a bloqueado
    }
}
