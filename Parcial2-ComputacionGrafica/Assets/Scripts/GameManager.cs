using UnityEngine;

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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Importante: en escenas como Combate este componente puede vivir junto a HUD/SistemaCombate.
            // Si destruimos el GameObject completo, rompemos botones y logica de combate.
            Destroy(this);
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
