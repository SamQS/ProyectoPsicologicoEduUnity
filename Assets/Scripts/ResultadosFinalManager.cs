using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class ResultadosFinalManager : MonoBehaviour
{
    public GameObject respuestaPrefab;
    public Transform contentParent;
    public TextMeshProUGUI totalTexto;
    public TextMeshProUGUI nivelTexto;

    public string apiUrl = "https://proyectopsicologicoedu-production.up.railway.app/api/postrespuestas/"; // ← Aquí coloca tu IP o dominio
    private int estudianteID;

    private Dictionary<string, int> puntajes = new Dictionary<string, int>()
    {
        { "Nunca", 0 },
        { "Casi Nunca", 1 },
        { "A Veces", 2 },
        { "Casi Siempre", 3 },
        { "Siempre", 4 }
    };



    void Start()
    {
        estudianteID = PlayerPrefs.GetInt("EstudianteID", 0);
        if (estudianteID == 0)
        {
            Debug.LogError("No se encontró un estudiante activo.");
            return;
        }

        StartCoroutine(CargarResultados());
    }

    IEnumerator CargarResultados()
    {
        string url = apiUrl + "?estudiante_id=" + estudianteID;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error al cargar respuestas: " + www.error);
            yield break;
        }

        string json = "{\"items\":" + www.downloadHandler.text + "}"; // para usar JsonHelper
        RespuestaFinalArrayWrapper wrapper = JsonUtility.FromJson<RespuestaFinalArrayWrapper>(json);

        int totalPuntaje = 0;
        int numero = 1;

        foreach (Respuesta r in wrapper.items)
        {
            GameObject item = Instantiate(respuestaPrefab, contentParent);
            TextMeshProUGUI[] textos = item.GetComponentsInChildren<TextMeshProUGUI>();
            textos[0].text = r.pregunta;
            textos[1].text = "Respuesta: " + r.opcion_elegida;
            textos[2].text = "Puntaje: " + puntajes[r.opcion_elegida];

            totalPuntaje += puntajes[r.opcion_elegida];
            numero++;
        }

        totalTexto.text = "Puntaje total: " + totalPuntaje;
        nivelTexto.text = ObtenerNivel(totalPuntaje);
    }

    string ObtenerNivel(int puntaje)
    {
        if (puntaje <= 29) return "Progreso Nulo: No muestra avances en sus habilidades sociales";
        else if (puntaje <= 59) return "Progreso moderado. Aunque ya has mostrado algunas mejoras, es recomendable seguir trabajando en tus habilidades sociales, particularmente en la resolución de conflictos y la gestión de emociones.";
        else if (puntaje <= 89) return "Buen progreso. Has mostrado un desarrollo notable en varias áreas clave, pero aún hay algunas áreas en las que puedes seguir mejorando, como la iniciativa en situaciones sociales difíciles.";
        else return "Progreso excelente. Has mostrado una gran mejora en tu capacidad para comunicarte, manejar conflictos, y entender y expresar tus emociones de manera efectiva. Continúa trabajando en estas habilidades para seguir mejorando.";
    }
}
[System.Serializable]
public class RespuestaFinal
{
    public string pregunta;
    public string opcion_elegida;
}

[System.Serializable]
public class RespuestaFinalArrayWrapper
{
    public Respuesta[] items;
}