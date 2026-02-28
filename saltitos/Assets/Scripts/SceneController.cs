using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("Maria Paula");
    }

    public void GoToInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}