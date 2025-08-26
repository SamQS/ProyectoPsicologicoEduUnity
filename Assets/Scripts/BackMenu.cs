using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackMenu : MonoBehaviour
{
    private TiempoRAController tiempoRAController;

    void Start()
    {
        // Usamos el método recomendado por Unity para encontrar el script
        tiempoRAController = Object.FindFirstObjectByType<TiempoRAController>();
    }

    public void OnClickContinuar()
    {
        SceneManager.LoadScene("WelcomeScene");
    }
    public void BackInicio()
    {
        SceneManager.LoadScene("WelcomeScene");
    }

    public void BackOpciones()
    {
        if (tiempoRAController != null)
        {
            tiempoRAController.FinalizarRA();
        }

        StartCoroutine(VolverMenuOpcionesConDelay());
    }

    IEnumerator VolverMenuOpcionesConDelay()
    {
        yield return new WaitForSeconds(1f); // Ajusta si necesitas más tiempo para enviar el POST
        SceneManager.LoadScene("MenuOpciones");
    }
}
