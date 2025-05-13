using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AlunoMovimento : MonoBehaviour
{
    public event Action OnCaminhoCompleto;

    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidade = 2f;
    [SerializeField] private float distanciaMinima = 0.1f;
    [SerializeField] private float tempoEsperaEntrePontos = 1f;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parametroAndando = "Andando";

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Transform[] waypoints;
    private int waypointAtual = 0;
    private bool estaAndando = false;
    private Coroutine movimentoCoroutine;

    private void Update()
    {
        // Garante movimento contínuo mesmo se o spawn estiver desativado
        if (estaAndando && waypointAtual < waypoints.Length && waypoints[waypointAtual] != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                waypoints[waypointAtual].position,
                velocidade * Time.deltaTime
            );
        }
    }
    

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null && debugLogs)
                Debug.LogWarning("Animator não encontrado no objeto", this);
        }
    }

    public void DefinirWaypoints(Transform[] novosWaypoints)
    {
        if (novosWaypoints == null || novosWaypoints.Length == 0)
        {
            Debug.LogError("Waypoints não definidos ou vazios!", this);
            return;
        }

        // Verifica se todos os waypoints são válidos
        foreach (var wp in novosWaypoints)
        {
            if (wp == null)
            {
                Debug.LogError("Um dos waypoints é nulo!", this);
                return;
            }
        }

        if (movimentoCoroutine != null)
        {
            StopCoroutine(movimentoCoroutine);
        }

        waypoints = novosWaypoints;
        waypointAtual = 0;
        estaAndando = true;
        AtualizarAnimacao();

        movimentoCoroutine = StartCoroutine(MoverEntreWaypoints());
    }

    private IEnumerator MoverEntreWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypoints não inicializados!", this);
            yield break;
        }

        if (debugLogs) Debug.Log($"Iniciando movimento por {waypoints.Length} waypoints", this);

        while (waypointAtual < waypoints.Length)
        {
            Transform waypointAlvo = waypoints[waypointAtual];

            if (waypointAlvo == null)
            {
                Debug.LogError($"Waypoint {waypointAtual} é nulo!", this);
                yield break;
            }

            if (debugLogs) Debug.Log($"Movendo para waypoint {waypointAtual}", this);

            while (Vector3.Distance(transform.position, waypointAlvo.position) > distanciaMinima)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    waypointAlvo.position,
                    velocidade * Time.deltaTime
                );

                yield return null;
            }

            waypointAtual++;

            if (waypointAtual < waypoints.Length)
            {
                if (debugLogs) Debug.Log($"Esperando no waypoint {waypointAtual - 1}", this);
                estaAndando = false;
                AtualizarAnimacao();
                yield return new WaitForSeconds(tempoEsperaEntrePontos);
                estaAndando = true;
                AtualizarAnimacao();
            }
        }

        if (debugLogs) Debug.Log("Caminho completo", this);
        estaAndando = false;
        AtualizarAnimacao();
        OnCaminhoCompleto?.Invoke();
    }

    private void AtualizarAnimacao()
    {
        if (animator != null && !string.IsNullOrEmpty(parametroAndando))
        {
            animator.SetBool(parametroAndando, estaAndando);
        }
    }

    private void OnDestroy()
    {
        if (debugLogs) Debug.Log("AlunoMovimento sendo destruído", this);
        if (movimentoCoroutine != null)
        {
            StopCoroutine(movimentoCoroutine);
        }
    }
}