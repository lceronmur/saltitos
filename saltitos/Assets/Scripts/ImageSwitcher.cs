using UnityEngine;

public class ImageSwitcher : MonoBehaviour
{
    public GameObject image1;
    public GameObject image2;

    public void ShowSecondImage()
    {
        image1.SetActive(false);
        image2.SetActive(true);
    }
}