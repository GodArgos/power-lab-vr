using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RoomPlayerController : NetworkRoomPlayer
{
    [Header("DEPENDENCIES")]
    [SerializeField] private GameObject playerStatusPrefab;

    private GameObject playerStatus;

    [SyncVar(hook = nameof(SetSelectedAvatar))] public int characterAvatar = -1;
    private Button readyButton;
    private Button startGameButton;

    private XRSimpleInteractable[] interactables = new XRSimpleInteractable[2];

    private void OnEnable()
    {
        readyButton = RoomManagmentUIReferences.Instance.readyButton;
        readyButton.gameObject.SetActive(false);

        startGameButton = RoomManagmentUIReferences.Instance.startGameButton;

        readyButton.onClick.AddListener(UpdateReadyStatus);
        startGameButton.onClick.AddListener(StartGame);

        interactables[0] = RoomManagmentUIReferences.Instance.BorisInteractable;
        interactables[1] = RoomManagmentUIReferences.Instance.MechaInteractable;

        interactables[0].activated.AddListener(OnBorisActivated);
        interactables[1].activated.AddListener(OnMechaActivated);

        startGameButton.gameObject.SetActive(false);
    }

    public override void OnDisable()
    {
        RoomManagmentUIReferences.Instance.readyButton.onClick.RemoveListener(UpdateReadyStatus);
        RoomManagmentUIReferences.Instance.startGameButton.onClick.RemoveListener(StartGame);

        RoomManagmentUIReferences.Instance.BorisInteractable.activated.RemoveListener(OnBorisActivated);
        RoomManagmentUIReferences.Instance.MechaInteractable.activated.RemoveListener(OnMechaActivated);
    }

    #region Multiplayer CallBacks
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerStatus = Instantiate(playerStatusPrefab, RoomManagmentUIReferences.Instance.playerContainer);
        LocalStatusSetup();
        Debug.Log("JUGADOR LOCAL INSTANCEADO");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isLocalPlayer)
        {
            playerStatus = Instantiate(playerStatusPrefab, RoomManagmentUIReferences.Instance.playerContainer);
            RemoteStatusSetup(this.characterAvatar, this.readyToBegin);

            Debug.Log("ENTRO OTRO JUGADOR");
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!isLocalPlayer)
        {
            Destroy(playerStatus);
        }
    }

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        if (!isLocalPlayer)
        {
            Destroy(playerStatus);
        }
    }
    #endregion

    #region Local Status Setup
    private void LocalStatusSetup()
    {
        if (playerStatus == null) return;

        CmdSelectAvatar(-1);
        PlayerNumberSetup(this.index);
        ReadySetup(this.readyToBegin);
        ButtonSetup(this.readyToBegin);
        HandleFirstConnection();
    }
    #endregion

    #region Remote Status Setup
    private void RemoteStatusSetup(int avatar, bool status)
    {
        if (playerStatus == null) return;

        PlayerNumberSetup(this.index);
        AvatarSetup(avatar);
        ReadySetup(status);
    }
    #endregion

    #region Auxilary Methods
    private void PlayerNumberSetup(int playerIndex)
    {
        this.playerStatus.GetComponent<RoomPlayerStatusReferences>().playerName.text = $"JUGADOR {playerIndex + 1}";
    }

    private void AvatarSetup(int avatarIndex)
    {
        if (avatarIndex != -1)
        {
            playerStatus.GetComponent<RoomPlayerStatusReferences>().SetSelected(avatarIndex);
        }
        else
        {
            playerStatus.GetComponent<RoomPlayerStatusReferences>().SetUnselected();
        }
    }

    private void ReadySetup(bool status)
    {
        playerStatus.GetComponent<RoomPlayerStatusReferences>().SetReadyStatus(status);
    }

    private void ButtonSetup(bool status)
    {
        if (isServer)
        {
            RoomManagmentUIReferences.Instance.readyButton.transform.GetComponentInChildren<TMP_Text>().text = status ? "Cancelar" : "Listo?";
        }
        else
        {
            RoomManagmentUIReferences.Instance.readyButton.transform.GetComponentInChildren<TMP_Text>().text = status ? "Cancelar" : "Listo?";
        }
    }

    private void UpdateReadyStatus()
    {
        CmdChangeReadyState(!this.readyToBegin);
    }

    private void ForceReadyStatus(bool status)
    {
        CmdChangeReadyState(status);
    }

    private void StartGame()
    {
        if (CustomNetworkRoomManager.singleton != null)
        {
            CmdPrepareForGame();
            CustomNetworkRoomManager.singleton.CheckForReadiness();
        }
    }

    private void HandleFirstConnection()
    {
        foreach (var interactable in interactables)
        {
            if (interactable.GetComponent<ChangeInteractableOutline>().isSelected)
            {
                interactable.enabled = false;
            }
            else
            {
                interactable.enabled = true;
            }
        }
    }
    #endregion

    #region Hooks
    private void SetSelectedAvatar(int oldIndex, int newIndex)
    {
        if (isLocalPlayer)
        {
            AvatarSetup(newIndex);
        }
        else
        {
            RemoteStatusSetup(this.characterAvatar, this.readyToBegin);
        }
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        RemoteStatusSetup(this.characterAvatar, newReadyState);

        if (isLocalPlayer)
        {
            ButtonSetup(newReadyState);
        }
    }

    public override void IndexChanged(int oldIndex, int newIndex)
    {
        if (playerStatus == null) return;
        PlayerNumberSetup(newIndex);
    }
    #endregion

    #region Player Commands
    [Command(requiresAuthority = false)]
    private void CmdSelectAvatar(int index)
    {
        this.characterAvatar = index;
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateInteractableStatus(int index, bool status)
    {
        RpcUpdateInteractableStatus(index, status);
    }

    [Command(requiresAuthority = false)]
    private void CmdPrepareForGame()
    {
        RpcPrepareForGame();
    }
    #endregion

    #region Remote Procedure Calls
    [ClientRpc(includeOwner = false)]
    private void RpcUpdateInteractableStatus(int index, bool status)
    {
        for (int i = 0; i < interactables.Length; i++)
        {
            if (i  == index)
            {
                interactables[i].enabled = status;
            }
        }
    }

    [ClientRpc]
    private void RpcPrepareForGame()
    {
        if (characterAvatar != -1)
        {
            UserDataManager.Instance.avatar = characterAvatar == 0 ? UserDataManager.Avatar.BORIS : UserDataManager.Avatar.MECHA;
        }
    }
    #endregion

    #region Interactable Events
    private void OnBorisActivated(ActivateEventArgs args)
    {
        if (isLocalPlayer)
        {
            int value = characterAvatar == 0 ? -1 : 0;
            CmdSelectAvatar(value);
            CmdUpdateInteractableStatus(0, value == -1 ? true : false);

            interactables[1].enabled = value == -1 ? true : false;
            readyButton.gameObject.SetActive(value != -1 ? true : false);
            
            if (value == -1)
            {
                ForceReadyStatus(false);
            }
        }
    }

    private void OnMechaActivated(ActivateEventArgs args)
    {
        if (isLocalPlayer)
        {
            int value = characterAvatar == 1 ? -1 : 1;
            CmdSelectAvatar(value);
            CmdUpdateInteractableStatus(1, value == -1 ? true : false);

            interactables[0].enabled = value == -1 ? true : false;
            readyButton.gameObject.SetActive(value != -1 ? true : false);

            if (value == -1)
            {
                ForceReadyStatus(false);
            }
        }
    }
    #endregion
}
