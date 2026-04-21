using UnityEngine;
using System.Collections.Generic;

public class Inventario : MonoBehaviour
{
    [Header("Inventario")]
    public List<string> objetos = new List<string>();
    public Dictionary<string, int> cantidades = new Dictionary<string, int>();
    public int dinero;

    public void AgregarObjeto(string nombre)
    {
        objetos.Add(nombre);

        if (cantidades.ContainsKey(nombre))
        {
            cantidades[nombre]++;
        }
        else
        {
            cantidades.Add(nombre, 1);
        }

        Debug.Log("Objeto agregado: " + nombre + " | Total: " + cantidades[nombre]);
    }

    public void AgregarDinero(int cantidad)
    {
        dinero += cantidad;
        Debug.Log("Dinero agregado: " + cantidad + " | Total: " + dinero);
    }

    public bool TieneObjeto(string nombre)
    {
        return cantidades.ContainsKey(nombre);
    }

    public int ObtenerCantidad(string nombre)
    {
        if (cantidades.ContainsKey(nombre))
        {
            return cantidades[nombre];
        }
        return 0;
    }

    public string MostrarInventario()
    {
        string resultado = "Dinero: " + dinero + "\n";
        foreach (string clave in cantidades.Keys)
        {
            resultado += clave + ": " + cantidades[clave] + "\n";
        }
        return resultado;
    }
}