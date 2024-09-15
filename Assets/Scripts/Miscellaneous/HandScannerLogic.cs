using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScannerLogic : MonoBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [SerializeField] private GameObject[] fullCheckers = new GameObject[4];
    [SerializeField] private int checkerIndex = 4;
    [SerializeField] private float duration = 5f;
    [HideInInspector] public bool completed = false;

    private float checkerStep;
    private float timer;

    [Header("Test Variables")]
    [Space(10)]
    public bool tryActivate = false;

    // Start is called before the first frame update
    void Start()
    {
        checkerStep = duration / 4;
        timer = duration;
        foreach (var checker in fullCheckers)
        {
            checker.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tryActivate)
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
                timer = duration;
            }
        }
        else
        {
            foreach (var checker in fullCheckers)
            {
                checker.SetActive(false);
            }
            timer = duration;
            checkerIndex = fullCheckers.Length;
        }
    }
}
