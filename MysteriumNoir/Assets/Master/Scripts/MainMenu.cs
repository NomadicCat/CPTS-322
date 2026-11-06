using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string investigationScene;
    [SerializeField] private string interrogationScene;

    public void StartGame()
    {
        Debug.Log("Starting game...");
    }

    public void StartInvestigation()
    {
        SceneManager.LoadScene(investigationScene);
    }

    public void StartInterrogation()
    {
        SceneManager.LoadScene(interrogationScene);
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
