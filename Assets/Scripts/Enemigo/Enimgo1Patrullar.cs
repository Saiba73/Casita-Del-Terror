using UnityEngine;
using UnityEngine.AI;

public class Enimgo1Patrullar : MonoBehaviour
{
    enum estadosPatrulla
    {
        Patrullando,
        Persiguiendo,
        Volviendo,
        Stuneado
    }

    estadosPatrulla estadoEnemigo = estadosPatrulla.Patrullando;

    [Header("Puntos de patrullaje")]
    [SerializeField] private Transform[] puntoPatrulla;

    [Header("Referencia de jugador")]
    [SerializeField] private Transform jugador;

    private int indicePuntos;

    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        indicePuntos = Random.Range(0, puntoPatrulla.Length);
        agent.SetDestination(puntoPatrulla[indicePuntos].position);
    }

    
    void Update()
    {
        switch(estadoEnemigo)
        {
            case estadosPatrulla.Patrullando:
                    if (Vector3.Distance(this.transform.position, puntoPatrulla[indicePuntos].position) < 0.1f)
                    {
                        indicePuntos = Random.Range(0, puntoPatrulla.Length);
                        agent.SetDestination(puntoPatrulla[indicePuntos].position);
                        Debug.Log(indicePuntos);
                    }
                    if(Vector3.Distance(this.transform.position, jugador.position) < 1f)
                    {
                        agent.SetDestination(jugador.position);
                        estadoEnemigo = estadosPatrulla.Persiguiendo;
                    }
                break;
            case estadosPatrulla.Persiguiendo:
                    agent.SetDestination(jugador.position);
                    if(Vector3.Distance(this.transform.position, jugador.position) > 5f)
                    {
                        indicePuntos = Random.Range(0, puntoPatrulla.Length);
                        agent.SetDestination(puntoPatrulla[indicePuntos].position);
                        Debug.Log(indicePuntos);
                        estadoEnemigo = estadosPatrulla.Patrullando;
                    }
                break;
            case estadosPatrulla.Stuneado:
                
                break;
            case estadosPatrulla.Volviendo:

                break;
        }
    }
}
