using UnityEngine;
using UnityEngine.SceneManagement;

public class BackRegistro : MonoBehaviour
{
    public void CambiarEscenaRegistro()
    {
        Debug.Log("¡Botón presionado! Intentando cambiar de escena...");
        SceneManager.LoadScene("RegistroEstudiantes");
    }
}
