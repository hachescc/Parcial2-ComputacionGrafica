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
    public Transform puntoSpawnEnemigo;

    [Header("Estado del combate")]
    public string nombreTurnoActual;
    public string resultadoUltimaAccion;
    public bool combateActivo;

    [Header("Estructuras de datos")]
    public Queue<Personaje> colaTurnos = new Queue<Personaje>();
    public Stack<string> logAcciones = new Stack<string>();

    private Personaje personajeActual;
    private bool combateInicializado = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(EsperarCargaEnemigo());
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

    IEnumerator EsperarCargaEnemigo()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        float tiempoEspera = 0f;
        while (GameManager.Instance.prefabEnemigoCombate == null && tiempoEspera < 2f)
        {
            tiempoEspera += Time.deltaTime;
            yield return null;
        }

        if (GameManager.Instance.prefabEnemigoCombate == null)
        {
            GeneradorEncuentros generador = FindFirstObjectByType<GeneradorEncuentros>();
            if (generador != null && generador.prefabsEnemigos != null && generador.prefabsEnemigos.Length > 0)
            {
                GameObject fallback = generador.prefabsEnemigos[0];
                if (fallback != null)
                {
                    GameManager.Instance.RegistrarPrefabEnemigo(fallback);
                    Debug.LogWarning("Se uso prefab enemigo de respaldo desde GeneradorEncuentros.");
                }
            }
        }

        if (GameManager.Instance.prefabEnemigoCombate == null)
        {
            Debug.LogError("No hay prefab enemigo disponible para iniciar combate.");
            yield break;
        }

        // Si GameManager no tiene heroes validos (por cambio de escena), usamos los ya asignados en esta escena.
        if (GameManager.Instance.heroes != null && GameManager.Instance.heroes.Length > 0)
        {
            bool tieneAlMenosUnHeroeVivo = false;
            for (int i = 0; i < GameManager.Instance.heroes.Length; i++)
            {
                Personaje heroe = GameManager.Instance.heroes[i];
                if (heroe != null && heroe.estaVivo)
                {
                    tieneAlMenosUnHeroeVivo = true;
                    break;
                }
            }

            if (tieneAlMenosUnHeroeVivo)
            {
                heroes = GameManager.Instance.heroes;
            }
        }

        Vector3 posicionSpawn = transform.position;
        Quaternion rotacionSpawn = Quaternion.identity;

        if (enemigo != null)
        {
            // Si hay placeholder de enemigo en escena, usamos su posicion para no aparecer fuera de camara.
            posicionSpawn = enemigo.transform.position;
            rotacionSpawn = enemigo.transform.rotation;
            Destroy(enemigo.gameObject);
        }

        if (puntoSpawnEnemigo != null)
        {
            posicionSpawn = puntoSpawnEnemigo.position;
            rotacionSpawn = puntoSpawnEnemigo.rotation;
        }

        GameObject enemigoObj = Instantiate(GameManager.Instance.prefabEnemigoCombate, posicionSpawn, rotacionSpawn);
        enemigo = enemigoObj.GetComponent<Personaje>();
        Debug.Log("Enemigo instanciado en combate: " + enemigoObj.name + " en posicion " + posicionSpawn);

        if (enemigo == null)
        {
            Debug.LogError("El prefab del enemigo no tiene componente Personaje.");
            yield break;
        }

        GameManager.Instance.RegistrarEnemigo(enemigo);

        if (combateInicializado) yield break;

        combateInicializado = true;
        IniciarCombate();
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
            resultadoUltimaAccion = "Error - " + personajeActual.nombre + " fallo el ataque";
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
            resultadoUltimaAccion = "Dano - " + personajeActual.nombre + " hizo 25 de dano";
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
                EntregarDropsYNotificar();
                StartCoroutine(TerminarCombate(true));
                return;
            }
        }
        else
        {
            resultadoUltimaAccion = evaluacion + " - " + personajeActual.nombre;
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
            resultadoUltimaAccion = "Error - " + enemigo.nombre + " fallo el ataque";
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
                    resultadoUltimaAccion = "Dano - " + enemigo.nombre + " hizo 25 de dano a " + heroe.nombre;
                    if (GestorAudio.Instance != null)
                        GestorAudio.Instance.ReproducirEfecto("dano");
                    break;
                }
            }

            if (TodosMuertos())
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
            resultadoUltimaAccion = evaluacion + " - " + enemigo.nombre;
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
            Debug.Log("Victoria - volviendo al bosque");
            string escenaRetorno = "Juego";
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.escenaRetornoCombate))
            {
                escenaRetorno = GameManager.Instance.escenaRetornoCombate;
            }
            SceneManager.LoadScene(escenaRetorno);
        }
        else
        {
            Debug.Log("Derrota - Game Over");
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
        if (tirada >= 35) return "Dano";
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

    void EntregarDropsYNotificar()
    {
        if (GameManager.Instance == null) return;
        Inventario inv = GameManager.Instance.inventario;
        if (inv == null) return;

        int tiposPrevios = inv.cantidades.Count;

        EnemigoBosque enemigoScript = enemigo.GetComponent<EnemigoBosque>();
        if (enemigoScript != null)
        {
            enemigoScript.EntregarDrops();
        }

        if (inv.cantidades.Count > tiposPrevios)
        {
            StartCoroutine(NotificacionObjetoNuevo());
        }
    }

    IEnumerator NotificacionObjetoNuevo()
    {
        yield return new WaitForSeconds(2.5f);
        if (HUDController.Instance != null)
        {
            HUDController.Instance.MostrarNotificacion(
                "Has obtenido un nuevo objeto.\nRevisa tu inventario (I)"
            );
        }
    }
}
