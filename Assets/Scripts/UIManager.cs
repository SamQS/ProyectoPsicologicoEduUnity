using UnityEngine;
using UnityEngine.UI;
using TMPro; // si usas TextMeshPro
using System.Collections;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    public GameObject botonTexto;
    public GameObject botonVoz;
    public GameObject scrollOpciones;
    public GameObject botonOpcionPrefab;
    public Transform content;
    public AudioSource audioSource;

    private string[] opciones = {
        "Hola, ¿cómo estás?",
        "A veces me cuesta iniciar una conversación, pero quería hablar contigo.",
        "¿Qué fue lo mejor que te pasó esta semana?",
        "Me gusta escuchar música cuando estoy solo, ¿y tú?",
        "Hoy me sentí un poco nervioso en clase, ¿te ha pasado?",
        "Mi juego favorito es Minecraft, ¿tú juegas algo?",
        "¿Qué harías si ves a alguien solo en el recreo?",
        "Creo que no soy muy bueno haciendo amigos, pero lo intento.",
        "A veces me siento como si no encajara, ¿te ha pasado?",
        "Me da miedo hablar frente al grupo, ¿cómo lo haces tú?",
        "¿Qué haces cuando te sientes triste o frustrado?",
        "Yo también me siento mejor cuando alguien me escucha.",
        "¿Te gustaría que hablemos más seguido? Me caes bien.",
        "A mí me encanta dibujar personajes de anime, ¿te gusta el anime?",
        "No sé por qué, pero contigo me siento más tranquilo.",
        "¿Cómo te va en tus clases? Hay unas que me cuestan un montón.",
        "A veces solo necesito que alguien me diga que todo va a estar bien.",
        "¡Qué bueno que hablamos hoy! Me alegra conocerte.",
        "¿Qué te gustaría ser cuando termines la secundaria?",
        "Me gustaría tener más amigos como tú."
    };

    void Start()
    {
        scrollOpciones.SetActive(false);
        botonTexto.GetComponent<Button>().onClick.AddListener(MostrarOpciones);
    }

    void MostrarOpciones()
    {
        botonTexto.SetActive(false);
        botonVoz.SetActive(false);
        scrollOpciones.SetActive(true);

        foreach (string opcion in opciones)
        {
            GameObject nuevoBoton = Instantiate(botonOpcionPrefab, content);
            nuevoBoton.GetComponentInChildren<TMP_Text>().text = opcion;
            nuevoBoton.GetComponent<Button>().onClick.AddListener(() => EnviarMensaje(opcion));
        }
    }

    void EnviarMensaje(string mensaje)
    {
        StartCoroutine(EnviarAAPI(mensaje));
    }

    IEnumerator EnviarAAPI(string mensaje)
    {
        string url = "https://proyectopsicologicoedu-production.up.railway.app/api/chat/"; // Reemplaza con tu URL real

        string jsonBody = JsonUtility.ToJson(new MensajeAPI { mensaje = mensaje });

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonBody))
        {
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                RespuestaAPI respuesta = JsonUtility.FromJson<RespuestaAPI>(request.downloadHandler.text);

                if (!string.IsNullOrEmpty(respuesta.audio_url))
                {
                    StartCoroutine(ReproducirAudio(respuesta.audio_url));
                }
                else
                {
                    Debug.LogWarning("Audio vacío");
                }
            }
            else
            {
                Debug.LogError("Error en la API: " + request.error);
            }
        }
    }

    IEnumerator ReproducirAudio(string url)
    {
        Debug.Log("URL de audio: " + url);
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Error al reproducir audio: " + request.error);
            }
        }
    }

    [System.Serializable]
    public class MensajeAPI
    {
        public string mensaje;
    }

    [System.Serializable]
    public class RespuestaAPI
    {
        public string texto;
        public string audio_url;
    }
}
