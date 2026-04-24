using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Vector3[] posicionesHeroesGuardadas = new Vector3[4];
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
        GuardarPosicionesHeroes();
        restaurarPosicionesPendiente = true;
    }

    void GuardarPosicionesHeroes()
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] != null)
            {
                posicionesHeroesGuardadas[i] = heroes[i].transform.position;
            }
        }
    }

    void RestaurarPosicionesHeroes()
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] != null)
            {
                heroes[i].transform.position = posicionesHeroesGuardadas[i];
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Combate") return;

        SincronizarHeroesConEscena();

        if (restaurarPosicionesPendiente && scene.name == escenaRetornoCombate)
        {
            RestaurarPosicionesHeroes();
            restaurarPosicionesPendiente = false;
        }
    }

    void SincronizarHeroesConEscena()
    {
        Heroe[] heroesEscena = FindObjectsOfType<Heroe>();
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
