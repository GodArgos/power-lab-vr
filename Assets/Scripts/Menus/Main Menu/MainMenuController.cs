using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject connectionMenu;
    [SerializeField] private GameObject joinMenu;
    
    public void OnPlayClicked()
    {
        if (connectionMenu != null)
        {
            connectionMenu.SetActive(true);
        }
    }

    public void OnJoinClicked()
    {
        if (joinMenu != null)
        {
            connectionMenu.SetActive(false);
            joinMenu.SetActive(true);
        }
    }

    public void OnExitClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
