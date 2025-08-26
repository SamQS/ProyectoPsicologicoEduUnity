using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class EstudianteResponse
{
    public int id;
}

public class APIConnector : MonoBehaviour
{
    private string apiUrl = "https://proyectopsicologicoedu-production.up.railway.app/api/estudiantes/";
    public int estudianteID = 0;

    private void Start()
    {
        // Cargar el estudianteID guardado previamente
        estudianteID = PlayerPrefs.GetInt("EstudianteID", 0);
        Debug.Log("EstudianteID cargado: " + estudianteID);
    }

    public IEnumerator PostEstudiante(string nombres, string apellidos, int edad, string grado, string salon)
    {
        WWWForm form = new WWWForm();
        form.AddField("nombres", nombres);
        form.AddField("apellidos", apellidos);
        form.AddField("edad", edad);
        form.AddField("grado", grado);
        form.AddField("salon", salon);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Registro exitoso: " + request.downloadHandler.text);

                string jsonResponse = request.downloadHandler.text;
                EstudianteResponse response = JsonUtility.FromJson<EstudianteResponse>(jsonResponse);

                if (response != null)
                {
                    estudianteID = response.id;
                    PlayerPrefs.SetInt("EstudianteID", estudianteID); // 🔹 Guardar ID en PlayerPrefs
                    PlayerPrefs.Save(); // 🔹 Asegurar que se guarde
                    Debug.Log("Estudiante registrado con ID: " + estudianteID);
                }
                else
                {
                    Debug.LogError("Error al procesar la respuesta JSON.");
                }
            }
            else
            {
                Debug.LogError("Error al enviar datos: " + request.error);
            }
        }
    }

    public void GetEstudiantes()
    {
        StartCoroutine(GetEstudiantesCoroutine());
    }

    private IEnumerator GetEstudiantesCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Datos recibidos: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al obtener datos: " + request.error);
            }
        }
    }
}
