using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeController : MonoBehaviour
{
    public void OnClickContinuar()
    {
        SceneManager.LoadScene("RegistroEstudiantes");
    }
}