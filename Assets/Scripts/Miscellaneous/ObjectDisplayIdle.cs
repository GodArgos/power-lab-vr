using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisplayIdle : MonoBehaviour
{
    public float alturaMaxima = 1.0f; // Altura máxima de flotación
    public float alturaMinima = 0.0f; // Altura mínima de flotación
    public float velocidad = 2.0f; // Velocidad de flotación

    private float direccion = 1.0f; // Dirección del movimiento

    private Rigidbody rb; // Referencia al Rigidbody

    void Start()
    {
        // Obtiene el Rigidbody del objeto
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        // Calcula la nueva velocidad en el eje Y
        float nuevaY = rb.position.y + direccion * velocidad;

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

        // Aplica la nueva posición usando Rigidbody
        rb.MovePosition(new Vector3(rb.position.x, nuevaY, rb.position.z));
    }

    public void OnSelectObject()
    {
        GetComponent<ObjectDisplayIdle>().enabled = false;
    }
}
