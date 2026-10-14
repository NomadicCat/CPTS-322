using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCharUI : MonoBehaviour
{
    [SerializeField] private CanvasManager canvasManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OpenCharConversation(int characterIndex)
    {
        switch(characterIndex)
        {
            case 0:
                Debug.Log("Open char " + characterIndex + " profile!");
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;
            case 1:
                Debug.Log("Open char " + characterIndex + " profile!");
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;
            case 2:
                Debug.Log("Open char " + characterIndex + " profile!");
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;
            case 3:
                Debug.Log("Open char " + characterIndex + " profile!");
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;
            case 4:
                Debug.Log("Open char " + characterIndex + " profile!");
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;
            case 5:
                Debug.Log("Open char " + characterIndex + " profile!");
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;  
            default:
                Debug.Log("Invalid character index: " + characterIndex);
                canvasManager.ToggleCharConversationUI(characterIndex);
                break;
        }

    }
}
