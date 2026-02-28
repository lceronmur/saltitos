using UnityEngine;
using System.Collections.Generic;

public class PlayerLives : MonoBehaviour
{
    public int lives = 3;

    [Header("Explosion Animation")]
    public string explodeTrigger = "Explode";

    [Header("Respawn")]
    public Transform respawnPoint;      // Empty donde reaparece (opcional)
    public GameObject visualRoot;       // Hijo Visual (Sprite/Animator)
    public float respawnInvulTime = 0f; // (opcional)

    // Plataforma “actual” donde está parado
    [HideInInspector] public PlatformRandomColor currentPlatform;

    // Conteo de overlaps por plataforma (anti falsos exits)
    private Dictionary<PlatformRandomColor, int> overlaps = new Dictionary<PlatformRandomColor, int>();

    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Animator (normalmente está en el hijo Visual)
        anim = GetComponentInChildren<Animator>();

        // VisualRoot: si no lo asignas, intenta encontrar "Visual"
        if (visualRoot == null)
        {
            Transform v = transform.Find("Visual");
            if (v != null) visualRoot = v.gameObject;
        }
    }

    public void RegisterPlatformEnter(PlatformRandomColor platform)
    {
        if (platform == null) return;

        if (!overlaps.ContainsKey(platform))
            overlaps[platform] = 0;

        overlaps[platform]++;
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

        // Asegura que el visual esté encendido para que se vea la explosión
        if (visualRoot != null && !visualRoot.activeSelf)
            visualRoot.SetActive(true);

        // Detiene al player para que la explosión no se vea rara
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        //  Disparar animación de explosión (UN SOLO animator)
        if (anim != null)
        {
            anim.ResetTrigger(explodeTrigger);
            anim.SetTrigger(explodeTrigger);
        }
        else
        {
            Debug.LogWarning($"{name}: No encontré Animator en hijos. No se puede reproducir Explode.");
        }

        Debug.Log($"{name} perdió vida. Vidas restantes: {lives}");
    }

    public bool IsAlive() => lives > 0;

    public void Respawn()
    {
        // Vuelve visible (si lo estabas ocultando)
        if (visualRoot != null)
            visualRoot.SetActive(true);

        // Reset físicas
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Mover a respawn si existe
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        // NO Rebind aquí: eso mata/borra animaciones y puede cortar Explode
        // Si necesitas volver a idle, lo haces en el Animator con Exit Time.
    }
}