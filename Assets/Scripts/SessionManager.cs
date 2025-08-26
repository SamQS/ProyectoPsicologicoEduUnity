using UnityEngine;
using System;

public class SessionManager : MonoBehaviour
{
    public static string sessionId;

    void Start()
    {
        // Generar un ID único para la sesión del usuario
        sessionId = Guid.NewGuid().ToString();
        Debug.Log("ID de Sesión: " + sessionId);
    }
}
