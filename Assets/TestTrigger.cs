using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTrigger : NetworkBehaviour
{
    public bool unloadLastScene = false;
    public int sceneFactor = 0;
    private bool test = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (test) return;
        
        if (unloadLastScene)
        {
            CmdUnload();
        }
        else
        {
            CmdChangeScene();
        }

        test = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeScene()
    {
        Debug.Log(GetNextSceneName());
        CustomNetworkRoomManager.singleton.ServerChangeScene(GetNextSceneName());
    }


    [Command(requiresAuthority = false)]
    public void CmdUnload()
    {
        Debug.Log(GetNextSceneName());
        RpcUnload(GetPreviousSceneName());
    }

    [ClientRpc]
    private void RpcUnload(string level)
    {
        SceneManager.UnloadSceneAsync(level);
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

    public string GetPreviousSceneName()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string nextSceneName = SceneUtility.GetScenePathByBuildIndex(currentSceneIndex + sceneFactor);

        // Extract the scene name from the path
        int slashIndex = nextSceneName.LastIndexOf('/');
        int dotIndex = nextSceneName.LastIndexOf('.');
        return nextSceneName.Substring(slashIndex + 1, dotIndex - slashIndex - 1);
    }
}
