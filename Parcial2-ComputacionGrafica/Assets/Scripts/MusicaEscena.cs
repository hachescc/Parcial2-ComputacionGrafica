using UnityEngine;

public class MusicaEscena : MonoBehaviour
{
    public string nombreEscena;

    void Start()
    {
        if (GestorAudio.Instance != null)
            GestorAudio.Instance.ReproducirMusica(nombreEscena);
    }
}