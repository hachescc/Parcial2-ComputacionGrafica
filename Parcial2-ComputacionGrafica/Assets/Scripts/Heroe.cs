using UnityEngine;

public class Heroe : MonoBehaviour
{
    [Header("Tipo de heroe")]
    public int numeroHeroe;

    void Awake()
    {
        Personaje personaje = GetComponent<Personaje>();
        if (personaje == null) return;

        switch (numeroHeroe)
        {
            case 1:
                personaje.nombre = "Héroe 1";
                personaje.fuerza = 19;
                personaje.resistencia = 68;
                break;
            case 2:
                personaje.nombre = "Héroe 2";
                personaje.fuerza = 23;
                personaje.resistencia = 87;
                break;
            case 3:
                personaje.nombre = "Héroe 3";
                personaje.fuerza = 17;
                personaje.resistencia = 50;
                break;
            case 4:
                personaje.nombre = "Héroe 4";
                personaje.fuerza = 16;
                personaje.resistencia = 38;
                break;
        }
    }

    public int Atacar(int indiceAtaque)
    {
        switch (numeroHeroe)
        {
            case 1:
                if (indiceAtaque == 0) return Random.Range(1, 11) + Random.Range(1, 5); // 1D10 + 1D4
                if (indiceAtaque == 1) return Random.Range(1, 7) + Random.Range(1, 7);  // 2D6
                break;
            case 2:
                if (indiceAtaque == 0) return Random.Range(1, 11);                                              // 1D10
                if (indiceAtaque == 1) return Random.Range(1, 11) + Random.Range(1, 5);                        // 1D10 + 1D4
                if (indiceAtaque == 2) return Random.Range(1, 11) + Random.Range(1, 5) + Random.Range(1, 5);  // 1D10 + 2D4
                break;
            case 3:
                if (indiceAtaque == 0) return Random.Range(1, 11) + Random.Range(1, 7); // 1D10 + 1D6
                break;
            case 4:
                if (indiceAtaque == 0) return Random.Range(1, 7) + Random.Range(1, 9);  // 1D6 + 1D8
                break;
        }
        return 0;
    }
}