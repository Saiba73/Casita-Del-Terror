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
    [SerializeField] private float velocidadCaidaStun = 12f;
    private float tiempoFinStun;

    [Header("Animacion")]
    [SerializeField] private Animator e2Animator;

    private int indicePuntos;
    private NavMeshAgent agent;
    private Coroutine corrutinaSaltoActual;

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
                e2Animator.SetInteger("EstadoAnimacion", 0);
                
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    ElegirNuevoPuntoPatrulla();
                }

                if (distanciaAJugador <= rangoAtaque && Time.time >= tiempoSiguienteSalto)
                {
                    corrutinaSaltoActual = StartCoroutine(RutinaSalto(jugador.position));
                }
                break;

            case EstadosEnemigo.Saltando:
                
                break;

            case EstadosEnemigo.Stuneado:
                e2Animator.SetInteger("EstadoAnimacion", 2);
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
        Debug.Log("TOCO LA ALGO");
        if (other.CompareTag("Pala"))
        {
            Debug.Log("COLISION PALA");
            AplicarStun();
        }
    }

    IEnumerator RutinaSalto(Vector3 posObjetivo)
    {
        estadoEnemigo = EstadosEnemigo.Saltando;
        e2Animator.SetInteger("EstadoAnimacion", 1);

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
        corrutinaSaltoActual = null;
    }

    public void AplicarStun()
    {
        
        if (corrutinaSaltoActual != null)
        {
            StopCoroutine(corrutinaSaltoActual);
            corrutinaSaltoActual = null;
        }

        estadoEnemigo = EstadosEnemigo.Stuneado;
        tiempoFinStun = Time.time + duracionStun;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        
        StartCoroutine(RutinaCaidaStun());
    }

    IEnumerator RutinaCaidaStun()
    {
        
        Vector3 posSuelo = transform.position;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            posSuelo = hit.position;
        }
        else if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit rayHit, 10f))
        {
            posSuelo = rayHit.point;
        }

        
        while (transform.position.y > posSuelo.y + 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posSuelo, velocidadCaidaStun * Time.deltaTime);
            yield return null;
        }

        
        transform.position = posSuelo;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(posSuelo);
            agent.isStopped = true;
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