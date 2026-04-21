using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;

    [Header("Configuracion")]
    public float velocidadSeguimiento = 5f;
    public Vector3 offset = new Vector3(0, 2, -10);

    private Vector3 velocidadActual;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionObjetivo = objetivo.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, posicionObjetivo, ref velocidadActual, 1f / velocidadSeguimiento);
    }
}