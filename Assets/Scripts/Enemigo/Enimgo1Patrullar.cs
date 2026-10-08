using UnityEngine;
using UnityEngine.AI;

public class Enimgo1Patrullar : MonoBehaviour
{
    enum EstadosPatrulla
    {
        Patrullando,
        Persiguiendo,
        Stuneado
    }

    [SerializeField] private EstadosPatrulla estadoEnemigo = EstadosPatrulla.Patrullando;

    [Header("Puntos de patrullaje")]
    [SerializeField] private Transform[] puntoPatrulla;

    [Header("Referencia de jugador")]
    [SerializeField] private Transform jugador;

    [Header("Ajustes de Persecución")]
    [SerializeField] private float intervaloActualizacionRuta = 0.2f;
    private float tiempoSiguienteActualizacion;

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
            case EstadosPatrulla.Patrullando:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    ElegirNuevoPuntoPatrulla();
                }

                if (distanciaAJugador < 5f)
                {
                    estadoEnemigo = EstadosPatrulla.Persiguiendo;
                    tiempoSiguienteActualizacion = 0f;
                }
                break;

            case EstadosPatrulla.Persiguiendo:
                if (Time.time >= tiempoSiguienteActualizacion)
                {
                    agent.SetDestination(jugador.position);
                    tiempoSiguienteActualizacion = Time.time + intervaloActualizacionRuta;
                }

                if (distanciaAJugador > 8f)
                {
                    ElegirNuevoPuntoPatrulla();
                    estadoEnemigo = EstadosPatrulla.Patrullando;
                }
                break;

            case EstadosPatrulla.Stuneado:
                if (Time.time >= tiempoFinStun)
                {
                    Debug.Log("STUN FINALIZADO");
                    agent.isStopped = false;
                    ElegirNuevoPuntoPatrulla();
                    estadoEnemigo = EstadosPatrulla.Patrullando;
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
            Destroy(this.gameObject);
            AplicarStun();
        }
    }

    public void AplicarStun()
    {
        estadoEnemigo = EstadosPatrulla.Stuneado;
        tiempoFinStun = Time.time + duracionStun;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    void ElegirNuevoPuntoPatrulla()
    {
        if (puntoPatrulla == null || puntoPatrulla.Length == 0) return;

        indicePuntos = Random.Range(0, puntoPatrulla.Length);
        agent.isStopped = false;
        agent.SetDestination(puntoPatrulla[indicePuntos].position);
    }
}