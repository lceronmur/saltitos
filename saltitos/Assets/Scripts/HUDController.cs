using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public PlayerLives player1;
    public PlayerLives player2;
    public PlatformColorManager roundManager;

    public TMP_Text p1LivesText;
    public TMP_Text p2LivesText;
    public TMP_Text timerText;
    public TMP_Text instructionText;
    public TMP_Text resultText;

    void Update()
    {
        if (player1 != null) p1LivesText.text = $"P1 Vidas: {player1.lives}";
        if (player2 != null) p2LivesText.text = $"P2 Vidas: {player2.lives}";

        if (roundManager != null)
        {
            timerText.text = $"Tiempo: {Mathf.CeilToInt(roundManager.TimeLeft)}";
            instructionText.text = $"VE A: {roundManager.TargetColorName}";
        }
    }

    public void ShowResult(string msg)
    {
        if (resultText != null)
        {
            resultText.text = msg;
            resultText.gameObject.SetActive(true);
        }
    }
}