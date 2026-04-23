using UnityEngine;
using UnityEngine.SceneManagement; 

public class CambiadorEscena : MonoBehaviour
{
    
    [Header("Configuración de Escena")]
    public string nombreEscena;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            
            SceneManager.LoadScene(nombreEscena);
        }
    }
}