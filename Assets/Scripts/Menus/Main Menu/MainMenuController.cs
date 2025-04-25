using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    
    public void OnPlayClicked()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
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
