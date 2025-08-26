using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarResultados : MonoBehaviour
{
    public void CargarResultadosInicial()
    {
        // Debug.Log("📲 Cargando Escena de Realidad Aumentada...");
        SceneManager.LoadScene("ResultadosCuestionarioInicial");
    }

    public void CargarResultadosFinal()
    {
        // Debug.Log("📲 Cargando Escena de Realidad Aumentada...");
        SceneManager.LoadScene("ResultadosCuestionarioFinal");
    }

     public void CargarResFinales()
    {
        // Debug.Log("📲 Cargando Escena de Realidad Aumentada...");
        SceneManager.LoadScene("ResultadosFinales");
    }
}