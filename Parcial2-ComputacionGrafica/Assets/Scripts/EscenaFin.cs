using UnityEngine;
using UnityEngine.SceneManagement;

public class EscenaFin : MonoBehaviour
{
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}