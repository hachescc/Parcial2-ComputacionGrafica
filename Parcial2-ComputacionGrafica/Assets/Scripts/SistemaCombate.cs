using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SistemaCombate : MonoBehaviour
{
    public static SistemaCombate Instance;

    [Header("Participantes")]
    public Personaje[] heroes = new Personaje[4];
    public Personaje enemigo;

    [Header("Estado del combate")]
    public string nombreTurnoActual;
    public string resultadoUltimaAccion;
    public bool combateActivo;

    [Header("Estructuras de datos")]
    public Queue<Personaje> colaTurnos = new Queue<Personaje>();
    public Stack<string> logAcciones = new Stack<string>();

    private Personaje personajeActual;

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

    public void IniciarCombate()
    {
        colaTurnos.Clear();

        foreach (Personaje heroe in heroes)
        {
            if (heroe != null && heroe.estaVivo)
            {
                colaTurnos.Enqueue(heroe);
            }
        }

        if (enemigo != null)
        {
            colaTurnos.Enqueue(enemigo);
        }

        combateActivo = true;
        Debug.Log("Combate iniciado. Turnos: " + colaTurnos.Count);

        StartCoroutine(SiguienteTurno());
    }

    public IEnumerator SiguienteTurno()
    {
        yield return new WaitForSeconds(1f);

        if (!combateActivo) yield break;

        if (TodosMuertos()) yield break;

        personajeActual = colaTurnos.Dequeue();

        if (!personajeActual.estaVivo)
        {
            StartCoroutine(SiguienteTurno());
            yield break;
        }

        nombreTurnoActual = personajeActual.nombre;
        Debug.Log("Turno de: " + personajeActual.nombre);

        if (personajeActual.CompareTag("Enemigo"))
        {
            yield return new WaitForSeconds(1f);
            TurnoEnemigo();
        }
    }

    public void AtacarConHeroe(int indiceAtaque)
    {
        if (!combateActivo) return;
        if (personajeActual == null) return;
        if (personajeActual.CompareTag("Enemigo")) return;

        int tiradaExito = TiradaExito();

        if (tiradaExito <= 3)
        {
            resultadoUltimaAccion = "Error — " + personajeActual.nombre + " fallo el ataque";
            RegistrarAccion(resultadoUltimaAccion);
            Debug.Log(resultadoUltimaAccion);
            if (HUDController.Instance != null)
                HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
            if (GestorAudio.Instance != null)
                GestorAudio.Instance.ReproducirEfecto("pifia");
            TerminarTurno();
            return;
        }

        int tiradaDano = TiradaDano();
        string evaluacion = EvaluarDano(tiradaDano);
        resultadoUltimaAccion = evaluacion;

        if (evaluacion == "Dano")
        {
            enemigo.getDamage(25);
            resultadoUltimaAccion = "Dano — " + personajeActual.nombre + " hizo 25 de dano";
            if (GestorAudio.Instance != null)
                GestorAudio.Instance.ReproducirEfecto("ataque");

            if (!enemigo.estaVivo)
            {
                resultadoUltimaAccion += " | Enemigo derrotado";
                combateActivo = false;
                RegistrarAccion(resultadoUltimaAccion);
                Debug.Log(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
                if (GestorAudio.Instance != null)
                    GestorAudio.Instance.ReproducirEfecto("muerte");
                StartCoroutine(TerminarCombate(true));
                return;
            }
        }
        else
        {
            resultadoUltimaAccion = evaluacion + " — " + personajeActual.nombre;
        }

        RegistrarAccion(resultadoUltimaAccion);
        Debug.Log(resultadoUltimaAccion);
        if (HUDController.Instance != null)
            HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
        TerminarTurno();
    }

    void TurnoEnemigo()
    {
        if (!combateActivo) return;

        int tiradaExito = TiradaExito();

        if (tiradaExito <= 3)
        {
            resultadoUltimaAccion = "Error — " + enemigo.nombre + " fallo el ataque";
            RegistrarAccion(resultadoUltimaAccion);
            Debug.Log(resultadoUltimaAccion);
            if (HUDController.Instance != null)
                HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
            if (GestorAudio.Instance != null)
                GestorAudio.Instance.ReproducirEfecto("pifia");
            TerminarTurno();
            return;
        }

        int tiradaDano = TiradaDano();
        string evaluacion = EvaluarDano(tiradaDano);
        resultadoUltimaAccion = evaluacion;

        if (evaluacion == "Dano")
        {
            foreach (Personaje heroe in heroes)
            {
                if (heroe != null && heroe.estaVivo)
                {
                    heroe.getDamage(25);
                    resultadoUltimaAccion = "Dano — " + enemigo.nombre + " hizo 25 de dano a " + heroe.nombre;
                    if (GestorAudio.Instance != null)
                        GestorAudio.Instance.ReproducirEfecto("dano");
                    break;
                }
            }

            if (GameManager.Instance != null && GameManager.Instance.TodosLoHeroesMuertos())
            {
                combateActivo = false;
                RegistrarAccion(resultadoUltimaAccion);
                if (HUDController.Instance != null)
                    HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
                StartCoroutine(TerminarCombate(false));
                return;
            }
        }
        else
        {
            resultadoUltimaAccion = evaluacion + " — " + enemigo.nombre;
        }

        RegistrarAccion(resultadoUltimaAccion);
        Debug.Log(resultadoUltimaAccion);
        if (HUDController.Instance != null)
            HUDController.Instance.MostrarNotificacion(resultadoUltimaAccion);
        TerminarTurno();
    }

    void TerminarTurno()
    {
        if (personajeActual.estaVivo)
        {
            colaTurnos.Enqueue(personajeActual);
        }

        StartCoroutine(SiguienteTurno());
    }

    IEnumerator TerminarCombate(bool victoria)
    {
        yield return new WaitForSeconds(2f);

        if (victoria)
        {
            Debug.Log("Victoria — volviendo al bosque");
            SceneManager.LoadScene("Juego");
        }
        else
        {
            Debug.Log("Derrota — Game Over");
            SceneManager.LoadScene("Menu");
        }
    }

    bool TodosMuertos()
    {
        foreach (Personaje heroe in heroes)
        {
            if (heroe != null && heroe.estaVivo) return false;
        }
        combateActivo = false;
        return true;
    }

    public int TiradaExito()
    {
        return Random.Range(1, 11);
    }

    public int TiradaDano()
    {
        int decenas = Random.Range(0, 10);
        int unidades = Random.Range(0, 10);
        return decenas * 10 + unidades;
    }

    public string EvaluarDano(int tirada)
    {
        if (tirada >= 90) return "Pifia";
        if (tirada >= 50) return "Dano";
        return "Fallo";
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