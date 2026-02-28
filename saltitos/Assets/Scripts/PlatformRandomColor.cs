using UnityEngine;

public class PlatformRandomColor : MonoBehaviour
{
    public SpriteRenderer colorLayer;

    // ID del color actual de esta plataforma (0,1,2,...)
    public int colorId = -1;

    public void SetColor(Color c, int id)
    {
        colorId = id;
        if (colorLayer != null)
            colorLayer.color = c;
    }
}