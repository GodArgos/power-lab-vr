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
            spawnPoints = new Dictionary<int, List<GameObject>>();
        }
    }
    #endregion

    [Header("DEPENDENCIES")]
    [SerializeField] private Dictionary<int, List<GameObject>> spawnPoints;
    [SerializeField] private List<BoxCollider> colliders; 
    public int currentLevel = 0;
    public int currentSpawnPoint = 0;
    private GameObject localPlayer;
    
    // Unity Events
    [HideInInspector] public UnityEvent OnHitPlayer;

    private void Start()
    {
        OnHitPlayer.AddListener(PlayerHit);
        localPlayer = FindObjectOfType<XROrigin>().gameObject;
    }

    public void AddSpawnPoint(int level, int order, GameObject spawn)
    {
        if (!spawnPoints.ContainsKey(level))
        {
            spawnPoints[level] = new List<GameObject>();
        }

        if (order >= spawnPoints[level].Count)
        {
            spawnPoints[level].Add(null); // Add a null value to fill the gap
            for (int i = spawnPoints[level].Count; i <= order; i++)
            {
                spawnPoints[level].Add(null);
            }
        }

        spawnPoints[level][order] = spawn;
    }

    private void PlayerHit()
    {
        localPlayer.transform.position = spawnPoints[currentLevel][currentSpawnPoint].transform.position;
        localPlayer.transform.rotation = spawnPoints[currentLevel][currentSpawnPoint].transform.rotation;
    }

    public void UpdateCurrentSpawnPoint(int newValue)
    {
        currentSpawnPoint = newValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            colliders[currentLevel].enabled = false;
            currentLevel++;
            currentSpawnPoint = 0;
        }
    }
}
