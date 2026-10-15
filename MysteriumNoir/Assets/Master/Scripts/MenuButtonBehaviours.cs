using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonBehaviours : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("Start game!");
        SceneManager.LoadScene("character-ui");
    }

    public void GoToSettings()
    {
        Debug.Log("Go to settings!");
    }

    public void ExitGame()
    {
        Debug.Log("Exit game!");
    }
}
