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

    private Dictionary<string, Vector3> posicionesHeroesPorNombre = new Dictionary<string, Vector3>();
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
        GameObject grupoHeroes = GameObject.Find("Heroes");
        if (grupoHeroes != null)
        {
            posicionGrupoHeroesGuardada = grupoHeroes.transform.position;
            hayPosicionGrupoGuardada = true;
        }
    }

    void GuardarPosicionesHeroesEscena()
    {
        posicionesHeroesPorNombre.Clear();
        hayPosicionesHeroesGuardadas = false;

        string[] nombresHeroes = { "F.Heroe1", "F.Heroe2", "F.Heroe3", "F.Heroe4" };
        foreach (string nombreHeroe in nombresHeroes)
        {
            GameObject heroeObj = GameObject.Find(nombreHeroe);
            if (heroeObj == null) continue;

            posicionesHeroesPorNombre[nombreHeroe] = heroeObj.transform.position;
            hayPosicionesHeroesGuardadas = true;
        }

        Personaje[] personajesEscena = FindObjectsByType<Personaje>(FindObjectsSortMode.None);
        foreach (Personaje personaje in personajesEscena)
        {
            if (personaje == null) continue;

            string nombre = personaje.gameObject.name;
            bool esHeroe = nombre.StartsWith("F.Heroe") || personaje.CompareTag("Player");
            if (!esHeroe) continue;

            string clave = nombre;
            if (!posicionesHeroesPorNombre.ContainsKey(clave))
            {
                posicionesHeroesPorNombre.Add(clave, personaje.transform.position);
                hayPosicionesHeroesGuardadas = true;
            }
        }
    }

    void RestaurarPosicionesHeroesEscena()
    {
        if (!hayPosicionesHeroesGuardadas) return;

        string[] nombresHeroes = { "F.Heroe1", "F.Heroe2", "F.Heroe3", "F.Heroe4" };
        foreach (string nombreHeroe in nombresHeroes)
        {
            GameObject heroeObj = GameObject.Find(nombreHeroe);
            if (heroeObj == null) continue;

            Vector3 posicionGuardada;
            if (!posicionesHeroesPorNombre.TryGetValue(nombreHeroe, out posicionGuardada)) continue;

            heroeObj.transform.position = posicionGuardada;
            Rigidbody2D rbHeroe = heroeObj.GetComponent<Rigidbody2D>();
            if (rbHeroe != null)
            {
                rbHeroe.position = new Vector2(posicionGuardada.x, posicionGuardada.y);
                rbHeroe.linearVelocity = Vector2.zero;
            }
        }

        Personaje[] personajesEscena = FindObjectsByType<Personaje>(FindObjectsSortMode.None);
        foreach (Personaje personaje in personajesEscena)
        {
            if (personaje == null) continue;

            string nombre = personaje.gameObject.name;
            bool esHeroe = nombre.StartsWith("F.Heroe") || personaje.CompareTag("Player");
            if (!esHeroe) continue;

            Vector3 posicionGuardada;
            if (posicionesHeroesPorNombre.TryGetValue(nombre, out posicionGuardada))
            {
                personaje.transform.position = posicionGuardada;

                Rigidbody2D rb = personaje.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.position = new Vector2(posicionGuardada.x, posicionGuardada.y);
                    rb.linearVelocity = Vector2.zero;
                }
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
        yield return null;
        yield return null;
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

        GameObject grupoHeroes = GameObject.Find("Heroes");
        if (grupoHeroes != null)
        {
            grupoHeroes.transform.position = posicionGrupoHeroesGuardada;
        }
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
