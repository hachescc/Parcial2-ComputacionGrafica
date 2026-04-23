using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class IABoss : MonoBehaviour
{
    public static IABoss Instance;

    [Header("Participantes Boss")]
    public Personaje lupus;
    public Personaje helena;
    public Personaje[] heroes = new Personaje[4];

    [Header("Estado del Boss")]
    public bool combateBossActivo;
    public string resultadoUltimaAccion;

    [Header("Regeneracion")]
    public float porcentajeRegenerar = 0.30f;
    public float cantidadRegeneracion = 20f;

    [Header("Drops de Lupus")]
    public int minCofres = 2;
    public int maxCofres = 6;
    public int minDinero = 500;
    public int maxDinero = 1500;

    private Stack<string> logAcciones = new Stack<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IniciarCombateBoss()
    {
        combateBossActivo = true;
        Debug.Log("Combate Boss iniciado — Lupus y Helena atacan");
        if (GestorAudio.Instance != null)
            GestorAudio.Instance.ReproducirMusica("Boss");
        StartCoroutine(TurnoBoss());
    }

    public IEnumerator TurnoBoss()
    {
        yield return new WaitForSeconds(1.5f);

        if (!combateBossActivo) yield break;
        if (lupus == null || !lupus.estaVivo)
        {
            StartCoroutine(TerminarCombateBoss(true));
            yield break;
        }

        float porcentajeSalud = lupus.saludActual / lupus.resistencia;
        if (porcentajeSalud < porcentajeRegenerar)
        {
            lupus.curar(cantidadRegeneracion);
            resultadoUltimaAccion = "Lupus uso regeneracion — salud: " + lupus.saludActual;
            RegistrarAccion(resultadoUltimaAccion);
            Debug.Log(resultadoUltimaAccion);
            if (HUDController.Instance != null)
                HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
            if (GestorAudio.Instance != null)
                GestorAudio.Instance.ReproducirEfecto("ataque");
            StartCoroutine(TurnoBoss());
            yield break;
        }

        int tirada = Random.Range(1, 11);
        Debug.Log("Tirada IA Boss: " + tirada);

        if (helena != null && helena.estaVivo)
        {
            if (tirada > 7 && tirada <= 9)
            {
                resultadoUltimaAccion = "Pifia — Helena no ataca este turno";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
                if (GestorAudio.Instance != null)
                    GestorAudio.Instance.ReproducirEfecto("pifia");
            }
            else if (tirada < 7 && tirada > 0)
            {
                int dano = Random.Range(1, 9) + Random.Range(1, 7);
                resultadoUltimaAccion = AplicarDanoAHeroe(helena.nombre, dano);
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
            }
            else
            {
                resultadoUltimaAccion = "Pifia — Helena no ataca (tirada: " + tirada + ")";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
                if (GestorAudio.Instance != null)
                    GestorAudio.Instance.ReproducirEfecto("pifia");
            }
        }
        else
        {
            if (tirada > 7 && tirada <= 9)
            {
                resultadoUltimaAccion = "Pifia — Lupus no ataca este turno";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
                if (GestorAudio.Instance != null)
                    GestorAudio.Instance.ReproducirEfecto("pifia");
            }
            else if (tirada < 7 && tirada > 0)
            {
                int ataqueElegido = Random.Range(0, 2);
                int dano;
                if (ataqueElegido == 0)
                {
                    dano = Random.Range(1, 5) + Random.Range(1, 7);
                }
                else
                {
                    dano = Random.Range(1, 7) + Random.Range(1, 7);
                }
                resultadoUltimaAccion = AplicarDanoAHeroe(lupus.nombre, dano);
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
            }
            else
            {
                resultadoUltimaAccion = "Pifia — Lupus no ataca (tirada: " + tirada + ")";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
                if (GestorAudio.Instance != null)
                    GestorAudio.Instance.ReproducirEfecto("pifia");
            }
        }

        if (TodosLoHeroesMuertos())
        {
            combateBossActivo = false;
            StartCoroutine(TerminarCombateBoss(false));
            yield break;
        }

        StartCoroutine(TurnoBoss());
    }

    string AplicarDanoAHeroe(string atacante, int dano)
    {
        foreach (Personaje heroe in heroes)
        {
            if (heroe != null && heroe.estaVivo)
            {
                heroe.getDamage(dano);
                if (GestorAudio.Instance != null)
                    GestorAudio.Instance.ReproducirEfecto("dano");
                return "Dano — " + atacante + " hizo " + dano + " de dano a " + heroe.nombre;
            }
        }
        return "Sin objetivo";
    }

    bool TodosLoHeroesMuertos()
    {
        foreach (Personaje heroe in heroes)
        {
            if (heroe != null && heroe.estaVivo) return false;
        }
        return true;
    }

    IEnumerator TerminarCombateBoss(bool victoria)
    {
        yield return new WaitForSeconds(2f);

        if (victoria)
        {
            if (GameManager.Instance != null && GameManager.Instance.inventario != null)
            {
                int cantidadCofres = Random.Range(minCofres, maxCofres + 1);
                for (int i = 0; i < cantidadCofres; i++)
                {
                    GameManager.Instance.inventario.AgregarObjeto("cofre de oro");
                }
                GameManager.Instance.inventario.AgregarDinero(Random.Range(minDinero, maxDinero + 1));
                Debug.Log("Lupus derrotado — cofres entregados: " + cantidadCofres);
            }

            Debug.Log("Cargando escena Fin");
            SceneManager.LoadScene("Fin");
        }
        else
        {
            Debug.Log("Heroes derrotados — Game Over");
            SceneManager.LoadScene("Menu");
        }
    }

    public void RegistrarAccion(string mensaje)
    {
        logAcciones.Push(mensaje);
    }

    public string ObtenerUltimaAccion()
    {
        if (logAcciones.Count > 0)
        {
            return logAcciones.Peek();
        }
        return "";
    }
}