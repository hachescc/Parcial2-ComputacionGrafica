using UnityEngine;
using System.Collections;

public class CinematicaMain : MonoBehaviour
{
    public Cinematicas cinematicas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinematicas.StartCoroutine(cinematicas.StartCinematica());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
