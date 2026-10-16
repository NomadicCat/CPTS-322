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
    
    private bool isAwaitingResponse;
    private int selectedCharacter;
    #endregion

    // START AND UPDATE =====================================================================
    #region Start & Update
    private void Start()
    {
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
            TextMeshProUGUI message = Instantiate(messagePrefab, scrollViewContent).GetComponent<TextMeshProUGUI>();
            message.text = "You: " + inputField.text; 
            UpdateScrollView();
            GetResponse(message.text);           
        }
        inputField.text = "";

        inputField.ActivateInputField();
        inputField.Select();        
    }

    private async void GetResponse(string input)
    {
        isAwaitingResponse = true;

        TextMeshProUGUI message = Instantiate(messagePrefab, scrollViewContent).GetComponent<TextMeshProUGUI>();
        message.text = "ChatGPT is thinking...";
        message.fontStyle = FontStyles.Italic;
        UpdateScrollView();
        
        string response = await AIManager.Instance.GenerateResponse(input);

        message.text = "ChatGPT: " + response;
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
