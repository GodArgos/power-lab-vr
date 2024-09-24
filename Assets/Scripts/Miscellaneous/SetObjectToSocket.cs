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

    public DoorValve door;

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
                            socket.attachTransform.transform.rotation.y,
                            socket.attachTransform.transform.rotation.z - 90f));

        // Pasar la referencia de la válvula al DoorController
        door = FindObjectOfType<DoorValve>();

        // Destruir el objeto original que se puso en el socket
        Destroy(obj);

        // Desactivar el socket para evitar más interacciones
        socket.socketActive = false;
    }
}
