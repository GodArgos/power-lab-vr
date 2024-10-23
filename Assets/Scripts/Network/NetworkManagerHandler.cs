using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerHandler : MonoBehaviour
{
    private enum NetManagerType
    {
        LOCAL,
        STEAM
    }

    [SerializeField] private NetManagerType Type = NetManagerType.LOCAL;
    [SerializeField] private List<GameObject> m_managers;

    private void Awake()
    {
        if (Type == NetManagerType.LOCAL)
        {
            m_managers[0].SetActive(true);
            m_managers[1].SetActive(false);
        }
        else
        {
            m_managers[0].SetActive(false);
            m_managers[1].SetActive(true);
        }
    }
}
