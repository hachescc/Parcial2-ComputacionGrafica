using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Barras de salud heroes")]
    public Slider barraHeroe1;
    public Slider barraHeroe2;
    public Slider barraHeroe3;
    public Slider barraHeroe4;

    [Header("Textos de salud")]
    public Text textoSaludHeroe1;
    public Text textoSaludHeroe2;
    public Text textoSaludHeroe3;
    public Text textoSaludHeroe4;

    [Header("Turno actual")]
    public Text textoTurnoActual;

    [Header("Notificacion de accion")]
    public Text textoNotificacion;
    public GameObject panelNotificacion;

    [Header("Inventario")]
    public Text textoInventario;
    public GameObject panelInventario;

    [Header("Sistema de combate")]
    public SistemaCombate sistemaCombate;

    private float timerNotificacion;
    private float duracionNotificacion = 2f;

    void Start()
    {
        if (panelNotificacion != null)
        {
            panelNotificacion.SetActive(false);
        }

        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
        }

        InicializarBarras();
    }

    void Update()
    {
        ActualizarBarras();
        ActualizarTurno();
        ActualizarNotificacion();

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventario();
        }
    }

    void InicializarBarras()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < GameManager.Instance.heroes.Length; i++)
        {
            Personaje heroe = GameManager.Instance.heroes[i];
            if (heroe == null) continue;

            switch (i)
            {
                case 0:
                    if (barraHeroe1 != null) barraHeroe1.maxValue = heroe.resistencia;
                    break;
                case 1:
                    if (barraHeroe2 != null) barraHeroe2.maxValue = heroe.resistencia;
                    break;
                case 2:
                    if (barraHeroe3 != null) barraHeroe3.maxValue = heroe.resistencia;
                    break;
                case 3:
                    if (barraHeroe4 != null) barraHeroe4.maxValue = heroe.resistencia;
                    break;
            }
        }
    }

    void ActualizarBarras()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < GameManager.Instance.heroes.Length; i++)
        {
            Personaje heroe = GameManager.Instance.heroes[i];
            if (heroe == null) continue;

            switch (i)
            {
                case 0:
                    if (barraHeroe1 != null) barraHeroe1.value = heroe.saludActual;
                    if (textoSaludHeroe1 != null) textoSaludHeroe1.text = heroe.saludActual + "/" + heroe.resistencia;
                    break;
                case 1:
                    if (barraHeroe2 != null) barraHeroe2.value = heroe.saludActual;
                    if (textoSaludHeroe2 != null) textoSaludHeroe2.text = heroe.saludActual + "/" + heroe.resistencia;
                    break;
                case 2:
                    if (barraHeroe3 != null) barraHeroe3.value = heroe.saludActual;
                    if (textoSaludHeroe3 != null) textoSaludHeroe3.text = heroe.saludActual + "/" + heroe.resistencia;
                    break;
                case 3:
                    if (barraHeroe4 != null) barraHeroe4.value = heroe.saludActual;
                    if (textoSaludHeroe4 != null) textoSaludHeroe4.text = heroe.saludActual + "/" + heroe.resistencia;
                    break;
            }
        }
    }

    void ActualizarTurno()
    {
        if (sistemaCombate == null) return;
        if (textoTurnoActual == null) return;
        textoTurnoActual.text = "Turno: " + sistemaCombate.nombreTurnoActual;
    }

    public void MostrarNotificacion(string mensaje)
    {
        if (textoNotificacion == null) return;
        textoNotificacion.text = mensaje;
        panelNotificacion.SetActive(true);
        timerNotificacion = duracionNotificacion;
    }

    void ActualizarNotificacion()
    {
        if (!panelNotificacion.activeSelf) return;

        timerNotificacion -= Time.deltaTime;
        if (timerNotificacion <= 0)
        {
            panelNotificacion.SetActive(false);
        }
    }

    void ToggleInventario()
    {
        if (panelInventario == null) return;
        bool estaActivo = panelInventario.activeSelf;
        panelInventario.SetActive(!estaActivo);

        if (!estaActivo)
        {
            ActualizarTextoInventario();
        }
    }

    void ActualizarTextoInventario()
    {
        if (textoInventario == null) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.inventario == null) return;

        textoInventario.text = GameManager.Instance.inventario.MostrarInventario();
    }
}