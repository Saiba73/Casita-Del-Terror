using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoSaltador : MonoBehaviour
{
    enum EstadosEnemigo
    {
        Patrullando,
        Saltando,
        Stuneado
    }

    [SerializeField] private EstadosEnemigo estadoEnemigo = EstadosEnemigo.Patrullando;

    [Header("Puntos de patrullaje")]
    [SerializeField] private Transform[] puntoPatrulla;

    [Header("Referencia de jugador")]
    [SerializeField] private Transform jugador;

    [Header("Ajustes de Salto")]
    [SerializeField] private float rangoAtaque = 6f;
    [SerializeField] private float alturaSalto = 3f;
    [SerializeField] private float duracionSalto = 1.2f;
    [SerializeField] private float tiempoEntreSaltos = 2f;
    private float tiempoSiguienteSalto;

    [Header("Ajustes de Stun")]
    [SerializeField] private float duracionStun = 5f;
    private float tiempoFinStun;

    private int indicePuntos;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (puntoPatrulla != null && puntoPatrulla.Length > 0)
        {
            indicePuntos = Random.Range(0, puntoPatrulla.Length);
            agent.SetDestination(puntoPatrulla[indicePuntos].position);
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaAJugador = Vector3.Distance(transform.position, jugador.position);

        switch (estadoEnemigo)
        {
            case EstadosEnemigo.Patrullando:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    ElegirNuevoPuntoPatrulla();
                }

                if (distanciaAJugador <= rangoAtaque && Time.time >= tiempoSiguienteSalto)
                {
                    StartCoroutine(RutinaSalto(jugador.position));
                }
                break;

            case EstadosEnemigo.Saltando:
                
                break;

            case EstadosEnemigo.Stuneado:
                if (Time.time >= tiempoFinStun)
                {
                    agent.enabled = true;
                    agent.isStopped = false;
                    ElegirNuevoPuntoPatrulla();
                    estadoEnemigo = EstadosEnemigo.Patrullando;
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pala"))
        {
            Debug.Log("COLISION PALA");
            estadoEnemigo = EstadosEnemigo.Stuneado;
            AplicarStun();
        }
    }

    IEnumerator RutinaSalto(Vector3 posObjetivo)
    {
        estadoEnemigo = EstadosEnemigo.Saltando;

        
        agent.enabled = false;

        Vector3 posInicio = transform.position;
        float tiempoTranscurrido = 0f;

        
        Vector3 direccionLook = (posObjetivo - posInicio).normalized;
        direccionLook.y = 0;
        if (direccionLook != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccionLook);
        }

        
        while (tiempoTranscurrido < duracionSalto)
        {
            float t = tiempoTranscurrido / duracionSalto;

            
            Vector3 posActual = Vector3.Lerp(posInicio, posObjetivo, t);

            
            posActual.y += Mathf.Sin(t * Mathf.PI) * alturaSalto;

            transform.position = posActual;

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        
        transform.position = posObjetivo;

        
        agent.enabled = true;
        agent.Warp(transform.position);

        
        tiempoSiguienteSalto = Time.time + tiempoEntreSaltos;
        ElegirNuevoPuntoPatrulla();
        estadoEnemigo = EstadosEnemigo.Patrullando;
    }

    public void AplicarStun()
    {
        
        StopAllCoroutines();

        estadoEnemigo = EstadosEnemigo.Stuneado;
        tiempoFinStun = Time.time + duracionStun;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    void ElegirNuevoPuntoPatrulla()
    {
        if (puntoPatrulla == null || puntoPatrulla.Length == 0) return;

        indicePuntos = Random.Range(0, puntoPatrulla.Length);
        
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(puntoPatrulla[indicePuntos].position);
        }
    }
}