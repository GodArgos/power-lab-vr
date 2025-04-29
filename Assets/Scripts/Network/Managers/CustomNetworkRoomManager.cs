using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using System.Security.Principal;

public class CustomNetworkRoomManager : NetworkRoomManager
{
    private int roomPlayersAlreadyGenerated = 0;
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

            GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
            if (newRoomGameObject == null)
                newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

            if (roomPlayersAlreadyGenerated < 2)
            {
                NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
                roomPlayersAlreadyGenerated++;
            }
            else
            {
                NetworkServer.ReplacePlayerForConnection(conn, newRoomGameObject, true);
            }
        }
        else
        {
            if (playersAlreadyGenerated < 2)
            {
                GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

                // Replace the player on the connection
                NetworkServer.ReplacePlayerForConnection(conn, newPlayer, true);

                playersAlreadyGenerated++;
            }
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (newSceneName == RoomScene)
        {
            foreach (NetworkRoomPlayer roomPlayer in roomSlots)
            {
                if (roomPlayer == null)
                    continue;

                // find the game-player object for this connection, and destroy it
                NetworkIdentity identity = roomPlayer.GetComponent<NetworkIdentity>();

                if (NetworkServer.active)
                {
                    // re-add the room object
                    roomPlayer.GetComponent<NetworkRoomPlayer>().SetReadyToBegin(false);
                    NetworkServer.ReplacePlayerForConnection(identity.connectionToClient, roomPlayer.gameObject);
                }
            }

            allPlayersReady = false;
        }

        if (string.IsNullOrWhiteSpace(newSceneName))
        {
            Debug.LogError("ServerChangeScene empty scene name");
            return;
        }

        if (NetworkServer.isLoadingScene && newSceneName == networkSceneName)
        {
            Debug.LogError($"Scene change is already in progress for {newSceneName}");
            return;
        }

        // Throw error if called from client
        // Allow changing scene while stopping the server
        if (!NetworkServer.active && newSceneName != offlineScene)
        {
            Debug.LogError("ServerChangeScene can only be called on an active server.");
            return;
        }

        // Debug.Log($"ServerChangeScene {newSceneName}");
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = newSceneName;

        // Let server prepare for scene change
        OnServerChangeScene(newSceneName);

        // set server flag to stop processing messages while changing scenes
        // it will be re-enabled in FinishLoadScene.
        NetworkServer.isLoadingScene = true;

        SceneMessage sceneMessage;

        if (newSceneName != RoomScene && newSceneName != offlineScene && newSceneName != GameplayScene)
        {
            loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            sceneMessage = new SceneMessage{ sceneName = newSceneName, sceneOperation = SceneOperation.LoadAdditive };
        }
        else
        {
            loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
            sceneMessage = new SceneMessage { sceneName = newSceneName, sceneOperation = SceneOperation.Normal };

        }

        // Crear metodo que siempre unload escena anterior suponiendo que es diferente al menu o  room

        // ServerChangeScene can be called when stopping the server
        // when this happens the server is not active so does not need to tell clients about the change
        if (NetworkServer.active)
        {
            // notify all clients about the new scene
            NetworkServer.SendToAll(sceneMessage);
        }

        startPositionIndex = 0;
        startPositions.Clear();
    }
}
