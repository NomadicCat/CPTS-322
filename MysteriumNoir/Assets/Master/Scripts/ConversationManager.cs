using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject conversationMenu;

    [Header("Portraits")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite[] portraits;

    [Header("Conversation")]
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private TMP_InputField inputField;
    
    private int selectedCharacter;

    private void Start()
    {
        OpenCharacterSelectionMenu();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && conversationMenu.activeInHierarchy &&  !String.IsNullOrWhiteSpace(inputField.text))
        {
            AskQuestion();
        }
    }

    public void OpenCharacterSelectionMenu()
    {
        selectionMenu.SetActive(true);
        conversationMenu.SetActive(false);
        for(int i = scrollViewContent.childCount - 1; i >= 0; i--)
        {
            Destroy(scrollViewContent.GetChild(i));
        }
    }

    public void OpenCharacterConversationMenu(int selection)
    {
        selectionMenu.SetActive(false);
        conversationMenu.SetActive(true);

        selectedCharacter = selection;
        portraitImage.sprite = portraits[selectedCharacter];

        inputField.Select();
    }

    public void AskQuestion()
    {
        TextMeshProUGUI message = Instantiate(messagePrefab, scrollViewContent).GetComponent<TextMeshProUGUI>();
        message.text = "You: " + inputField.text;
        inputField.text = "";
        inputField.Select();
    }
}
