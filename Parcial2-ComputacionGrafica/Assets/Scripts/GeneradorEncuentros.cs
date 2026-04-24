using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneradorEncuentros : MonoBehaviour
{
    [Header("Configuracion")]
    public float probabilidadEncuentro = 0.3f;
    public float tiempoEntreEncuentros = 5f;

    private float timerEncuentro;
    private bool encuentroActivo;

    [Header("Prefabs enemigos")]
    public GameObject[] prefabsEnemigos = new GameObject[4];

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Combate")
        {
            enabled = false;
            return;
        }

        timerEncuentro = tiempoEntreEncuentros;
        encuentroActivo = false;
    }

    void Update()
    {
        if (encuentroActivo) return;

        timerEncuentro -= Time.deltaTime;

        if (timerEncuentro <= 0)
        {
            timerEncuentro = tiempoEntreEncuentros;
            VerificarEncuentro();
        }
    }

    void VerificarEncuentro()
    {
        float tirada = Random.Range(0f, 1f);

        if (tirada <= probabilidadEncuentro)
        {
            IniciarEncuentro();
        }
    }

    void IniciarEncuentro()
    {
        encuentroActivo = true;

        // Elegir enemigo aleatorio entre los 4 tipos
        int tipoEnemigo = Random.Range(0, 4);

        if (GameManager.Instance != null && prefabsEnemigos[tipoEnemigo] != null)
        {
            // Guardamos el prefab para instanciarlo dentro de la escena de combate.
            GameManager.Instance.RegistrarPrefabEnemigo(prefabsEnemigos[tipoEnemigo]);
        }

        Debug.Log("Encuentro iniciado - cargando combate");
        SceneManager.LoadScene("Combate");
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (encuentroActivo) return;

        if (otro.CompareTag("Player"))
        {
            float tirada = Random.Range(0f, 1f);
            if (tirada <= probabilidadEncuentro)
            {
                IniciarEncuentro();
            }
        }
    }
}
