using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager
{
    private int playersAlreadyGenerated = 0;

    public static new CustomNetworkRoomManager singleton => NetworkManager.singleton as CustomNetworkRoomManager;

    private bool showStartButton = false;

    public override void OnRoomServerPlayersReady()
    {
        if (RoomManagmentUIReferences.Instance != null)
        {
            showStartButton = true;
            RoomManagmentUIReferences.Instance.startGameButton.gameObject.SetActive(showStartButton);
        }
    }

    public override void OnRoomServerPlayersNotReady()
    {
        if (RoomManagmentUIReferences.Instance != null)
        {
            showStartButton = false;
            RoomManagmentUIReferences.Instance.startGameButton.gameObject.SetActive(showStartButton);
        }
    }

    public void CheckForReadiness()
    {
        if (allPlayersReady && showStartButton)
        {
            showStartButton = false;
            RoomManagmentUIReferences.Instance.startGameButton.gameObject.SetActive(showStartButton);

            ServerChangeScene(GameplayScene);
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // increment the index before adding the player, so first player starts at 1
        clientIndex++;

        if (Utils.IsSceneActive(RoomScene))
        {
            allPlayersReady = false;

            //Debug.Log("NetworkRoomManager.OnServerAddPlayer playerPrefab: {roomPlayerPrefab.name}");

            GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
            if (newRoomGameObject == null)
                newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

            if (playersAlreadyGenerated < 2)
            {
                NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
                playersAlreadyGenerated++;
            }
            else
            {
                NetworkServer.ReplacePlayerForConnection(conn, newRoomGameObject, true);
            }
        }
        else
        {
            GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            // Replace the player on the connection
            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, true);
        }
    }

}
