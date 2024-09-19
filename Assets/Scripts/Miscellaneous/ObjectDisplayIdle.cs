using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisplayIdle : MonoBehaviour
{
    public float alturaMaxima = 1.0f; // Altura máxima de flotación
    public float alturaMinima = 0.0f; // Altura mínima de flotación
    public float velocidad = 2.0f; // Velocidad de flotación

    private float direccion = 1.0f; // Dirección del movimiento

    void Update()
    {
        // Calcula la nueva posición en el eje Y
        float nuevaY = transform.position.y + direccion * velocidad * Time.deltaTime;

        // Verifica si el objeto ha alcanzado la altura máxima o mínima
        if (nuevaY >= alturaMaxima)
        {
            nuevaY = alturaMaxima;
            direccion = -1.0f; // Cambia la dirección hacia abajo
        }
        else if (nuevaY <= alturaMinima)
        {
            nuevaY = alturaMinima;
            direccion = 1.0f; // Cambia la dirección hacia arriba
        }

        // Aplica la nueva posición
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }
}
