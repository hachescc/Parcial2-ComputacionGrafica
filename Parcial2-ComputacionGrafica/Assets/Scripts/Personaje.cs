using UnityEngine;

public class Personaje : MonoBehaviour
{
    [Header("Datos del personaje")]
    public string nombre;
    public int fuerza;
    public int resistencia;

    [Header("Estado en combate")]
    public float saludActual;
    public bool estaVivo;

    void Awake()
    {
        saludActual = resistencia;
        estaVivo = true;
    }

    public void getDamage(float dmg)
    {
        saludActual -= dmg;
        if (saludActual <= 0)
        {
            saludActual = 0;
            estaVivo = false;
        }
    }

    public void curar(float cantidad)
    {
        saludActual += cantidad;
        if (saludActual > resistencia)
        {
            saludActual = resistencia;
        }
    }
}