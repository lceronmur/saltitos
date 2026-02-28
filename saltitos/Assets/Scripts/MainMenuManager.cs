using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;
    public GameObject creditsPanel;

    public GameObject image1;
    public GameObject image2;

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenInstructions()
    {
        instructionsPanel.SetActive(true);
        image1.SetActive(true);
        image2.SetActive(false);
    }

    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
    }

    public void ShowImage1()
    {
        image1.SetActive(true);
        image2.SetActive(false);
    }

    public void ShowImage2()
    {
        image1.SetActive(false);
        image2.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    // 🔥 AQUÍ VA EL BOTÓN EXIT (al final, pero dentro de la clase)
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}