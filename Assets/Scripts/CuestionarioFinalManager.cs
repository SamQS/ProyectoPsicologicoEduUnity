using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class CuestionarioFinalManager : MonoBehaviour
{
    public APIConnector apiConnector;  // Conectar con la API
    public List<GameObject> preguntasFinales; // Lista de Preguntas (Cada pregunta tiene un ToggleGroup)

    public GameObject popUpFinalizado; // Referencia al pop-up

    private string apiRespuestasUrl = "https://proyectopsicologicoedu-production.up.railway.app/api/postrespuestas/";

    // Lista de escenas en orden
    private string[] escenasCuestionario = {
        "CuestionarioFinal",
        "CuestionarioFinal02",
        "CuestionarioFinal03",
        "CuestionarioFinal04",
        "CuestionarioFinal05",
        "CuestionarioFinal06",
        "CuestionarioFinal07",
        "CuestionarioFinal08",
        "CuestionarioFinal09",
        "CuestionarioFinal10",
        "CuestionarioFinal11",
        "CuestionarioFinal12",
        "CuestionarioFinal13",
        "CuestionarioFinal14",
        "CuestionarioFinal15",
        "CuestionarioFinal16",
        "CuestionarioFinal17",
        "CuestionarioFinal18",
        "CuestionarioFinal19",
        "CuestionarioFinal20",
        "CuestionarioFinal21",
        "CuestionarioFinal22",
        "CuestionarioFinal23",
        "CuestionarioFinal24",
        "CuestionarioFinal25",
        "CuestionarioFinal26",
        "CuestionarioFinal27",
        "CuestionarioFinal28",
        "CuestionarioFinal29",
        "CuestionarioFinal30",
    };

    void Start()
    {
        if (apiConnector == null)
        {
            apiConnector = Object.FindFirstObjectByType<APIConnector>();
            if (apiConnector == null)
            {
                Debug.LogError("❌ APIConnector no encontrado en la escena.");
            }
        }

        if (popUpFinalizado != null)
        {
            popUpFinalizado.SetActive(false);
        }
    }

    public void EnviarRespuestas()
    {
        Debug.Log("📩 Intentando enviar respuestas...");

        if (apiConnector == null)
        {
            Debug.LogError("❌ APIConnector es NULL.");
            return;
        }

        if (apiConnector.estudianteID == 0)
        {
            Debug.LogError("❌ No hay un estudiante registrado. Asegúrate de que se ha creado antes de responder.");
            return;
        }

        if (preguntasFinales == null || preguntasFinales.Count == 0)
        {
            Debug.LogError("❌ La lista de preguntas está vacía o no ha sido asignada.");
            return;
        }

        StartCoroutine(EnviarRespuestasCoroutine());
    }

    IEnumerator EnviarRespuestasCoroutine()
    {
        bool hayPreguntaSinResponder = false;

        foreach (GameObject preguntaObj in preguntasFinales)
        {
            if (preguntaObj == null)
            {
                Debug.LogError("❌ Pregunta nula en la lista.");
                continue;
            }

            ToggleGroup toggleGroup = preguntaObj.GetComponentInChildren<ToggleGroup>();
            if (toggleGroup == null)
            {
                Debug.LogError($"❌ No se encontró un ToggleGroup en la pregunta {preguntaObj.name}.");
                continue;
            }

            Toggle seleccionado = null;
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn)
                {
                    seleccionado = toggle;
                    break;
                }
            }

            if (seleccionado == null)
            {
                Debug.LogWarning($"⚠️ No se seleccionó ninguna opción para la pregunta: {preguntaObj.name}");
                hayPreguntaSinResponder = true;
                break; // Detener validación aquí
            }

            Transform textoTransform = preguntaObj.transform.Find("Texto");
            if (textoTransform == null)
            {
                Debug.LogError($"❌ No se encontró el objeto 'Texto' en la pregunta: {preguntaObj.name}");
                continue;
            }

            Text preguntaTextoComponent = textoTransform.GetComponent<Text>();
            if (preguntaTextoComponent == null)
            {
                Debug.LogError($"❌ No se encontró el componente Text en 'Texto' de la pregunta: {preguntaObj.name}");
                continue;
            }

            Text respuestaTextoComponent = seleccionado.GetComponentInChildren<Text>();
            if (respuestaTextoComponent == null)
            {
                Debug.LogError("❌ No se encontró el componente Text en la opción seleccionada.");
                continue;
            }

            string preguntaTexto = preguntaTextoComponent.text;
            string respuestaTexto = respuestaTextoComponent.text;

            Debug.Log($"📨 Enviando respuesta: {preguntaTexto} -> {respuestaTexto}");

            WWWForm form = new WWWForm();
            form.AddField("estudiante", apiConnector.estudianteID);
            form.AddField("pregunta", preguntaTexto);
            form.AddField("opcion_elegida", respuestaTexto);

            using (UnityWebRequest request = UnityWebRequest.Post(apiRespuestasUrl, form))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"✅ Respuesta guardada: {preguntaTexto} -> {respuestaTexto}");
                    Debug.Log("📨 Respuesta del servidor: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError($"❌ Error al enviar respuesta: {request.error}");
                }
            }
        }

        if (hayPreguntaSinResponder)
        {
            Debug.LogWarning("⛔ No todas las preguntas han sido respondidas. Por favor, selecciona una opción antes de continuar.");
            yield break;
        }

        Debug.Log("✅ Envío de respuestas completado.");

        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentIndex = System.Array.IndexOf(escenasCuestionario, currentSceneName);

        if (currentIndex == escenasCuestionario.Length - 1) // Última escena
        {
            Debug.Log("🏁 Cuestionario finalizado.");
            MostrarPopUpFinalizado();
        }
        else
        {
            string nextSceneName = escenasCuestionario[currentIndex + 1];
            Debug.Log($"➡️ Cargando la siguiente escena: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void MostrarPopUpFinalizado()
    {
        if (popUpFinalizado != null)
        {
            popUpFinalizado.SetActive(true);
        }
        else
        {
            Debug.LogError("❌ No se encontró el pop-up. ¿Está asignado en el Inspector?");
        }
    }
}
