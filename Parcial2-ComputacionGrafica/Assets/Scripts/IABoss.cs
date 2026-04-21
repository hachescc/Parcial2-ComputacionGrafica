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

        // Lupus decide si regenerar salud primero
        float porcentajeSalud = lupus.saludActual / lupus.resistencia;
        if (porcentajeSalud < porcentajeRegenerar)
        {
            lupus.curar(cantidadRegeneracion);
            resultadoUltimaAccion = "Lupus usó regeneración — salud: " + lupus.saludActual;
            RegistrarAccion(resultadoUltimaAccion);
            Debug.Log(resultadoUltimaAccion);
            StartCoroutine(TurnoBoss());
            yield break;
        }

        // Tirada de IA
        int tirada = Random.Range(1, 11);
        Debug.Log("Tirada IA Boss: " + tirada);

        // Helena viva
        if (helena != null && helena.estaVivo)
        {
            if (tirada > 7 && tirada <= 9)
            {
                resultadoUltimaAccion = "Pifia — Helena no ataca este turno";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
            }
            else if (tirada < 7 && tirada > 0)
            {
                int daño = Random.Range(1, 9) + Random.Range(1, 7);
                resultadoUltimaAccion = AplicarDañoAHeroe(helena.nombre, daño);
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
            }
            else
            {
                // Tirada 7 o 10 — no está en los rangos, se trata como pifia
                resultadoUltimaAccion = "Pifia — Helena no ataca (tirada: " + tirada + ")";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
            }
        }
        // Helena muerta — ataca Lupus directamente
        else
        {
            if (tirada > 7 && tirada <= 9)
            {
                resultadoUltimaAccion = "Pifia — Lupus no ataca este turno";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
            }
            else if (tirada < 7 && tirada > 0)
            {
                // Lupus elige aleatoriamente entre Ataque1 y Ataque2
                int ataqueElegido = Random.Range(0, 2);
                int daño;
                if (ataqueElegido == 0)
                {
                    daño = Random.Range(1, 5) + Random.Range(1, 7); // 1D4 + 1D6
                }
                else
                {
                    daño = Random.Range(1, 7) + Random.Range(1, 7); // 2D6
                }
                resultadoUltimaAccion = AplicarDañoAHeroe(lupus.nombre, daño);
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
            }
            else
            {
                resultadoUltimaAccion = "Pifia — Lupus no ataca (tirada: " + tirada + ")";
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
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

    string AplicarDañoAHeroe(string atacante, int daño)
    {
        foreach (Personaje heroe in heroes)
        {
            if (heroe != null && heroe.estaVivo)
            {
                heroe.getDamage(daño);
                return "Daño — " + atacante + " hizo " + daño + " de daño a " + heroe.nombre;
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
            Debug.Log("Lupus derrotado — cargando escena Fin");
            SceneManager.LoadScene("Fin");
        }
        else
        {
            Debug.Log("Héroes derrotados — Game Over");
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