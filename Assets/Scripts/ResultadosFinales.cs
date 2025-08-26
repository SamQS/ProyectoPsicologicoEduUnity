using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ResumenCompletoManager : MonoBehaviour
{
    public TextMeshProUGUI totalInicialTexto;
    public TextMeshProUGUI nivelInicialTexto;
    public TextMeshProUGUI totalFinalTexto;
    public TextMeshProUGUI nivelFinalTexto;

    public string apiInicialUrl = "https://proyectopsicologicoedu-production.up.railway.app/api/respuestas/";
    public string apiFinalUrl = "https://proyectopsicologicoedu-production.up.railway.app/api/postrespuestas/";
    private int estudianteID;

    private readonly Dictionary<string, int> puntajes = new Dictionary<string, int>()
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

        StartCoroutine(CargarResumen());
    }

    IEnumerator CargarResumen()
    {
        // Cargar respuestas iniciales
        UnityWebRequest wwwInicial = UnityWebRequest.Get(apiInicialUrl + "?estudiante_id=" + estudianteID);
        yield return wwwInicial.SendWebRequest();

        if (wwwInicial.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error al cargar respuestas iniciales: " + wwwInicial.error);
        }
        else
        {
            string json = "{\"items\":" + wwwInicial.downloadHandler.text + "}";
            RespuestaArrayWrapper wrapper = JsonUtility.FromJson<RespuestaArrayWrapper>(json);

            int total = 0;
            foreach (Respuesta r in wrapper.items)
            {
                if (puntajes.ContainsKey(r.opcion_elegida))
                    total += puntajes[r.opcion_elegida];
            }

            totalInicialTexto.text = "Puntaje inicial: " + total;
            nivelInicialTexto.text = ObtenerNivelInicial(total);
        }

        // Cargar respuestas finales
        UnityWebRequest wwwFinal = UnityWebRequest.Get(apiFinalUrl + "?estudiante_id=" + estudianteID);
        yield return wwwFinal.SendWebRequest();

        if (wwwFinal.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error al cargar respuestas finales: " + wwwFinal.error);
        }
        else
        {
            string json = "{\"items\":" + wwwFinal.downloadHandler.text + "}";
            RespuestaFinalArrayWrapper wrapper = JsonUtility.FromJson<RespuestaFinalArrayWrapper>(json);

            int total = 0;
            foreach (Respuesta r in wrapper.items)
            {
                if (puntajes.ContainsKey(r.opcion_elegida))
                    total += puntajes[r.opcion_elegida];
            }

            totalFinalTexto.text = "Puntaje final: " + total;
            nivelFinalTexto.text = ObtenerNivelFinal(total);
        }
    }

    string ObtenerNivelInicial(int puntaje)
    {
        if (puntaje <= 40) return "Nivel Bajo: Dificultades importantes en habilidades sociales, necesita apoyo y orientación.";
        else if (puntaje <= 80) return "Nivel Medio-Bajo: Presenta algunas habilidades sociales, pero requiere mejorar en varias áreas.";
        else if (puntaje <= 110) return "Nivel Medio: Tiene habilidades sociales aceptables, aunque puede mejorar en ciertos aspectos.";
        else if (puntaje <= 140) return "Nivel Medio-Alto: Buen nivel de habilidades sociales, maneja bien la comunicación y las relaciones.";
        else return "Nivel Alto: Muestra habilidades sociales sólidas y efectivas en diversas situaciones.";
    }

    string ObtenerNivelFinal(int puntaje)
    {
        if (puntaje <= 29) return "Progreso Nulo: No muestra avances en sus habilidades sociales.";
        else if (puntaje <= 59) return "Progreso moderado: Has mostrado algunas mejoras, pero debes seguir trabajando en la resolución de conflictos y gestión emocional.";
        else if (puntaje <= 89) return "Buen progreso: Has mejorado en varias áreas, aunque puedes seguir reforzando la iniciativa social.";
        else return "Progreso excelente: Has mejorado en comunicación, manejo de conflictos y expresión emocional. ¡Sigue así!";
    }
}

[System.Serializable]
public class RespuestaFinales
{
    public string pregunta;
    public string opcion_elegida;
}

[System.Serializable]
public class RespuestaFinalesArrayWrapper
{
    public Respuesta[] items;
}

[System.Serializable]
public class RespuestaFinalFinalesArrayWrapper
{
    public Respuesta[] items;
}
