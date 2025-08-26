using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscenaRA : MonoBehaviour
{
    public void CargarRA()
    {
        // Debug.Log("📲 Cargando Escena de Realidad Aumentada...");
        SceneManager.LoadScene("PruebaRA");
    }
}