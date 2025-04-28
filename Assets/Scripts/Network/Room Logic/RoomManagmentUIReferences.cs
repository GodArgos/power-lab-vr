using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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
    public XRSimpleInteractable BorisInteractable;
    public XRSimpleInteractable MechaInteractable;
}
