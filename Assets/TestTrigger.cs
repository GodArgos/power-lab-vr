using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTrigger : NetworkBehaviour
{
    private bool test = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (test) return;
        
        CmdChangeScene();

        test = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeScene()
    {
        Debug.Log(GetNextSceneName());
        NetworkManager.singleton.ServerChangeScene(GetNextSceneName());
    }

    public string GetNextSceneName()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string nextSceneName = SceneUtility.GetScenePathByBuildIndex(currentSceneIndex + 1);

        // Extract the scene name from the path
        int slashIndex = nextSceneName.LastIndexOf('/');
        int dotIndex = nextSceneName.LastIndexOf('.');
        return nextSceneName.Substring(slashIndex + 1, dotIndex - slashIndex - 1);
    }
}
