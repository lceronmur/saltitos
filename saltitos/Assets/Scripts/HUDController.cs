using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Refs")]
    public PlayerLives player1;
    public PlayerLives player2;
    public PlatformColorManager roundManager;

    [Header("Texts (TMP)")]
    public TMP_Text p1LivesText;
    public TMP_Text p2LivesText;
    public TMP_Text timerText;
    public TMP_Text instructionText;

    [Header("Winner UI")]
    public Image winnerImage;       // arrastra WinnerImage aquí
    public Sprite winnerP1Sprite;   // sprite gana P1
    public Sprite winnerP2Sprite;   // sprite gana P2
    public Sprite drawSprite;       // sprite empate

    void Start()
    {
        if (winnerImage != null)
        {
            winnerImage.preserveAspect = true;
            winnerImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Solo número (si ya tienes icono al lado)
        if (player1 != null && p1LivesText != null)
            p1LivesText.text = player1.lives.ToString();

        if (player2 != null && p2LivesText != null)
            p2LivesText.text = player2.lives.ToString();

        if (roundManager != null)
        {
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(roundManager.TimeLeft).ToString();

            if (instructionText != null)
                instructionText.text = "Ve a la plataforma: " + roundManager.TargetColorName;
        }
    }

    // Para que no tengas que cambiar tu PlatformColorManager
    // (que ya llama hud.ShowResult(result);)
public void ShowResult(string msg)
{
    Debug.Log("ShowResult recibido: " + msg);

    if (winnerImage == null)
    {
        Debug.LogError("HUDController: winnerImage NO está asignado en el Inspector.");
        return;
    }

    Sprite s = null;
    if (msg.Contains("Player1")) s = winnerP1Sprite;
    else if (msg.Contains("Player2")) s = winnerP2Sprite;
    else s = drawSprite;

    if (s == null)
    {
        Debug.LogError("HUDController: el sprite a mostrar es NULL. Revisa winnerP1Sprite / winnerP2Sprite / drawSprite en el Inspector.");
        return;
    }

    winnerImage.sprite = s;
    winnerImage.color = Color.white;          // asegura alpha
    winnerImage.preserveAspect = true;
    winnerImage.gameObject.SetActive(true);

    Debug.Log("WinnerImage ACTIVADO con sprite: " + s.name);
}
}