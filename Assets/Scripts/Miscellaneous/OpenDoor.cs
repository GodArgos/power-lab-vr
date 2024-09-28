using Mirror;
using UnityEngine;

public class OpenDoor : NetworkBehaviour
{
    [Header("Dependencies")]
    [Space(10)]
    [SerializeField] private Transform door;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private float desiredDuration = 3f;

    [Space(15)]

    [Header("Door State")]
    [Space(10)]
    [SyncVar(hook = nameof(OnOpenDoorChanged))]
    public bool openDoor = false;

    [Header("TEST")]
    [Space(10)]
    [SerializeField] private bool testVariable = false;

    // Non-Serialized Variables
    private Vector3 startPosition;
    private float elapsedTime;
    private bool isMoving = false;

    private void Start()
    {
        startPosition = door.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            door.position = Vector3.Lerp(startPosition, targetPos, percentageComplete);

            if (percentageComplete >= 1f)
            {
                isMoving = false;
            }
        }

        if (testVariable && !openDoor)
        {
            TriggerOpenDoor();
        }
    }

    private void OnOpenDoorChanged(bool oldValue, bool newValue)
    {
        if (newValue && !isMoving)
        {
            isMoving = true;
            elapsedTime = 0f;
        }
    }

    public void TriggerOpenDoor()
    {
        if (!openDoor)
        {
            openDoor = true;
        }
    }
}
