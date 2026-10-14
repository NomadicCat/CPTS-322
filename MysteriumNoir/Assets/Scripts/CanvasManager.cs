using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject charSelectionUI;
    [SerializeField] private GameObject charConversationUI;

    // Start is called before the first frame update
    void Start()
    {
        charSelectionUI.SetActive(true);
        charConversationUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    public void ToggleCharConversationUI(int charNumber)
    {
        charSelectionUI.SetActive(false);
        charConversationUI.SetActive(true);
        Debug.Log("Character " + charNumber + " conversation UI opened.");
    }

    public void ToggleCharSelectionUI()
    {
        charSelectionUI.SetActive(true);
        charConversationUI.SetActive(false);
        Debug.Log("Character selection UI opened.");
    }
}
