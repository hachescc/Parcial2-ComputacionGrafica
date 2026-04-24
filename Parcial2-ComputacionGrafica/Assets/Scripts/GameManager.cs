using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Personajes")]
    public Personaje[] heroes = new Personaje[4];
    public Personaje enemigoCombate;
    public GameObject prefabEnemigoCombate;
    public Personaje lupus;
    public Personaje helena;

    [Header("Inventario")]
    public Inventario inventario;

    [Header("Estado del juego")]
    public bool combateConBoss;
    public int heroesVivos;
    public string escenaRetornoCombate = "Juego";
    public string nombreContenedorHeroes = "Heroes";

    private Dictionary<int, Vector3> posicionesLocalesHeroesPorIndice = new Dictionary<int, Vector3>();
    private Vector3 posicionGrupoHeroesGuardada;
    private bool hayPosicionGrupoGuardada;
    private bool hayPosicionesHeroesGuardadas;
    private bool restaurarPosicionesPendiente;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Importante: en escenas como Combate este componente puede vivir junto a HUD/SistemaCombate.
            // Si destruimos el GameObject completo, rompemos botones y logica de combate.
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void PrepararEncuentro(string escenaActual)
    {
        if (!string.IsNullOrEmpty(escenaActual))
        {
            escenaRetornoCombate = escenaActual;
        }

        SincronizarHeroesConEscena();
        GuardarPosicionGrupoHeroes();
        GuardarPosicionesHeroesEscena();
        restaurarPosicionesPendiente = true;
        Debug.Log("Encuentro preparado. Escena retorno: " + escenaRetornoCombate);
    }

    void GuardarPosicionGrupoHeroes()
    {
        GameObject grupoHeroes = BuscarContenedorHeroes();
        if (grupoHeroes != null)
        {
            posicionGrupoHeroesGuardada = grupoHeroes.transform.position;
            hayPosicionGrupoGuardada = true;
        }
    }

    void GuardarPosicionesHeroesEscena()
    {
        posicionesLocalesHeroesPorIndice.Clear();
        hayPosicionesHeroesGuardadas = false;

        GameObject grupoHeroes = BuscarContenedorHeroes();
        Transform raiz = grupoHeroes != null ? grupoHeroes.transform : null;

        Heroe[] heroesEscena = FindObjectsByType<Heroe>(FindObjectsSortMode.None);
        foreach (Heroe heroeComp in heroesEscena)
        {
            if (heroeComp == null) continue;

            int indice = heroeComp.numeroHeroe - 1;
            if (indice < 0 || indice >= heroes.Length) continue;

            Vector3 posicionLocal = heroeComp.transform.localPosition;
            if (raiz != null && heroeComp.transform.parent != raiz)
            {
                posicionLocal = raiz.InverseTransformPoint(heroeComp.transform.position);
            }

            posicionesLocalesHeroesPorIndice[indice] = posicionLocal;
            hayPosicionesHeroesGuardadas = true;
        }
    }

    void RestaurarPosicionesHeroesEscena()
    {
        if (!hayPosicionesHeroesGuardadas) return;

        Heroe[] heroesEscena = FindObjectsByType<Heroe>(FindObjectsSortMode.None);
        foreach (Heroe heroeComp in heroesEscena)
        {
            if (heroeComp == null) continue;

            int indice = heroeComp.numeroHeroe - 1;
            if (indice < 0 || indice >= heroes.Length) continue;

            Vector3 posicionLocalGuardada;
            if (!posicionesLocalesHeroesPorIndice.TryGetValue(indice, out posicionLocalGuardada)) continue;

            Transform parent = heroeComp.transform.parent;
            if (parent != null)
            {
                heroeComp.transform.localPosition = posicionLocalGuardada;
            }
            else
            {
                heroeComp.transform.position = posicionLocalGuardada;
            }

            Vector3 posicionMundo = heroeComp.transform.position;

            Rigidbody2D rb = heroeComp.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = new Vector2(posicionMundo.x, posicionMundo.y);
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Combate") return;

        SincronizarHeroesConEscena();

        if (restaurarPosicionesPendiente && scene.name == escenaRetornoCombate)
        {
            restaurarPosicionesPendiente = false;
            StartCoroutine(RestaurarPosicionesAlFinalFrame());
        }
    }

    IEnumerator RestaurarPosicionesAlFinalFrame()
    {
        int esperaMaximaFrames = 60;
        int framesEsperados = 0;
        while (BuscarContenedorHeroes() == null && framesEsperados < esperaMaximaFrames)
        {
            framesEsperados++;
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        Debug.Log("Iniciando restauracion de posiciones en escena: " + SceneManager.GetActiveScene().name);

        // Reaplicamos varias veces para evitar que otro Start/Awake pise la posicion al volver de combate.
        for (int i = 0; i < 8; i++)
        {
            RestaurarPosicionGrupoHeroes();
            RestaurarPosicionesHeroesEscena();
            yield return null;
        }

        Debug.Log("Restauracion de posiciones finalizada.");
    }

    void RestaurarPosicionGrupoHeroes()
    {
        if (!hayPosicionGrupoGuardada) return;

        GameObject grupoHeroes = BuscarContenedorHeroes();
        if (grupoHeroes != null)
        {
            grupoHeroes.transform.position = posicionGrupoHeroesGuardada;
        }
    }

    GameObject BuscarContenedorHeroes()
    {
        GameObject grupoHeroes = GameObject.Find(nombreContenedorHeroes);
        if (grupoHeroes != null) return grupoHeroes;

        Heroe[] heroesEscena = FindObjectsByType<Heroe>(FindObjectsSortMode.None);
        foreach (Heroe heroeComp in heroesEscena)
        {
            if (heroeComp != null && heroeComp.transform.parent != null)
            {
                return heroeComp.transform.parent.gameObject;
            }
        }

        return null;
    }

    void SincronizarHeroesConEscena()
    {
        Heroe[] heroesEscena = FindObjectsByType<Heroe>(FindObjectsSortMode.None);
        foreach (Heroe heroeComp in heroesEscena)
        {
            Personaje personaje = heroeComp.GetComponent<Personaje>();
            int indice = heroeComp.numeroHeroe - 1;

            if (personaje == null) continue;
            if (indice < 0 || indice >= heroes.Length) continue;

            heroes[indice] = personaje;
        }
    }

    public void RegistrarPrefabEnemigo(GameObject prefabEnemigo)
    {
        prefabEnemigoCombate = prefabEnemigo;
    }


    public void RegistrarEnemigo(Personaje enemigo)
    {
        enemigoCombate = enemigo;
    }

    public void ActualizarHeroesVivos()
    {
        heroesVivos = 0;
        foreach (Personaje heroe in heroes)
        {
            if (heroe != null && heroe.estaVivo)
            {
                heroesVivos++;
            }
        }
    }

    public bool TodosLoHeroesMuertos()
    {
        ActualizarHeroesVivos();
        return heroesVivos == 0;
    }
}
