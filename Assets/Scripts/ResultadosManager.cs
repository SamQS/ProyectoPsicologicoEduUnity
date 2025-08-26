using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class ResultadosManager : MonoBehaviour
{
    public GameObject respuestaPrefab;
    public Transform contentParent;
    public TextMeshProUGUI totalTexto;
    public TextMeshProUGUI nivelTexto;

    public string apiUrl = "https://proyectopsicologicoedu-production.up.railway.app/api/respuestas/";
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
        Debug.Log("Intentando conectarse a: " + url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error al cargar respuestas: " + www.error);
            Debug.Log("Resultado: " + www.result);
            Debug.Log("Error: " + www.error);
            Debug.Log("Response Code: " + www.responseCode);
            yield break;
        }

        string json = "{\"items\":" + www.downloadHandler.text + "}"; // para usar JsonHelper
        RespuestaArrayWrapper wrapper = JsonUtility.FromJson<RespuestaArrayWrapper>(json);

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
        if (puntaje <= 40) return "Nivel Bajo: Dificultades importantes en habilidades sociales, necesita apoyo y orientación.";
        else if (puntaje <= 80) return "Nivel Medio-Bajo: Presenta algunas habilidades sociales, pero requiere mejorar en varias áreas.";
        else if (puntaje <= 110) return "Nivel Medio: Tiene habilidades sociales aceptables, aunque puede mejorar en ciertos aspectos.";
        else if (puntaje <= 140) return "Nivel Medio-Alto: Buen nivel de habilidades sociales, maneja bien la comunicación y las relaciones.";
        else return "Nivel Alto: Muestra habilidades sociales sólidas y efectivas en diversas situaciones.";
    }
}
[System.Serializable]
public class Respuesta
{
    public string pregunta;
    public string opcion_elegida;
}

[System.Serializable]
public class RespuestaArrayWrapper
{
    public Respuesta[] items;
}