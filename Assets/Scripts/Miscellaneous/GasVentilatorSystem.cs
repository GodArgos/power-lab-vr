using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasVentilatorSystem : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_particleSystem;
    [SerializeField] private float m_cycleTime = 1f;
    [SerializeField] private float m_durationTime = 1f;
    [SerializeField] private bool m_playOnAwake = true;
    private float m_timer;
    private BoxCollider m_collider;
    private bool m_state = true;
    private bool m_firstCycleSkipped = false; // Variable para controlar si se ha saltado el primer ciclo

    private void Start()
    {
        m_collider = GetComponent<BoxCollider>();

        if (m_playOnAwake)
        {
            m_timer = m_cycleTime; // Iniciar el ciclo inmediatamente
            m_firstCycleSkipped = true; // Ya reproducimos en awake, así que saltamos el primer ciclo
        }
        else
        {
            m_timer = 0; // Empezar directamente con el estado inactivo
        }

        foreach (ParticleSystem p in m_particleSystem)
        {
            p.Stop();
            var main = p.main;
            main.duration = m_durationTime;

            if (m_playOnAwake)
            {
                p.Play();
            }
        }
    }

    private void Update()
    {
        m_timer -= Time.deltaTime;

        if (m_timer <= 0)
        {
            // Si el primer ciclo debe ser saltado, lo manejamos aquí
            if (!m_firstCycleSkipped && !m_playOnAwake)
            {
                m_firstCycleSkipped = true; // Marcar que ya hemos saltado el primer ciclo
                m_timer = m_cycleTime; // Resetear el timer para el próximo ciclo
                return; // Saltar este ciclo
            }

            m_state = !m_state; // Cambia el estado

            if (m_state)
            {
                // Activar el collider y reproducir las partículas
                m_collider.enabled = true;
                foreach (ParticleSystem ps in m_particleSystem)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }
                }
            }
            else
            {
                // Detener las partículas gradualmente
                StartCoroutine(StopParticlesAndDisableCollider());
            }

            // Reiniciar el temporizador
            m_timer = m_cycleTime;
        }
    }

    private IEnumerator StopParticlesAndDisableCollider()
    {
        // Detener la emisión de partículas
        foreach (ParticleSystem ps in m_particleSystem)
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting); // Detiene la emisión pero permite que las partículas actuales terminen
        }

        // Esperar hasta que todas las partículas hayan desaparecido
        bool allParticlesStopped = false;
        while (!allParticlesStopped)
        {
            allParticlesStopped = true;
            foreach (ParticleSystem ps in m_particleSystem)
            {
                if (ps.IsAlive(true)) // Verifica si el sistema de partículas sigue "vivo"
                {
                    allParticlesStopped = false;
                    break;
                }
            }
            yield return null; // Espera un frame antes de volver a verificar
        }

        // Desactivar el collider cuando todas las partículas hayan desaparecido
        m_collider.enabled = false;
    }
}
