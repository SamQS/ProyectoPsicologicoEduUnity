using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaceOnPlane : MonoBehaviour
{
    public GameObject objectToPlace; // Asigna tu modelo 3D en el Inspector
    private ARRaycastManager arRaycastManager;

    public Animator objectAnimator; 
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isPlaced = false;

    void Start()
    {
        arRaycastManager = Object.FindFirstObjectByType<ARRaycastManager>();

        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager no encontrado en la escena.");
            return;
        }

        if (objectToPlace == null)
        {
            Debug.LogError("El modelo 3D no está asignado en el Inspector.");
            return;
        }

        objectToPlace.SetActive(false); // Ocultar al inicio
        objectToPlace.transform.SetParent(null);
    }

    void Update()
    {
        if (isPlaced || arRaycastManager == null || objectToPlace == null)
            return;

        // Hacer Raycast hacia el centro de la pantalla para detectar el suelo
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Colocar el objeto en la posición detectada y activarlo
            objectToPlace.SetActive(true);
            objectToPlace.transform.position = hitPose.position;
            objectToPlace.transform.rotation = hitPose.rotation;

            // Ajustar altura si es necesario
            objectToPlace.transform.position += new Vector3(0, 0.05f, 0); // Elevarlo un poco si queda hundido

            objectToPlace.transform.Rotate(0, 180, 0);

            objectToPlace.transform.SetParent(null);

            if (objectAnimator != null)
            {
                objectAnimator.SetTrigger("StartAnimation"); // Cambia "StartAnimation" por el nombre del trigger en tu Animator
            }

            Debug.Log($"Modelo colocado automáticamente en: {objectToPlace.transform.position}");
            isPlaced = true;
        }
        else
        {
            Debug.Log("Aún no se ha detectado una superficie.");
        }
    }
}
