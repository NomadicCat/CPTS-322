using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBehaviour : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = "Please type your question then hit enter.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDialogueText(string newText)
    {
        dialogueText.text = "Player: " + newText;
        inputField.text = "";
    }

    
}
