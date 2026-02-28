using UnityEngine;

public class PlatformRandomColor : MonoBehaviour
{
   public SpriteRenderer colorLayer;

    public void SetColor(Color c)
    {
        if (colorLayer != null)
            colorLayer.color = c;
    }
}