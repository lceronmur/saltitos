using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    private PlatformRandomColor platform;

    void Awake()
    {
        platform = GetComponent<PlatformRandomColor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerLives pl = other.GetComponent<PlayerLives>();
        if (pl != null)
            pl.RegisterPlatformEnter(platform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerLives pl = other.GetComponent<PlayerLives>();
        if (pl != null)
            pl.RegisterPlatformExit(platform);
    }
}