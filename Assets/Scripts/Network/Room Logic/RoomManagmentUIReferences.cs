using UnityEngine;
using UnityEngine.UI;

public class RoomManagmentUIReferences : MonoBehaviour
{
    public static RoomManagmentUIReferences Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform playerContainer;
    public Button readyButton;
    public Button startGameButton;
}
