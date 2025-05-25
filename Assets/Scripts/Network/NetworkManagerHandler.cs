using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkManagerHandler : MonoBehaviour
{
    private enum NetManagerType
    {
        LOCAL,
        STEAM
    }

    [SerializeField] private NetManagerType Type = NetManagerType.LOCAL;
    [SerializeField] private List<GameObject> m_managers;
    [SerializeField] private Button hostButton;
    [SerializeField] private UnityEvent onLocalHostButtonClicked;
    [SerializeField] private UnityEvent onSteamHostButtonClicked;

    private void Awake()
    {
        if (Type == NetManagerType.LOCAL)
        {
            m_managers[0].SetActive(true);
            m_managers[1].SetActive(false);

            hostButton.onClick.AddListener(() => { onLocalHostButtonClicked?.Invoke(); });
        }
        else
        {
            m_managers[0].SetActive(false);
            m_managers[1].SetActive(true);
            hostButton.onClick.AddListener(() => { onSteamHostButtonClicked?.Invoke(); });
        }
    }
}
