using UnityEngine;
using UnityEngine.SceneManagement;

public class CuestionarioInicial : MonoBehaviour
{
    public void OnClickContinuar()
    {
        SceneManager.LoadScene("CuestionarioInicial");
    }

    public void OnClickContinuarCF()
    {
        SceneManager.LoadScene("CuestionarioFinal");
    }
}