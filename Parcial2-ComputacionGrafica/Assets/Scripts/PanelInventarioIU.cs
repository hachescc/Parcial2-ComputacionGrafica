using UnityEngine;
using UnityEngine.UI;

public class PanelInventarioIU : MonoBehaviour
{
    [Header("Panel principal")]
    public GameObject panelInventario;

    [Header("Mensaje inventario vacio")]
    public Text textoVacio;

    [Header("Slot - Pocion")]
    public Image imagenPocion;
    public Text cantidadPocion;

    [Header("Slot - Cofre de tesoro")]
    public Image imagenCofre;
    public Text cantidadCofre;

    [Header("Dinero")]
    public Text textoDinero;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AbrirCerrarPanel();
        }
    }

    void AbrirCerrarPanel()
    {
        if (panelInventario == null) return;

        bool estabaAbierto = panelInventario.activeSelf;
        panelInventario.SetActive(!estabaAbierto);

        if (!estabaAbierto)
        {
            ActualizarPanel();
        }
    }

    public void ActualizarPanel()
    {
        if (GameManager.Instance == null) return;
        Inventario inv = GameManager.Instance.inventario;
        if (inv == null) return;


        bool inventarioVacio = inv.cantidades.Count == 0 && inv.dinero == 0;

  
        if (textoVacio != null)
            textoVacio.gameObject.SetActive(inventarioVacio);


        if (inventarioVacio)
        {
            if (imagenPocion != null) imagenPocion.gameObject.SetActive(false);
            if (cantidadPocion != null) cantidadPocion.gameObject.SetActive(false);
            if (imagenCofre != null) imagenCofre.gameObject.SetActive(false);
            if (cantidadCofre != null) cantidadCofre.gameObject.SetActive(false);
            if (textoDinero != null) textoDinero.gameObject.SetActive(false);
            return;
        }


        int cantPocion = inv.ObtenerCantidad("pocion");
        bool tienePocion = cantPocion > 0;
        if (imagenPocion != null) imagenPocion.gameObject.SetActive(tienePocion);
        if (cantidadPocion != null)
        {
            cantidadPocion.gameObject.SetActive(tienePocion);
            cantidadPocion.text = "x" + cantPocion;
        }

   
        int cantCofre = inv.ObtenerCantidad("cofre tesoro");
        bool tieneCofre = cantCofre > 0;
        if (imagenCofre != null) imagenCofre.gameObject.SetActive(tieneCofre);
        if (cantidadCofre != null)
        {
            cantidadCofre.gameObject.SetActive(tieneCofre);
            cantidadCofre.text = "x" + cantCofre;
        }

      
        bool tieneDinero = inv.dinero > 0;
        if (textoDinero != null)
        {
            textoDinero.gameObject.SetActive(tieneDinero);
            textoDinero.text = "Dinero: " + inv.dinero + " monedas";
        }
    }
}
