using UnityEngine;
using System.Collections;

public class Cinematicas : MonoBehaviour
{
    public GameObject panelCinematica;
    public float duracionCinematica; // Duración de la cinemática en segundos

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartCinematica());
    
    }

    public IEnumerator StartCinematica()
    {
        // Mostrar el panel de la cinemática
        panelCinematica.SetActive(true);

        // Esperar la duración de la cinemática
        yield return new WaitForSeconds(duracionCinematica);

        // Ocultar el panel de la cinemática
        panelCinematica.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
