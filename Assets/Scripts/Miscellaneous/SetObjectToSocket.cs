using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SetObjectToSocket : NetworkBehaviour
{
    [SerializeField] GameObject valvePrefab;
    private XRSocketInteractor socket;
    private GameObject connectedObject;
    [SyncVar] public GameObject connectedValve;

    public DoorValve door;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    public void EnableSteeringValve()
    {
        if (!socket.socketActive) { return; }

        // Obtener el objeto interactivo conectado al socket
        connectedObject = socket.GetOldestInteractableSelected().transform.gameObject;
        GameObject obj = connectedObject.transform.gameObject;

        // Si es el servidor, realizamos la acción directamente
        if (isServer)
        {
            SpawnValveAndHandleSocket(obj);
        }
        else
        {
            // Si es un cliente, enviamos una comanda al servidor
            CmdSpawnValveAndHandleSocket(obj);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnValveAndHandleSocket(GameObject obj)
    {
        SpawnValveAndHandleSocket(obj);
    }

    void SpawnValveAndHandleSocket(GameObject obj)
    {
        // Instanciar la válvula especial que puede girar
        GameObject valve = Instantiate(valvePrefab, socket.attachTransform.position,
            Quaternion.Euler(socket.attachTransform.rotation.eulerAngles.x,
                             socket.attachTransform.rotation.eulerAngles.y,
                             socket.attachTransform.rotation.eulerAngles.z - 90f));

        // Sincronizar la nueva válvula en la red
        NetworkServer.Spawn(valve);

        // Pasar la referencia de la válvula al DoorController si es necesario
        door = FindObjectOfType<DoorValve>();

        // Destruir el objeto original en la red
        NetworkServer.Destroy(obj);

        connectedValve = valve;

        // Desactivar el socket para evitar más interacciones
        socket.socketActive = false;
    }
}
