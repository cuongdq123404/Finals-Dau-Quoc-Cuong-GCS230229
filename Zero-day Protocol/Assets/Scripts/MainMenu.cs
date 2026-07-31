using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // 1. Call this function from your "Play" Button OnClick event
    public void PlayGame()
    {
        Debug.Log("Loading Player Base / Shop...");
        SceneManager.LoadScene(2); // Loads your map scene by its name
    }

    // 2. Call this function from your "Tutorial" Button OnClick event
    public void PlayTutorial()
    {
        Debug.Log("Loading Tutorial Stage...");
        SceneManager.LoadScene(1); // Loads index 1 (Tutorial)
    }

    public void QuitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}