using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class DuracionRAViewer : MonoBehaviour
{
    public TextMeshProUGUI duracionTexto; // Arrástralo desde el Canvas
    private int estudianteID;

    private string duracionUrl = "https://proyectopsicologicoedu-production.up.railway.app/ra/duracion/"; // Ajusta si tu endpoint es distinto

    void Start()
    {
        estudianteID = PlayerPrefs.GetInt("EstudianteID", 0);

        if (estudianteID == 0)
        {
            Debug.LogError("EstudianteID no encontrado en PlayerPrefs.");
            return;
        }

        StartCoroutine(CargarDuracion());
    }

    IEnumerator CargarDuracion()
    {
        string urlConId = duracionUrl + "?estudiante_id=" + estudianteID;
        UnityWebRequest request = UnityWebRequest.Get(urlConId);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener duración: " + request.error);
            yield break;
        }

        // Asume que la API responde con: { "duracion": "00:03:12" } o similar
        DuracionResponse data = JsonUtility.FromJson<DuracionResponse>(request.downloadHandler.text);
        duracionTexto.text = "Tiempo de uso RA: " + data.duracion;
    }

    [System.Serializable]
    public class DuracionResponse
    {
        public string duracion;
    }
}
