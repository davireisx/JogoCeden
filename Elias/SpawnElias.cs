using System.Collections.Generic;
using UnityEngine;

public class SpawnElias : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform gerador1; // Refer�ncia ao Gerador 1
    [SerializeField] Transform gerador2; // Refer�ncia ao Gerador 2

    private List<EnemyElias> inimigosAtivos = new List<EnemyElias>();
    private bool podeSpawnar = true;
    private bool gera2 = false;
    private Transform destinoAtual;

    void Start()
    {
        destinoAtual = gerador1; // Definir destino inicial
        podeSpawnar = false; // Spawn come�a desativado
        Debug.Log("Spawn desativado at� o bot�o 1 ser pressionado.");
    }

    void Update()
    {
        if (podeSpawnar && inimigosAtivos.Count < 20)
        {
            Debug.Log("Inimigos est�o sendo criados..."); // Adicione este para verificar
            SpawnEnemies();
            podeSpawnar = false;
            Invoke(nameof(ReativarSpawn), 1.5f); // Tempo entre spawns
        }

        if (podeSpawnar && gera2 && inimigosAtivos.Count < 40)
        {
            Debug.Log("Inimigos est�o sendo criados..."); // Adicione este para verificar
            SpawnEnemies();
            podeSpawnar = false;
            Invoke(nameof(ReativarSpawn), 1f); // Tempo entre spawns
        }


    }


    void SpawnEnemies()
    {
        Debug.Log("Tentando criar um inimigo..."); // Verificar se o m�todo � acionado

        if (inimigosAtivos.Count >= 40)
        {
            Debug.Log("Limite de inimigos atingido."); // Checar se o limite de inimigos � o problema
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        GameObject novoInimigo = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);

        EnemyElias inimigoScript = novoInimigo.GetComponent<EnemyElias>();
        if (inimigoScript != null)
        {
            Debug.Log("Novo inimigo criado com sucesso no spawn point " + index); // Confirmar cria��o
            inimigosAtivos.Add(inimigoScript);
            inimigoScript.SetTarget(destinoAtual); // Define o destino do inimigo
        }
        else
        {
            Debug.LogWarning("Prefab do inimigo est� sem o script EnemyElias."); // Checar se o prefab est� correto
        }
    }

    void RemoverInimigo(EnemyElias inimigo)
    {
        if (inimigo != null && inimigosAtivos.Contains(inimigo))
        {
            inimigosAtivos.Remove(inimigo);
            Debug.Log("Inimigo removido da lista. Total de inimigos ativos: " + inimigosAtivos.Count);
        }
    }

    public void AtivarSpawn()
    {
        podeSpawnar = true;
        Debug.Log("Spawn ativado! Pode spawnar inimigos."); // Confirme que o m�todo est� sendo chamado
    }

    void ReativarSpawn()
    {
        podeSpawnar = true;
    }

    public void SetDestinationToGerador1()
    {
        destinoAtual = gerador1;
        AtualizarDestinos();
    }

    public void SetDestinationToGerador2()
    {
        gera2 = true;
        destinoAtual = gerador2;
        Debug.Log("Destino dos inimigos atualizado para Gerador 2!");
        AtualizarDestinos();
    }

    void AtualizarDestinos()
    {
        foreach (EnemyElias inimigo in inimigosAtivos)
        {
            if (inimigo != null)
            {
                inimigo.SetTarget(destinoAtual);
            }
        }
    }
}
