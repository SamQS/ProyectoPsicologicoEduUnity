using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Android;

public class SpeechToTextManager : MonoBehaviour
{
    public GameObject botonTexto;
    public GameObject botonVoz;
    public GameObject botonGrabar;
    public AudioSource audioSource;

    private AudioClip clipGrabado;
    private bool grabando = false;

    private string rutaAudio;
    private string micName;

    void Start()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        else
        {
            Debug.Log("Permiso de micrófono ya concedido");
        }

        botonGrabar.SetActive(false);
        botonVoz.GetComponent<Button>().onClick.AddListener(PrepararGrabacion);
        botonGrabar.GetComponent<Button>().onClick.AddListener(GrabarOParar);

        // Selecciona el primer micrófono disponible
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0]; // Primer micrófono disponible
            Debug.Log("Micrófono encontrado: " + micName);
        }
        else
        {
            Debug.LogError("No se encontró un micrófono.");
        }
        Debug.Log("Micrófonos detectados:");
        foreach (var device in Microphone.devices)
        {
            Debug.Log(" - " + device);
        }

    }

    void PrepararGrabacion()
    {
        botonTexto.SetActive(false);
        botonVoz.SetActive(false);
        botonGrabar.SetActive(true);
    }

    void GrabarOParar()
    {
        if (!grabando)
        {
            // Comienza la grabación desde el micrófono
            clipGrabado = Microphone.Start(micName, false, 5, 44100); // 5 segundos de grabación
            grabando = true;
            Debug.Log("🎙 Grabando...");
        }
        else
        {
            Microphone.End(micName);
            grabando = false;
            Debug.Log("🛑 Fin de grabación");

            // Verifica si hay datos grabados
            if (clipGrabado != null && clipGrabado.samples > 0)
            {
                rutaAudio = Path.Combine(Application.persistentDataPath, "grabacion.wav");
                WavUtility.SaveWav(rutaAudio, clipGrabado); // Guardar como .wav

                StartCoroutine(EnviarAudioARuta(rutaAudio)); // Enviar el audio grabado a la API
            }
            else
            {
                Debug.LogError("No se ha grabado nada o el micrófono está vacío.");
            }
        }
    }

    IEnumerator EnviarAudioARuta(string ruta)
    {
        byte[] audioBytes = File.ReadAllBytes(ruta);
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioBytes, "grabacion.wav", "audio/wav");

        UnityWebRequest request = UnityWebRequest.Post("https://proyectopsicologicoedu-production.up.railway.app/api/voz/", form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            RespuestaAPI respuesta = JsonUtility.FromJson<RespuestaAPI>(json);
            StartCoroutine(ReproducirAudio(respuesta.audio_url));
        }
        else
        {
            Debug.LogError("Error al enviar audio: " + request.responseCode + " " + request.error);
            Debug.LogError("Respuesta completa: " + request.downloadHandler.text);
        }
    }

    IEnumerator ReproducirAudio(string url)
    {
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
    public class RespuestaAPI
    {
        public string texto;
        public string audio_url;
    }
}
