using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HostCodeDisplay : MonoBehaviour
{
    private void Start()
    {
        if (UserDataManager.Instance.sessionCode != string.Empty)
        {
            GetComponent<TMP_Text>().text = UserDataManager.Instance.sessionCode;
        }
    }
}
