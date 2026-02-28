using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformColorManager : MonoBehaviour
{
    // Para UI
    public float TimeLeft { get; private set; }
    public string TargetColorName { get; private set; } = "";
    public HUDController hud; // (opcional) arrastras el HUD aquí

    [Header("References")]
    public PlatformRandomColor[] platforms;   // tus 5 plataformas
    public PlayerLives player1;
    public PlayerLives player2;

    [Header("Round Settings")]
    public float roundDuration = 10f;

    [Header("Fixed Color Palette (no similares)")]
    public Color[] palette;

    [Header("Color Names (mismo orden que palette)")]
    public string[] colorNames;

    // Color objetivo actual (por ID)
    private int targetColorId = -1;

    void Start()
    {
        if (palette == null || palette.Length < platforms.Length)
        {
            Debug.LogError("La paleta debe tener AL MENOS la misma cantidad de colores que plataformas.");
            return;
        }

        if (colorNames == null || colorNames.Length != palette.Length)
        {
            Debug.LogError("colorNames debe existir y tener el MISMO tamaño que palette (mismo orden).");
            return;
        }

        StartCoroutine(RoundLoop());
    }

    IEnumerator RoundLoop()
    {
        while (player1.IsAlive() && player2.IsAlive())
        {
            AssignUniqueColorsToPlatforms();
            PickTargetColorFromPlatforms();

            Debug.Log($"COLOR OBJETIVO: {TargetColorName}");

            // contador para UI
            TimeLeft = roundDuration;
            while (TimeLeft > 0f)
            {
                TimeLeft -= Time.deltaTime;
                yield return null;
            }

            EvaluateAndApplyLives();
        }

        AnnounceWinnerAndStop();
    }

    void AssignUniqueColorsToPlatforms()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < palette.Length; i++) ids.Add(i);

        // Shuffle
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }

        for (int i = 0; i < platforms.Length; i++)
        {
            int id = ids[i];
            platforms[i].SetColor(palette[id], id);
        }
    }

    void PickTargetColorFromPlatforms()
    {
        int index = Random.Range(0, platforms.Length);
        targetColorId = platforms[index].colorId;

        TargetColorName = colorNames[targetColorId];
    }

    void EvaluateAndApplyLives()
    {
        bool p1Correct = IsPlayerOnTarget(player1);
        bool p2Correct = IsPlayerOnTarget(player2);

        if (p1Correct && p2Correct)
        {
            player1.LoseLife(1);
            player2.LoseLife(1);
            Debug.Log("Ambos en la correcta: ambos pierden 1 vida.");
        }
        else if (!p1Correct && !p2Correct)
        {
            player1.LoseLife(1);
            player2.LoseLife(1);
            Debug.Log("Ninguno en la correcta: ambos pierden 1 vida.");
        }
        else
        {
            if (!p1Correct) { player1.LoseLife(1); Debug.Log("P1 falló: pierde 1 vida."); }
            if (!p2Correct) { player2.LoseLife(1); Debug.Log("P2 falló: pierde 1 vida."); }
        }

        Debug.Log($"Vidas -> P1: {player1.lives} | P2: {player2.lives}");
    }

    bool IsPlayerOnTarget(PlayerLives p)
    {
        if (p == null) return false;
        if (p.currentPlatform == null) return false;
        return p.currentPlatform.colorId == targetColorId;
    }

    void AnnounceWinnerAndStop()
    {
        string result;

        if (player1.lives > player2.lives) result = "GANADOR: Player 1";
        else if (player2.lives > player1.lives) result = "GANADOR: Player 2";
        else result = "EMPATE";

        Debug.Log(result);

        if (hud != null)
            hud.ShowResult(result);

        StopGame();
    }

    void StopGame()
    {
        Time.timeScale = 0f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}