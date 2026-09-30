using System.Collections;
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
    [SerializeField] private float rangoAtaque = 6f;       // Distancia para iniciar el salto
    [SerializeField] private float alturaSalto = 3f;        // Altura máxima de la parábola
    [SerializeField] private float duracionSalto = 1.2f;    // Tiempo en segundos para completar el salto
    [SerializeField] private float tiempoEntreSaltos = 2f;  // Cooldown entre saltos
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
                // Patrulla normal
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    ElegirNuevoPuntoPatrulla();
                }

                // Si detecta al jugador y pasó el cooldown, inicia la secuencia de salto
                if (distanciaAJugador <= rangoAtaque && Time.time >= tiempoSiguienteSalto)
                {
                    StartCoroutine(RutinaSalto(jugador.position));
                }
                break;

            case EstadosEnemigo.Saltando:
                // El movimiento y la física del salto se gestionan dentro de la Corrutina (RutinaSalto)
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

    IEnumerator RutinaSalto(Vector3 posObjetivo)
    {
        estadoEnemigo = EstadosEnemigo.Saltando;

        // 1. Apagar el NavMeshAgent para permitir movimiento en 3D (eje Y)
        agent.enabled = false;

        Vector3 posInicio = transform.position;
        float tiempoTranscurrido = 0f;

        // Orientar al enemigo hacia la posición final antes de saltar
        Vector3 direccionLook = (posObjetivo - posInicio).normalized;
        direccionLook.y = 0;
        if (direccionLook != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccionLook);
        }

        // 2. Animar la parábola del salto
        while (tiempoTranscurrido < duracionSalto)
        {
            float t = tiempoTranscurrido / duracionSalto;

            // Interpolación lineal sobre el plano XZ
            Vector3 posActual = Vector3.Lerp(posInicio, posObjetivo, t);

            // Añadir curva parabólica en Y (Sinergia de la curva cuadrática: 4 * t * (1 - t))
            posActual.y += Mathf.Sin(t * Mathf.PI) * alturaSalto;

            transform.position = posActual;

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // 3. Garantizar que aterrice exactamente en el punto final
        transform.position = posObjetivo;

        // 4. Reactivar el NavMeshAgent re-enganchándolo al NavMesh más cercano
        agent.enabled = true;
        agent.Warp(transform.position);

        // Configurar cooldown de salto y regresar a patrullar
        tiempoSiguienteSalto = Time.time + tiempoEntreSaltos;
        ElegirNuevoPuntoPatrulla();
        estadoEnemigo = EstadosEnemigo.Patrullando;
    }

    public void AplicarStun()
    {
        // Cancelar saltos activos si entra en stun durante el aire
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