using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class OnTriggerLoadSceneAdditive : NetworkBehaviour
{
    public int sceneFactor = 0;
    [SerializeField] private float neededPlayers = 2;
    [SyncVar] public bool active = true;

    [SyncVar(hook = nameof(OnNumberOfPlayersChanged))]
    public int numberOfPlayers = 0;

    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;

        NetworkIdentity networkIdentity = other.GetComponentInParent<NetworkIdentity>();

        if (networkIdentity != null && other.gameObject.CompareTag("PlayerNetwork") && !playersInTrigger.Contains(networkIdentity.gameObject))
        {
            playersInTrigger.Add(networkIdentity.gameObject);
            numberOfPlayers++;
        }
    }

    private void OnNumberOfPlayersChanged(int oldNumber, int newNumber)
    {
        if (newNumber == neededPlayers)
        {
            CmdChangeScene();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeScene()
    {
        Debug.Log(GetNextSceneName());
        CustomNetworkRoomManager.singleton.ServerChangeScene(GetNextSceneName());
        active = false;
    }

    public string GetNextSceneName()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string nextSceneName = SceneUtility.GetScenePathByBuildIndex(currentSceneIndex + sceneFactor);

        // Extract the scene name from the path
        int slashIndex = nextSceneName.LastIndexOf('/');
        int dotIndex = nextSceneName.LastIndexOf('.');
        return nextSceneName.Substring(slashIndex + 1, dotIndex - slashIndex - 1);
    }
}
