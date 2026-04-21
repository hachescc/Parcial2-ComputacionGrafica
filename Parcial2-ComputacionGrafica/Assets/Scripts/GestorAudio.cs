using UnityEngine;

public class GestorAudio : MonoBehaviour
{
    public static GestorAudio Instance;

    [Header("Musica por escena")]
    public AudioClip musicaMenu;
    public AudioClip musicaPueblo;
    public AudioClip musicaJuego;
    public AudioClip musicaCombate;
    public AudioClip musicaBoss;
    public AudioClip musicaFin;

    [Header("Efectos de sonido")]
    public AudioClip sonidoAtaque;
    public AudioClip sonidoRecibirDaño;
    public AudioClip sonidoRecolectar;
    public AudioClip sonidoMuerteEnemigo;
    public AudioClip sonidoPifia;

    [Header("Fuentes de audio")]
    public AudioSource fuenteMusica;
    public AudioSource fuenteEfectos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReproducirMusica(string nombreEscena)
    {
        if (fuenteMusica == null) return;

        AudioClip clip = null;

        switch (nombreEscena)
        {
            case "Menu":    clip = musicaMenu;    break;
            case "Pueblo":  clip = musicaPueblo;  break;
            case "Juego":   clip = musicaJuego;   break;
            case "Combate": clip = musicaCombate; break;
            case "Boss":    clip = musicaBoss;    break;
            case "Fin":     clip = musicaFin;     break;
        }

        if (clip == null) return;
        if (fuenteMusica.clip == clip) return;

        fuenteMusica.clip = clip;
        fuenteMusica.loop = true;
        fuenteMusica.Play();

        Debug.Log("Reproduciendo música: " + nombreEscena);
    }

    public void ReproducirEfecto(string nombreEfecto)
    {
        if (fuenteEfectos == null) return;

        AudioClip clip = null;

        switch (nombreEfecto)
        {
            case "ataque":         clip = sonidoAtaque;        break;
            case "daño":           clip = sonidoRecibirDaño;   break;
            case "recolectar":     clip = sonidoRecolectar;    break;
            case "muerte":         clip = sonidoMuerteEnemigo; break;
            case "pifia":          clip = sonidoPifia;         break;
        }

        if (clip == null) return;
        fuenteEfectos.PlayOneShot(clip);

        Debug.Log("Efecto reproducido: " + nombreEfecto);
    }

    public void DetenerMusica()
    {
        if (fuenteMusica == null) return;
        fuenteMusica.Stop();
    }
}