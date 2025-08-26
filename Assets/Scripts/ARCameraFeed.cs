using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARCameraFeed : MonoBehaviour
{
    public ARCameraBackground arCameraBackground; // Se obtiene desde la AR Camera
    public RawImage rawImage; // Se asigna en el Inspector

    void Start()
    {
        if (arCameraBackground != null && rawImage != null)
        {
            rawImage.texture = arCameraBackground.material.mainTexture;
        }
    }
}
