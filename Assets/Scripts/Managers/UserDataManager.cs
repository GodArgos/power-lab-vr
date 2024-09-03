using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    #region Singleton Config
    public static UserDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    #endregion

    [Header("Variables")]
    [Space(10)]
    [Tooltip("BORIS or MECHA, each will also have representation in code being " +
        "BORIS equal to '0' and MECHA equal to '1'.")]
    public Avatar avatar;
    public enum Avatar
    {
        BORIS,
        MECHA
    }
}
