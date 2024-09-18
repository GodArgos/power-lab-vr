using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserActivationSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> laserPairs;
    [SerializeField] private float time;
    private int currentPairIndex = 0;

    void Start()
    {
        // Asegúrate de que todas las parejas se inicien activas
        foreach (var laser in laserPairs)
        {
            laser.SetActive(true);
        }

        laserPairs = AssignShotPoints();

        StartCoroutine(ManageLasers());
    }

    private IEnumerator ManageLasers()
    {
        while (true)
        {
            // Desactiva el par actual (dos láseres)
            laserPairs[currentPairIndex].SetActive(false); // Desactiva el láser 1 de la pareja
            laserPairs[currentPairIndex + 1].SetActive(false); // Desactiva el láser 2 de la pareja

            // Espera 1.5 segundos
            yield return new WaitForSeconds(time);

            // Reactiva el par anterior si no es el primero
            if (currentPairIndex > 0)
            {
                laserPairs[currentPairIndex - 1].SetActive(true); // Reactiva el láser 1 de la pareja anterior
                laserPairs[currentPairIndex - 2].SetActive(true); // Reactiva el láser 2 de la pareja anterior
            }

            // Avanza al siguiente par
            currentPairIndex += 2;

            // Reinicia si hemos pasado el último par
            if (currentPairIndex >= laserPairs.Count)
            {
                // Reactiva el último par
                if (currentPairIndex > 0)
                {
                    laserPairs[currentPairIndex - 2].SetActive(true); // Reactiva el láser 1 de la última pareja
                    laserPairs[currentPairIndex - 1].SetActive(true); // Reactiva el láser 2 de la última pareja
                }

                currentPairIndex = 0; // Reinicia al primer par
            }
        }
    }

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
}
