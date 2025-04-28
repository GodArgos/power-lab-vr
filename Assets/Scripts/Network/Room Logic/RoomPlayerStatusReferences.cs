using PowerLab;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PowerLab
{
    [Serializable]
    public class CharacterName
    {
        public TMP_Text baseName;
        public TMP_Text fullName;
    }
}

public class RoomPlayerStatusReferences : MonoBehaviour
{
    [Header("Dependencies")]
    public Image backgroundImage;
    public TMP_Text playerName;
    [SerializeField] private CharacterName[] characterNames = new CharacterName[2];
    [SerializeField] private TMP_Text characterNotSelected;
    [SerializeField] private GameObject characterNamesContainer;
    public Image[] statuses;
    [SerializeField] private Color unselectTextColor;
    [SerializeField] private Color selectTextColor;
    [SerializeField] private Color unselectedBackgroundColor;
    [SerializeField] private Color[] selectedBackgroundColor;

    private void Awake()
    {
        SetUnselected();
    }

    private void SetUnSelectedColor()
    {
        backgroundImage.color = unselectedBackgroundColor;
        playerName.color = unselectTextColor;
        
        foreach (var character in characterNames)
        {
            character.baseName.color = unselectTextColor;
            character.fullName.color = unselectedBackgroundColor;
        }

        for (int i = 0; i < statuses.Length; i++)
        {
            statuses[i].color = unselectTextColor;
        }
    }

    private void SetSelectedColor(int characterIndex)
    {
        backgroundImage.color = selectedBackgroundColor[characterIndex];
        playerName.color = selectTextColor;

        foreach (var character in characterNames)
        {
            character.baseName.color = selectTextColor;
            character.fullName.color = selectTextColor;
        }

        for (int i = 0; i < statuses.Length; i++)
        {
            statuses[i].color = selectTextColor;
        }
    }

    public void SetUnselected()
    {
        characterNotSelected.gameObject.SetActive(true);
        SetUnSelectedColor();
        characterNamesContainer.SetActive(false);
    }

    public void SetSelected(int characterSelected)
    {
        characterNotSelected.gameObject.SetActive(false);
        characterNamesContainer.SetActive(true);
        SetSelectedColor(characterSelected);

        for (int i = 0;i < characterNames.Length;i++)
        {
            if (i  == characterSelected)
            {
                characterNames[i].baseName.gameObject.SetActive(true);
            }
            else
            {
                characterNames[i].baseName.gameObject.SetActive(false);
            }
        }
    }

    public void SetReadyStatus(bool status)
    {
        if (status)
        {
            statuses[0].gameObject.SetActive(false);
            statuses[1].gameObject.SetActive(true);
        }
        else
        {
            statuses[0].gameObject.SetActive(true);
            statuses[1].gameObject.SetActive(false);
        }
    }
}
