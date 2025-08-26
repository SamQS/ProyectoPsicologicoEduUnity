using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class AvatarSpawner : MonoBehaviour
{
    public GameObject avatarPrefab;  // Asigna el avatar 3D desde el Inspector
    private ARRaycastManager arRaycastManager;
    private GameObject spawnedAvatar;

    void Start()
    {
        arRaycastManager = FindFirstObjectByType<ARRaycastManager>();

    }

    void Update()
    {
        if (spawnedAvatar == null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    spawnedAvatar = Instantiate(avatarPrefab, hitPose.position, hitPose.rotation);
                }
            }
        }
    }
}
