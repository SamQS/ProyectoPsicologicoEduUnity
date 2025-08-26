using UnityEngine;

public class ForceCameraRendering : MonoBehaviour
{
    void Start()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("❌ No se encontró la Main Camera en la escena.");
            return;
        }

        mainCamera.enabled = true;
        Debug.Log("✅ Cámara activada correctamente.");
    }
}
