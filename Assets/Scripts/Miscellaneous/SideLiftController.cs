using Mirror;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SideLiftController : NetworkBehaviour
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
    [SerializeField] private SoundPlayer soundPlayer_main;
    [SerializeField] private SoundPlayer soundPlayer_second;

    private float currentSpeed = 0f; // Velocidad actual del objeto
    private float targetPosition; // Objetivo de la posición a moverse
    private Transform previousPlayerTransform;

    private enum Axis
    {
        X,
        Y,
        Z
    }

    void FixedUpdate()
    {
        // Obtener los valores sincronizados de los sliders
        float sliderAValue = 0.5f; // Valor por defecto
        float sliderBValue = 0.5f; // Valor por defecto

        SliderValueSync sliderASync = sliderA.GetComponent<SliderValueSync>();
        SliderValueSync sliderBSync = sliderB.GetComponent<SliderValueSync>();

        if (sliderASync != null)
        {
            sliderAValue = sliderASync.GetSyncedValue();
        }

        if (sliderBSync != null)
        {
            sliderBValue = sliderBSync.GetSyncedValue();
        }

        // Revisamos el valor del slider A y B para determinar el destino y la velocidad
        if (sliderAValue <= 0.45f)
        {
            // El objeto regresa a A con velocidad proporcional
            targetPosition = pointA;
            currentSpeed = maxSpeed * (0.45f - sliderAValue) / 0.45f;
        }
        else if (sliderAValue >= 0.66f)
        {
            // El objeto va a B con velocidad proporcional
            targetPosition = pointB;
            currentSpeed = maxSpeed * (sliderAValue - 0.66f) / 0.34f;
        }
        else if (sliderBValue <= 0.45f)
        {
            // El objeto regresa a B con velocidad proporcional
            targetPosition = pointB;
            currentSpeed = maxSpeed * (0.45f - sliderBValue) / 0.45f;
        }
        else if (sliderBValue >= 0.66f)
        {
            // El objeto va a A con velocidad proporcional
            targetPosition = pointA;
            currentSpeed = maxSpeed * (sliderBValue - 0.66f) / 0.34f;
        }
        else
        {
            // El objeto se detiene
            currentSpeed = 0f;

            if (soundPlayer_main.audioSource.isPlaying)
            {
                soundPlayer_main.CmdPauseSoundForAll();
            }
        }

        // Mover el objeto si la velocidad es mayor que cero y no ha alcanzado el destino
        if (currentSpeed > 0 && !HasReachedTarget())
        {
            if (!soundPlayer_main.audioSource.isPlaying)
            {
                soundPlayer_main.CmdPlayPausableSoundForAll("sidelift");
            }

            MoveObject();
        }

        // Revisamos si ha llegado al objetivo, y si es así, reiniciamos los sliders
        if (HasReachedTarget())
        {
            if (soundPlayer_main.audioSource.isPlaying)
            {
                soundPlayer_main.CmdStopSoundForAll();
                soundPlayer_second.CmdPlaySoundForAll("sidelift_arrive");
            }
            
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            previousPlayerTransform = other.transform.parent;
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(previousPlayerTransform);
        }
    }
}
