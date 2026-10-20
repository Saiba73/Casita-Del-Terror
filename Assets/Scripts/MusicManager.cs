using UnityEngine;

public class MusicManager : MonoBehaviour
{
    
    [SerializeField] private Enimgo1Patrullar[] perseguidores;
    [SerializeField] private EnemigoSaltador[] saltadores;

    
    [SerializeField] private AudioSource music;
    [SerializeField] private float volumenMaximo = 1f;
    [SerializeField] private float velocidadFadeIn = 2f;
    [SerializeField] private float velocidadFadeOut = 1f;
    
    [SerializeField] private float retrasoApagado = 2f;

    private float tiempoUltimaAmenaza = -999f;

    void Start()
    {
        music.loop = true;
        music.volume = 0f;
        music.Stop();
    }

    void Update()
    {
        if (HayAmenaza())
        {
            tiempoUltimaAmenaza = Time.time;
        }

        bool debeSonar = Time.time - tiempoUltimaAmenaza <= retrasoApagado;

        if (debeSonar && !music.isPlaying)
        {
            music.Play();
        }

        float objetivo = debeSonar ? volumenMaximo : 0f;
        float velocidad = debeSonar ? velocidadFadeIn : velocidadFadeOut;
        music.volume = Mathf.MoveTowards(music.volume, objetivo, velocidad * Time.deltaTime);

        if (!debeSonar && music.volume <= 0f && music.isPlaying)
        {
            music.Stop();
        }
    }

    private bool HayAmenaza()
    {
        
        foreach (var p in perseguidores)
        {
            if (p != null && p.EstaPersiguiendo) return true;
        }

        foreach (var s in saltadores)
        {
            if (s != null && s.EstaSaltando) return true;
        }

        return false;
    }
}