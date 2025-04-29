using UnityEngine;
using System.Collections.Generic;

public class Arquiteto : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float waypointThreshold = 0.1f;
    public GameObject posicaoInicialMovimento;

    [Header("Configurações de Tiro")]
    public GameObject projetilVerdePrefab;
    public GameObject projetilAmareloPrefab;
    public GameObject projetilVermelhoPrefab;
    public Transform pontoDeTiro;
    public float tempoDeEspera = 3f;
    public Transform[] alvosPossiveis;

    [Header("Comportamento Final")]
    public bool pararNoUltimoWaypoint = true;

    private bool podeAtirar = false;
    private int currentWaypointIndex = 0;
    private bool isMovingForward = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Transform ultimoAlvoAtirado = null;
    private List<Transform> alvosDisponiveis = new List<Transform>();

    private bool comandoRecebido = false;
    private bool indoParaPosicaoInicial = false;

    void Start()
    {
        ResetarAlvosDisponiveis();
    }

    void Update()
    {
        if (!comandoRecebido) return;

        if (indoParaPosicaoInicial)
        {
            MoverParaPosicaoInicial();
        }
        else if (podeAtirar)
        {
            if (waypoints.Length == 0) return;

            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= tempoDeEspera)
                {
                    isWaiting = false;
                    GetNextWaypoint();
                }
            }
            else
            {
                MoveToWaypoint();
            }
        }
    }

    public void IniciarMovimento()
    {
        if (!comandoRecebido)
        {
            comandoRecebido = true;
            indoParaPosicaoInicial = true;
        }
    }

    void MoverParaPosicaoInicial()
    {
        if (posicaoInicialMovimento == null)
        {
            Debug.LogWarning("Posição inicial de movimento não atribuída!");
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, posicaoInicialMovimento.transform.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, posicaoInicialMovimento.transform.position) < waypointThreshold)
        {
            indoParaPosicaoInicial = false;
            podeAtirar = true;
        }
    }

    void ResetarAlvosDisponiveis()
    {
        alvosDisponiveis.Clear();
        foreach (Transform alvo in alvosPossiveis)
        {
            if (alvo != null && alvo != ultimoAlvoAtirado)
            {
                alvosDisponiveis.Add(alvo);
            }
        }

        if (alvosDisponiveis.Count == 0 && alvosPossiveis.Length > 0)
        {
            alvosDisponiveis.AddRange(alvosPossiveis);
        }
    }

    void MoveToWaypoint()
    {
        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < waypointThreshold)
        {
            StartWaiting();
        }
    }

    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = 0f;
        AtirarParaAlvoAleatorio();
    }

    void AtirarParaAlvoAleatorio()
    {
        if (!podeAtirar) return;

        if (pontoDeTiro == null || alvosDisponiveis.Count == 0)
        {
            Debug.LogWarning("Configurações de tiro incompletas ou sem alvos disponíveis!");
            return;
        }

        int randomIndex = Random.Range(0, alvosDisponiveis.Count);
        Transform alvoAtual = alvosDisponiveis[randomIndex];
        ultimoAlvoAtirado = alvoAtual;
        alvosDisponiveis.RemoveAt(randomIndex);

        GameObject projetilPrefab = GetProjetilPrefabForAlvo(alvoAtual);

        if (projetilPrefab != null)
        {
            // Instancia o projétil sem rotação especial
            GameObject projetil = Instantiate(projetilPrefab, pontoDeTiro.position, Quaternion.identity);

            // Passa o alvo para o projétil
            DisparoCodigo dc = projetil.GetComponent<DisparoCodigo>();
            if (dc == null)
            {
                dc = projetil.AddComponent<DisparoCodigo>();
            }
            dc.SetTarget(alvoAtual.position);
        }

        if (alvosDisponiveis.Count == 0)
        {
            ResetarAlvosDisponiveis();
        }
    }

    GameObject GetProjetilPrefabForAlvo(Transform alvo)
    {
        int index = System.Array.IndexOf(alvosPossiveis, alvo);

        if (index == -1)
        {
            Debug.LogWarning("Alvo não encontrado em alvosPossiveis! Usando projétil padrão.");
            return projetilVerdePrefab;
        }

        if (index == 0 || index == 4) return projetilVerdePrefab;
        if (index == 1 || index == 3) return projetilAmareloPrefab;
        if (index == 2 || index == 5) return projetilVermelhoPrefab;

        return projetilVerdePrefab;
    }

    void GetNextWaypoint()
    {
        if (isMovingForward)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                isMovingForward = false;
                if (pararNoUltimoWaypoint)
                {
                    podeAtirar = false;
                    Debug.Log("Arquiteto chegou no ponto final - disparos desativados");
                }
            }
        }
        else
        {
            currentWaypointIndex--;
            if (currentWaypointIndex <= 0)
            {
                isMovingForward = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (posicaoInicialMovimento != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(posicaoInicialMovimento.transform.position, Vector3.one * 0.5f);
        }
    }
}
