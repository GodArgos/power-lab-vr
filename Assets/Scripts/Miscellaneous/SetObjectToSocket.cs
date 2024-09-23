using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SetObjectToSocket : MonoBehaviour
{
    [SerializeField] GameObject valvePrefab;
    private XRSocketInteractor socket;
    private IXRSelectInteractable connectedObject;
    [HideInInspector] public GameObject connectedValve;

    // Referencia al DoorController
    public DoorValve doorController;

    public bool enableTestMode = false;
    public float testRotationSpeed = 30.0f; // Velocidad de rotación para el modo de prueba

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    public void EnableSteeringValve()
    {
        if (!socket.socketActive) { return; }

        // Obtener el objeto interactivo conectado al socket
        connectedObject = socket.GetOldestInteractableSelected();
        GameObject obj = connectedObject.transform.gameObject;

        // Instanciar la válvula especial que puede girar
        connectedValve = Instantiate(valvePrefab, socket.attachTransform.transform.position,
            Quaternion.Euler(socket.attachTransform.transform.rotation.x,
                            socket.attachTransform.transform.rotation.y - 90f,
                            socket.attachTransform.transform.rotation.z));

        // Configurar el modo de prueba en la válvula instanciada
        ValveSteering valveSteering = connectedValve.GetComponent<ValveSteering>();
        if (valveSteering != null)
        {
            valveSteering.testMode = enableTestMode; // Habilitar o deshabilitar el modo de prueba
            valveSteering.testRotationSpeed = testRotationSpeed; // Ajustar la velocidad del modo de prueba
        }

        // Destruir el objeto original que se puso en el socket
        Destroy(obj);

        // Desactivar el socket para evitar más interacciones
        socket.socketActive = false;

        // Notificar al DoorController que una válvula ha sido instanciada
        if (doorController != null)
        {
            doorController.RegisterValve(valveSteering);
        }
    }
}
