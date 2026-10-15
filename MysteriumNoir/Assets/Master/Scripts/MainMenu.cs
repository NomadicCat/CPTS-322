using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameScene;

    public void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene(gameScene);
    }

    public void GoToSettings()
    {
        Debug.Log("Opening settings...");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
