//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LaserActivationSystem : NetworkBehaviour
//{
//    [SerializeField] private List<GameObject> laserPairs;
//    [SerializeField] private float timeBetweenSwitches = 1.5f;

//    [SerializeField] private List<GameObject> shotPoints; // Lista de puntos de disparo

//    // Variables sincronizadas
//    [SyncVar] private int currentPairIndex;
//    [SyncVar] private float timer;

//    private void Start()
//    {
//        // Asigna los puntos de disparo
//        shotPoints = AssignShotPoints();

//        // Inicia todos los láseres activos
//        foreach (var laser in laserPairs)
//        {
//            laser.SetActive(true); // Asegúrate de que el objeto del láser esté activo
//        }

//        if (isServer)
//        {
//            timer = timeBetweenSwitches;
//            currentPairIndex = 0;
//        }
//    }

//    private void Update()
//    {
//        if (isServer)
//        {
//            ManageLasers();
//        }

//        // Actualiza el estado de los shotPoints independientemente para clientes
//        UpdateShotPointStates();
//    }

//    private void ManageLasers()
//    {
//        timer -= Time.deltaTime;

//        if (timer <= 0)
//        {
//            Debug.Log($"Desactivando láser en índice: {currentPairIndex}");
//            RpcSetLaserActive(currentPairIndex, false);

//            currentPairIndex += 2;
//            if (currentPairIndex >= laserPairs.Count)
//            {
//                currentPairIndex = 0;
//            }

//            Debug.Log($"Activando láser en índice: {currentPairIndex}");
//            RpcSetLaserActive(currentPairIndex, true);
//            timer = timeBetweenSwitches;
//        }
//    }

//    [ClientRpc]
//    private void RpcSetLaserActive(int index, bool isActive)
//    {
//        if (index < laserPairs.Count && index >= 0)
//        {
//            // Activa/desactiva los puntos de disparo (sin desactivar el láser completo)
//            shotPoints[index].SetActive(isActive);
//            if (index + 1 < shotPoints.Count) // Verifica que no se salga del rango
//            {
//                shotPoints[index + 1].SetActive(isActive); // El par de láseres
//            }
//        }
//    }

//    // Método para reasignar los shot points de los láseres
//    private List<GameObject> AssignShotPoints()
//    {
//        List<GameObject> shootPoints = new List<GameObject>();

//        for (int i = 0; i < laserPairs.Count; i++)
//        {
//            shootPoints.Add(laserPairs[i].transform.GetChild(0).GetChild(2).gameObject);
//            shootPoints.Add(laserPairs[i].transform.GetChild(1).GetChild(2).gameObject);
//        }

//        return shootPoints;
//    }

//    // Método para actualizar el estado de los shot points
//    private void UpdateShotPointStates()
//    {
//        // Verifica los shot points y ajusta el estado sin desactivar el láser completo
//        for (int i = 0; i < shotPoints.Count; i++)
//        {
//            bool shouldBeActive = (i == currentPairIndex || i == currentPairIndex + 1);
//            shotPoints[i].SetActive(shouldBeActive);
//        }
//    }
//}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserActivationSystem : NetworkBehaviour
{
    [SerializeField] private List<GameObject> laserPairs;
    [SerializeField] private float timeBetweenSwitches = 1.5f;

    [SerializeField] private List<GameObject> shotPoints; // Lista de puntos de disparo

    // Variables sincronizadas
    [SyncVar] private int currentPairIndex;
    [SyncVar] private float timer;

    private void Start()
    {
        // Asigna los puntos de disparo
        shotPoints = AssignShotPoints();

        if (isServer)
        {
            // Inicia todos los láseres activos
            foreach (var laser in laserPairs)
            {
                laser.SetActive(true); // Asegúrate de que el objeto del láser esté activo
            }

            // Inicializa el estado del par de láseres
            currentPairIndex = 0;
            timer = timeBetweenSwitches;

            // Llama a todos los clientes para activar los láseres
            RpcActivateLasers();
        }
    }

    private void Update()
    {
        if (isServer)
        {
            ManageLasers();
        }

        // Actualiza el estado de los shotPoints independientemente para clientes
        UpdateShotPointStates();
    }

    private void ManageLasers()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Desactiva el par actual
            RpcSetLaserActive(currentPairIndex, false);

            // Reactiva el par anterior si no es el primero
            if (currentPairIndex > 0)
            {
                RpcSetLaserActive(currentPairIndex - 2, true); // Reactiva el láser 1 de la pareja anterior
                RpcSetLaserActive(currentPairIndex - 1, true); // Reactiva el láser 2 de la pareja anterior
            }

            // Avanza al siguiente par
            currentPairIndex += 2;

            // Reinicia si hemos pasado el último par
            if (currentPairIndex >= shotPoints.Count)
            {
                currentPairIndex = 0; // Reinicia al primer par
            }

            // Desactiva el nuevo par
            RpcSetLaserActive(currentPairIndex, false);

            // Reinicia el temporizador
            timer = timeBetweenSwitches;
        }
    }

    [ClientRpc]
    private void RpcActivateLasers()
    {
        // Activa todos los láseres para los clientes
        foreach (var laser in shotPoints)
        {
            laser.SetActive(true);
        }
    }

    [ClientRpc]
    private void RpcSetLaserActive(int index, bool isActive)
    {
        if (index < laserPairs.Count && index >= 0)
        {
            // Activa/desactiva los puntos de disparo (sin desactivar el láser completo)
            shotPoints[index].SetActive(isActive);
            if (index + 1 < shotPoints.Count) // Verifica que no se salga del rango
            {
                shotPoints[index + 1].SetActive(isActive); // El par de láseres
            }
        }
    }

    // Método para reasignar los shot points de los láseres
    private List<GameObject> AssignShotPoints()
    {
        List<GameObject> shootPoints = new List<GameObject>();

        for (int i = 0; i < laserPairs.Count; i++)
        {
            shootPoints.Add(laserPairs[i].transform.GetChild(0).GetChild(2).gameObject);
            shootPoints.Add(laserPairs[i].transform.GetChild(1).GetChild(2).gameObject);
        }

        return shootPoints;
    }

    // Método para actualizar el estado de los shot points
    private void UpdateShotPointStates()
    {
        // Verifica los shot points y ajusta el estado sin desactivar el láser completo
        for (int i = 0; i < shotPoints.Count; i++)
        {
            bool shouldBeActive = (i == currentPairIndex || i == currentPairIndex + 1);
            shotPoints[i].SetActive(!shouldBeActive);
        }
    }
}
