using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAvatarSelector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject[] rigs;

    private void Awake()
    {
        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].SetActive(false);
        }
    }

    void Start()
    {
        int avatarSelected = (UserDataManager.Instance.avatar == UserDataManager.Avatar.BORIS) ? 0 : 1;
        
        for (int i = 0; i < rigs.Length; i++)
        {
            if (avatarSelected == i)
            {
                rigs[i].SetActive(true);
            }
        }
    }
}
