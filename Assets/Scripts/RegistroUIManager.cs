using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class RegistroUIManager : MonoBehaviour
{
    public TMP_InputField inputNombre;
    public TMP_InputField inputApellido;
    public TMP_InputField inputEdad;
    public TMP_InputField inputGrado;
    public TMP_InputField inputSalon;

    public APIConnector apiConnector; 

    public string siguienteEscena;  

    public void RegistrarEstudiante()
    {
        string nombres = inputNombre.text;
        string apellidos = inputApellido.text;
        int edad;
        if (!int.TryParse(inputEdad.text, out edad))
        {
            Debug.LogError("Edad no es un número válido.");
            return;
        }
        string gradoText = inputGrado.text;
        int grado;

        if (!int.TryParse(gradoText, out grado))
        {
            Debug.LogError("El grado no es un número válido.");
            return;
        }

        // Verificar que el número esté en el rango de 3 a 5
        if (grado < 3 || grado > 5)
        {
            Debug.LogError("El grado debe estar entre 3 y 5.");
            return;
        }

        string salon = inputSalon.text;

        // Llama a la función PostEstudiante con una corrutina para esperar la respuesta
        StartCoroutine(RegistrarYContinuar(nombres, apellidos, edad, gradoText, salon));
    }

    private IEnumerator RegistrarYContinuar(string nombres, string apellidos, int edad, string grado, string salon)
    {
        // Llamada a la API y esperar su finalización
        yield return StartCoroutine(apiConnector.PostEstudiante(nombres, apellidos, edad, grado, salon));

        // Cambiar de escena después del registro
        SceneManager.LoadScene(siguienteEscena);
    }
}
