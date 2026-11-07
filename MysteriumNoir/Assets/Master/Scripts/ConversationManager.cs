using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    // VARIABLES =====================================================================
    #region Variables
    [Header("Menu Objects")]
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject conversationMenu;

    [Header("Portraits")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite[] portraits;

    [Header("Conversation")]
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private TMP_InputField inputField;

    [Header("Story")]
    [SerializeField] private Story story;
    
    private bool isAwaitingResponse;
    private int selectedCharacter;
    #endregion

    // START AND UPDATE =====================================================================
    #region Start & Update
    private void Start()
    {
        if (story == null)
        {
            story = FindObjectOfType<Story>();
        }
        if (story == null)
        {
            Debug.LogWarning("ConversationManager: no Story found in scene, creating one.");
            story = new GameObject("Story").AddComponent<Story>();
        }
        OpenCharacterSelectionMenu();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && 
            conversationMenu.activeInHierarchy &&  
            !String.IsNullOrWhiteSpace(inputField.text))
        {
            AskQuestion();
        }
    }
    #endregion

    // MENU VISIBILITY =====================================================================
    #region Menu Visibility
    public void OpenCharacterSelectionMenu()
    {
        selectionMenu.SetActive(true);
        conversationMenu.SetActive(false);
        for(int i = scrollViewContent.childCount - 1; i >= 0; i--)
        {
            Destroy(scrollViewContent.GetChild(i).gameObject);
        }
        UpdateScrollView();
    }

    public void OpenCharacterConversationMenu(int selection)
    {
        selectionMenu.SetActive(false);
        conversationMenu.SetActive(true);

        selectedCharacter = selection;
        portraitImage.sprite = portraits[selectedCharacter];

        inputField.ActivateInputField();
        inputField.Select();
    }
    #endregion

    // QUESTION AND RESPONSE ============================================================
    #region Question and Response
    public void AskQuestion()
    {
        if(!isAwaitingResponse)
        {
            string question = inputField.text;
            TextMeshProUGUI message = Instantiate(messagePrefab, scrollViewContent).GetComponent<TextMeshProUGUI>();
            message.text = "You: " + question; 
            UpdateScrollView();
            GetResponse(question);           
        }
        inputField.text = "";

        inputField.ActivateInputField();
        inputField.Select();        
    }

    private async void GetResponse(string input)
    {
        isAwaitingResponse = true;

        Character character = null;
        if (story != null && selectedCharacter >= 0 && selectedCharacter < story.characters.Count)
        {
            character = story.characters[selectedCharacter];
        }

        bool hasCharacter = character != null;
        string speaker = hasCharacter && !string.IsNullOrEmpty(character.characterName)
            ? character.characterName
            : "ChatGPT";

        TextMeshProUGUI message = Instantiate(messagePrefab, scrollViewContent).GetComponent<TextMeshProUGUI>();
        message.text = speaker + " is thinking...";
        message.fontStyle = FontStyles.Italic;
        UpdateScrollView();
        
        string response = hasCharacter
            ? await character.AnswerDetective(input)
            : await AIManager.Instance.GenerateResponse(input);

        message.text = speaker + ": " + response;
        message.fontStyle = FontStyles.Normal;
        UpdateScrollView();

        isAwaitingResponse = false;
    }

    private void UpdateScrollView()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }
    #endregion
}
