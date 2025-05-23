using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem.LowLevel;

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
    public bool m_state = true;

    private bool m_isWaitingForParticles = false; // Control para pausar el ciclo

    private void Start()
    {
        m_timer = m_playOnAwake ? m_cycleTime : 0;

        foreach (ParticleSystem ps in m_particleSystem)
        {
            ps.Stop();
            var main = ps.main;
            main.duration = m_durationTime;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //if (isServer)
        //    CmdUpdateSystemStatus(true);
        m_collider = GetComponent<BoxCollider>();

        ApplyState(m_state);
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
        CmdHandleSystem(newState);
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

        yield return new WaitUntil(() =>
        {
            foreach (ParticleSystem ps in m_particleSystem)
            {
                if (ps.IsAlive(true))
                    return false;
            }
            return true;
        });

        m_collider.enabled = false; // Desactiva el collider
        m_soundPlayer.CmdStopSoundForAll();

        // Reanuda el ciclo una vez que todo está detenido
        m_isWaitingForParticles = false;
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateSystemStatus(bool status)
    {
        this.m_state = status;
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleSystem(bool status)
    {
        RpcStartSystem(status);
    }

    [ClientRpc]
    private void RpcStartSystem(bool status)
    {
        ApplyState(status);
    }

    private void ApplyState(bool status)
    {
        if (status)
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
}
