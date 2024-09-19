using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    #region Singleton Config
    public static SpawnManager Instance { get; private set; }

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

    [Header("DEPENDENCIES")]
    [SerializeField] private List<GameObject> spawnPoints;
    private int currentSpawnPoint = 0;
    private GameObject localPlayer;
    
    // Unity Events
    [HideInInspector] public UnityEvent OnHitPlayer;

    private void Start()
    {
        OnHitPlayer.AddListener(PlayerHit);
        localPlayer = FindObjectOfType<XROrigin>().gameObject;

        foreach (var go in spawnPoints)
        {
            go.GetComponent<SpawnPoint>().spawnOrder = spawnPoints.IndexOf(go);
        }
    }

    private void PlayerHit()
    {
        Debug.Log("HITEADO");
        localPlayer.transform.position = spawnPoints[currentSpawnPoint].transform.position;
        localPlayer.transform.rotation = spawnPoints[currentSpawnPoint].transform.rotation;
    }

    public void UpdateCurrentSpawnPoint(int newValue)
    {
        currentSpawnPoint = newValue;
    }
}
