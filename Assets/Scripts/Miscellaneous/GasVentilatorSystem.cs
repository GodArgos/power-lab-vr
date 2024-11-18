using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GasVentilatorSystem : NetworkBehaviour
{
    [SerializeField] private List<ParticleSystem> m_particleSystem;
    [SerializeField] private float m_cycleTime = 1f;
    [SerializeField] private float m_durationTime = 1f;
    [SerializeField] private bool m_playOnAwake = true;
    [SerializeField] private SoundPlayer m_soundPlayer;

    private float m_timer;
    private BoxCollider m_collider;

    [SyncVar(hook = nameof(OnStateChanged))]
    private bool m_state = true;

    private bool m_isWaitingForParticles = false; // Control para pausar el ciclo

    private void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        m_timer = m_playOnAwake ? m_cycleTime : 0;

        foreach (ParticleSystem ps in m_particleSystem)
        {
            ps.Stop();
            var main = ps.main;
            main.duration = m_durationTime;

            if (m_playOnAwake) ps.Play();
        }
    }

    private void Update()
    {
        if (!isServer || m_isWaitingForParticles) return;

        m_timer -= Time.deltaTime;

        if (m_timer <= 0)
        {
            m_state = !m_state; // Cambia el estado (esto activa OnStateChanged)
            m_timer = m_cycleTime; // Reinicia el temporizador
        }
    }

    private void OnStateChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            PlayParticles();
            m_collider.enabled = true;
            m_soundPlayer.CmdPlayPausableSoundForAll("gas_leaking");
        }
        else
        {
            StartCoroutine(StopParticlesAndDisableCollider());
        }
    }

    private void PlayParticles()
    {
        foreach (ParticleSystem ps in m_particleSystem)
        {
            if (!ps.isPlaying) ps.Play();
        }
    }

    private IEnumerator StopParticlesAndDisableCollider()
    {
        // Pausa el ciclo mientras las partículas se detienen
        m_isWaitingForParticles = true;

        foreach (ParticleSystem ps in m_particleSystem)
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        // Espera hasta que todas las partículas estén completamente detenidas
        while (true)
        {
            bool allStopped = true;
            foreach (ParticleSystem ps in m_particleSystem)
            {
                if (ps.IsAlive(true))
                {
                    allStopped = false;
                    break;
                }
            }

            if (allStopped) break;
            yield return null;
        }

        m_collider.enabled = false; // Desactiva el collider
        m_soundPlayer.CmdStopSoundForAll();

        // Reanuda el ciclo una vez que todo está detenido
        m_isWaitingForParticles = false;
    }
}
