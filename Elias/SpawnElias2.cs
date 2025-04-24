using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Para usar listas

public class SpawnElias2 : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints; // Pontos de spawn dos inimigos
    [SerializeField] GameObject enemyPrefab; // Prefab do inimigo

    private List<EnemyElias> inimigosAtivos = new List<EnemyElias>();
    private bool podeSpawnar = false; // Flag para controlar se o spawn est� ativo
    [SerializeField] private int maxInimigos = 10; // Limite m�ximo de inimigos
    [SerializeField] private float tempoEntreSpawns = 2.5f; // Intervalo entre spawns

    void Update()
    {
        // O spawn s� � executado se estiver ativado e o limite de inimigos n�o for excedido
        if (podeSpawnar && inimigosAtivos.Count < maxInimigos)
        {
            StartCoroutine(SpawnEnemyWithDelay()); // Gera inimigos de forma controlada
        }
    }

    private IEnumerator SpawnEnemyWithDelay()
    {
        podeSpawnar = false; // Evita spawns consecutivos
        SpawnEnemies();
        yield return new WaitForSeconds(tempoEntreSpawns);
        podeSpawnar = true; // Reativa o spawn ap�s o intervalo
    }

    void SpawnEnemies()
    {
        if (inimigosAtivos.Count >= maxInimigos)
        {
            Debug.Log("Limite de inimigos atingido. Nenhum novo inimigo ser� gerado.");
            return; // N�o spawnar mais inimigos se atingir o limite
        }

        int index = Random.Range(0, spawnPoints.Length);
        GameObject novoInimigo = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);

        // Adiciona o inimigo � lista
        EnemyElias inimigoScript = novoInimigo.GetComponent<EnemyElias>();
        if (inimigoScript != null)
        {
            inimigosAtivos.Add(inimigoScript);
            inimigoScript.StartMovingAfterDelay(0); // Faz o inimigo come�ar a se mover imediatamente
        }
    }

    public void AtivarSpawn()
    {
        podeSpawnar = true; // Ativa o spawn de inimigos
        Debug.Log("Spawn ativado!");
    }

    public void MoverTodosInimigos(float delay)
    {
        StartCoroutine(MoverInimigosComDelay(delay));
    }

    private IEnumerator MoverInimigosComDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (EnemyElias inimigo in inimigosAtivos)
        {
            if (inimigo != null)
            {
                inimigo.StartMovingAfterDelay(0); // Faz os inimigos come�arem a se mover
            }
        }
    }
}
