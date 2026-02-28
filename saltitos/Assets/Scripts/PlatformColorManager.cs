using UnityEngine;
using System.Collections.Generic;

public class PlatformColorManager : MonoBehaviour
{
    public PlatformRandomColor[] platforms; 
    public float changeEverySeconds = 10f;

    public Color[] palette = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    void Start()
    {
        ApplyUniqueColors();
        InvokeRepeating(nameof(ApplyUniqueColors), changeEverySeconds, changeEverySeconds);
    }

    void ApplyUniqueColors()
    {
        if (platforms == null || platforms.Length == 0) return;

        List<Color> colors = new List<Color>(palette);

        // Si faltan colores para la cantidad de plataformas, genera más
        while (colors.Count < platforms.Length)
            colors.Add(Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.7f, 1f));

        // Mezclar colores (shuffle)
        for (int i = colors.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (colors[i], colors[j]) = (colors[j], colors[i]);
        }

        // Asignar colores distintos a cada plataforma
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].SetColor(colors[i]);
        }
    }
}