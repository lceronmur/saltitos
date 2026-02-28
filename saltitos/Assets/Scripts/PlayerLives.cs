using UnityEngine;
using System.Collections.Generic;

public class PlayerLives : MonoBehaviour
{
    public int lives = 3;

    // Plataforma “actual” donde está parado (la que se evaluará)
    [HideInInspector] public PlatformRandomColor currentPlatform;

    // Conteo de overlaps por plataforma (para evitar falsos exits)
    private Dictionary<PlatformRandomColor, int> overlaps = new Dictionary<PlatformRandomColor, int>();

    public void RegisterPlatformEnter(PlatformRandomColor platform)
    {
        if (platform == null) return;

        if (!overlaps.ContainsKey(platform))
            overlaps[platform] = 0;

        overlaps[platform]++;

        // La última plataforma en la que entró (o la más probable) se vuelve la actual
        currentPlatform = platform;
    }

    public void RegisterPlatformExit(PlatformRandomColor platform)
    {
        if (platform == null) return;
        if (!overlaps.ContainsKey(platform)) return;

        overlaps[platform]--;
        if (overlaps[platform] <= 0)
        {
            overlaps.Remove(platform);

            // Si salió de la plataforma actual, elegir otra que todavía esté overlapeando
            if (currentPlatform == platform)
            {
                currentPlatform = null;
                foreach (var kv in overlaps)
                {
                    currentPlatform = kv.Key;
                    break;
                }
            }
        }
    }

    public void LoseLife(int amount = 1)
    {
        lives -= amount;
        if (lives < 0) lives = 0;
        Debug.Log($"{name} perdió vida. Vidas restantes: {lives}");
    }

    public bool IsAlive() => lives > 0;
}