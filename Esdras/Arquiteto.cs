using UnityEngine;
using System.Collections.Generic;

public class Arquiteto : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float waypointThreshold = 0.1f;

    [Header("Configurações de Tiro")]
    public GameObject projetilPrefab;
    public Transform pontoDeTiro;
    public float tempoDeEspera = 3f;
    public Transform[] alvosPossiveis;

    [Header("Comportamento Final")]
    public bool pararNoUltimoWaypoint = true; // Ativar/desativar esta funcionalidade

    private bool podeAtirar = true; // Controle global de disparos

    private int currentWaypointIndex = 0;
    private bool isMovingForward = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Transform ultimoAlvoAtirado = null;
    private List<Transform> alvosDisponiveis = new List<Transform>();

    void Start()
    {
        ResetarAlvosDisponiveis();
    }

    void Update()
    {

        if (waypoints.Length == 0 || !podeAtirar) return;

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



    void ResetarAlvosDisponiveis()
    {
        alvosDisponiveis.Clear();

        // Adiciona todos os alvos possíveis, exceto o último atirado (se houver)
        foreach (Transform alvo in alvosPossiveis)
        {
            if (alvo != null && alvo != ultimoAlvoAtirado)
            {
                alvosDisponiveis.Add(alvo);
            }
        }

        // Se só tinha um alvo disponível, permite atirar nele novamente
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

        if (projetilPrefab == null || pontoDeTiro == null || alvosDisponiveis.Count == 0)
        {
            Debug.LogWarning("Configurações de tiro incompletas ou sem alvos disponíveis!");
            return;
        }

        // Seleciona aleatoriamente um alvo disponível
        int randomIndex = Random.Range(0, alvosDisponiveis.Count);
        Transform alvoAtual = alvosDisponiveis[randomIndex];
        ultimoAlvoAtirado = alvoAtual;
        alvosDisponiveis.RemoveAt(randomIndex);

        // Instancia e configura o projétil
        GameObject projetil = Instantiate(projetilPrefab, pontoDeTiro.position, Quaternion.identity);
        DisparoCodigo dc = projetil.GetComponent<DisparoCodigo>();
        if (dc == null)
        {
            dc = projetil.AddComponent<DisparoCodigo>();
        }
        dc.SetTarget(alvoAtual.position);

        // Se não houver mais alvos disponíveis, reseta para a próxima rodada
        if (alvosDisponiveis.Count == 0)
        {
            ResetarAlvosDisponiveis();
        }
    }

    void GetNextWaypoint()
    {
        if (isMovingForward)
        {
            currentWaypointIndex++;

            // Verifica se chegou no último waypoint
            if (currentWaypointIndex >= waypoints.Length )
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
    }
}