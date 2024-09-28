using Mirror;
using UnityEngine;

public class HandScannerLogic : NetworkBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [SerializeField] private GameObject[] fullCheckers = new GameObject[4];
    [SerializeField] private float duration = 5f;

    [Space(15)]

    [Header("Sync Variables")]
    [Space(10)]
    [SyncVar]
    public bool completed = false;
    [SyncVar]
    public bool tryActivate = false;

    [Space(15)]
    [Header("TEST")]
    [Space(10)]
    [SerializeField] private bool testVariable = false;

    private float checkerStep;
    private float timer;
    private int checkerIndex = 4;
    

    void Start()
    {
        checkerStep = duration / 4;
        RestartScanner();
    }

    private void Update()
    {
        if (tryActivate && !completed)
        {
            if (timer > 0)
            {
                if (timer <= checkerStep * (checkerIndex))
                {
                    fullCheckers[fullCheckers.Length - checkerIndex].SetActive(true);
                    checkerIndex--;
                }

                timer -= Time.deltaTime;
            }
            else
            {
                completed = true;
            }
        }
        else if (!completed)
        {
            RestartScanner();
        }

        if (testVariable && !tryActivate) 
        {
            SetTryActivate(true);
        }

    }

    public void RestartScanner()
    {
        foreach (var checker in fullCheckers)
        {
            checker.SetActive(false);
        }
        timer = duration;
        checkerIndex = fullCheckers.Length;
    }

    public void SetTryActivate(bool state)
    {
        tryActivate = state;
    }
}
