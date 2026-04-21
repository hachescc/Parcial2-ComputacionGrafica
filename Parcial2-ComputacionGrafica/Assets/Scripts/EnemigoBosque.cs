using UnityEngine;

public class EnemigoBosque : MonoBehaviour
{
    [Header("Tipo de enemigo")]
    public int tipoEnemigo;

    void Awake()
    {
        Personaje personaje = GetComponent<Personaje>();
        if (personaje == null) return;

        switch (tipoEnemigo)
        {
            case 1:
                personaje.nombre = "Enemigo Bosque 1";
                personaje.fuerza = 15;
                personaje.resistencia = 13;
                break;
            case 2:
                personaje.nombre = "Enemigo Bosque 2";
                personaje.fuerza = 18;
                personaje.resistencia = 21;
                break;
            case 3:
                personaje.nombre = "Enemigo Bosque 3";
                personaje.fuerza = 12;
                personaje.resistencia = 23;
                break;
            case 4:
                personaje.nombre = "Enemigo Bosque 4";
                personaje.fuerza = 19;
                personaje.resistencia = 35;
                break;
        }
    }

    public int Atacar()
    {
        switch (tipoEnemigo)
        {
            case 1: return Random.Range(1, 7);                          // 1D6
            case 2: return Random.Range(1, 11);                         // 1D10
            case 3: return Random.Range(1, 5);                          // 1D4
            case 4: return Random.Range(1, 11);                         // 1D10
        }
        return 0;
    }

    public void EntregarDrops()
    {
        if (GameManager.Instance == null) return;
        Inventario inv = GameManager.Instance.inventario;
        if (inv == null) return;

        switch (tipoEnemigo)
        {
            case 1:
                inv.AgregarObjeto("pocion");
                inv.AgregarObjeto("pocion");
                inv.AgregarObjeto("pocion");
                inv.AgregarDinero(100);
                break;
            case 2:
                inv.AgregarObjeto("cofre tesoro");
                break;
            case 3:
                inv.AgregarObjeto("pocion");
                inv.AgregarObjeto("pocion");
                inv.AgregarObjeto("pocion");
                inv.AgregarObjeto("pocion");
                inv.AgregarDinero(250);
                break;
            case 4:
                inv.AgregarObjeto("pocion");
                inv.AgregarDinero(350);
                break;
        }

        Debug.Log("Drops entregados del enemigo tipo " + tipoEnemigo);
    }
}