using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class TiempoRAController : MonoBehaviour
{
    private int estudianteId;
    private int tiempoRaId;
    private string baseUrl = "https://proyectopsicologicoedu-production.up.railway.app/ra"; // Sin /api porque no lo usas

    void Start()
    {
        // Usa la clave correcta: "EstudianteID"
        estudianteId = PlayerPrefs.GetInt("EstudianteID", -1);

        if (estudianteId <= 0)
        {
            Debug.LogError("No se encontró un EstudianteID válido en PlayerPrefs.");
            return;
        }

        Debug.Log("Estudiante ID cargado: " + estudianteId);
        IniciarRA();
    }

    public void IniciarRA()
    {
        string horaInicio = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        StartCoroutine(EnviarInicioRA(estudianteId, horaInicio));
    }

    public void FinalizarRA()
    {
        string horaFin = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
        StartCoroutine(EnviarFinRA(tiempoRaId, horaFin));
    }

    IEnumerator EnviarInicioRA(int estudianteId, string horaInicio)
    {
        string url = $"{baseUrl}/inicio/";
        WWWForm form = new WWWForm();
        form.AddField("estudiante", estudianteId);
        form.AddField("hora_inicio", horaInicio);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Inicio RA registrado: " + request.downloadHandler.text);
            var response = JsonUtility.FromJson<RespuestaInicio>(request.downloadHandler.text);
            tiempoRaId = response.tiempo_ra_id;
        }
        else
        {
            Debug.LogError("Error al iniciar RA: " + request.error);
        }
    }

    IEnumerator EnviarFinRA(int tiempoRaId, string horaFin)
    {
        string url = $"{baseUrl}/fin/";
        WWWForm form = new WWWForm();
        form.AddField("tiempo_ra_id", tiempoRaId);
        form.AddField("hora_fin", horaFin);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Fin RA registrado: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al finalizar RA: " + request.error);
        }
    }

    [Serializable]
    public class RespuestaInicio
    {
        public string mensaje;
        public int tiempo_ra_id;
    }
}
