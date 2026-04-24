using UnityEngine;
using UnityEngine.UI;

public class DialogoPNJ : MonoBehaviour
{
    public Cinematicas cinematicas;
    
    [Header("UI Dialogo")]
    public GameObject panelDialogo;
    public Text textoPNJ;
    public Text textoNombrePNJ;

    [Header("Dialogos")]
    public string nombrePNJ;
    public string[] lineasDialogo;

    private int indiceActual;
    private bool dialogoActivo;

    void Start()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }

    void Update()
    {
        if (dialogoActivo && Input.GetKeyDown(KeyCode.E))
        {
            SiguienteLinea();
        }
    }

    public void IniciarDialogo()
    {
        if (lineasDialogo.Length == 0) return;

        indiceActual = 0;
        dialogoActivo = true;
        panelDialogo.SetActive(true);

        if (textoNombrePNJ != null)
        {
            textoNombrePNJ.text = nombrePNJ;
        }

        textoPNJ.text = lineasDialogo[indiceActual];
        Debug.Log("Dialogo iniciado con: " + nombrePNJ);
    }

    void SiguienteLinea()
    {
        indiceActual++;

        if (indiceActual < lineasDialogo.Length)
        {
            textoPNJ.text = lineasDialogo[indiceActual];
        }
        else
        {
            TerminarDialogo();
        }
    }

    void TerminarDialogo()
    {
        dialogoActivo = false;
        panelDialogo.SetActive(false);
        indiceActual = 0;
        Debug.Log("Dialogo terminado con: " + nombrePNJ);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            cinematicas.StartCoroutine(cinematicas.StartCinematica());
            IniciarDialogo();
        }
    }
}