using Mirror;
using System;
using TMPro;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    [SerializeField] private TMP_InputField sessionCodeInput;
    [SerializeField] private TMP_Text errorStatusText;

    [Header("PARAMETERS")]
    [SerializeField] private bool isTesting = false;
    [SerializeField] private string defaultIp = "127.0.0.1";

    private SessionCodeGenerator sessionCodeGenerator;

    private void Start()
    {
        sessionCodeGenerator = new SessionCodeGenerator(new LocalIPService(), new Base36Encoder());
    }

    public void HostGame()
    {
        string sessionCode = sessionCodeGenerator.GenerateSessionCode();

        //connectionStatusText.text = $"Cdigo: {sessionCode}";
        UserDataManager.Instance.sessionCode = sessionCode;
        Debug.Log(sessionCode);

        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.StartHost();
        }
    }

    public void JoinGame()
    {
        string hostIP = defaultIp;
        if (!isTesting)
        {
            string enteredCode = sessionCodeInput.text;
            string error = ErrorHandler(enteredCode);

            if (error != "")
            {
                errorStatusText.text = error;
                return;
            }

            hostIP = sessionCodeGenerator.DecodeSessionCode(enteredCode);

            if (hostIP == "invalid")
            {
                errorStatusText.text = ErrorHandler(hostIP);
                return;
            }
        }

        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.networkAddress = hostIP;
            NetworkManager.singleton.StartClient();
        }
    }

    private string ErrorHandler(string code)
    {
        if (code.Length <= 5)
        {
            return "Faltan mas caracteres";
        }
        else if (code == "invalid")
        {
            return "Codigo Invalido";
        }

        return "";
    }
}
