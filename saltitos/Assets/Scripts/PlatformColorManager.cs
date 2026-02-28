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
    public PlatformRandomColor[] platforms;
    public PlayerLives player1;
    public PlayerLives player2;

    [Header("Round Settings")]
    public float roundDuration = 10f;

    [Header("Pauses")]
    public float afterLifeLossPause = 1.0f;   // tiempo para ver explosión
    public float betweenRoundsPause = 0.3f;   // mini pausa entre rondas
    public float resultScreenSeconds = 2.5f;  // mostrar ganador antes de detener

    [Header("Fixed Color Palette (no similares)")]
    public Color[] palette;

    [Header("Color Names (mismo orden que palette)")]
    public string[] colorNames;

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

            // Evaluar y aplicar vidas (devuelve quién perdió)
            bool p1Lost, p2Lost;
            EvaluateAndApplyLives(out p1Lost, out p2Lost);

            bool someoneLostLife = p1Lost || p2Lost;

            // Pausa para ver explosión + respawn solo del que perdió
            if (someoneLostLife)
            {
                yield return new WaitForSeconds(afterLifeLossPause);

                if (p1Lost && player1.IsAlive()) player1.Respawn();
                if (p2Lost && player2.IsAlive()) player2.Respawn();
            }

            // Si alguien ya quedó en 0, termina
            if (!player1.IsAlive() || !player2.IsAlive())
                break;

            // mini pausa antes de la siguiente ronda
            if (betweenRoundsPause > 0f)
                yield return new WaitForSeconds(betweenRoundsPause);
        }

        // Ganador (solo cuando alguien llegó a 0)
        AnnounceWinner();

        // Espera para ver el resultado en pantalla
        yield return new WaitForSeconds(resultScreenSeconds);

        StopGame();
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

        // Asignar primeros N (N = # plataformas)
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

    // Evalúa la regla y aplica vidas. Devuelve quién perdió vida.
    void EvaluateAndApplyLives(out bool p1Lost, out bool p2Lost)
    {
        bool p1Correct = IsPlayerOnTarget(player1);
        bool p2Correct = IsPlayerOnTarget(player2);

        p1Lost = false;
        p2Lost = false;

        // Regla:
        // - Ambos correctos -> pierden ambos
        // - Ninguno correcto -> pierden ambos
        // - Solo uno correcto -> pierde el que NO está
        if (p1Correct && p2Correct)
        {
            player1.LoseLife(1); p1Lost = true;
            player2.LoseLife(1); p2Lost = true;
            Debug.Log("Ambos en la correcta: ambos pierden 1 vida.");
        }
        else if (!p1Correct && !p2Correct)
        {
            player1.LoseLife(1); p1Lost = true;
            player2.LoseLife(1); p2Lost = true;
            Debug.Log("Ninguno en la correcta: ambos pierden 1 vida.");
        }
        else
        {
            if (!p1Correct) { player1.LoseLife(1); p1Lost = true; Debug.Log("P1 falló: pierde 1 vida."); }
            if (!p2Correct) { player2.LoseLife(1); p2Lost = true; Debug.Log("P2 falló: pierde 1 vida."); }
        }

        Debug.Log($"Vidas -> P1: {player1.lives} | P2: {player2.lives}");
    }

    bool IsPlayerOnTarget(PlayerLives p)
    {
        if (p == null) return false;
        if (p.currentPlatform == null) return false;
        return p.currentPlatform.colorId == targetColorId;
    }

    void AnnounceWinner()
    {
        string result;

        if (player1.lives > player2.lives) result = "GANADOR: Player 1";
        else if (player2.lives > player1.lives) result = "GANADOR: Player 2";
        else result = "EMPATE";

        Debug.Log(result);

        if (hud != null)
            hud.ShowResult(result);
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