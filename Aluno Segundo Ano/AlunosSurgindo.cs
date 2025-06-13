using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlunoSurgindo : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform spawnTransform;
        public Transform[] waypoints;
        public bool emUso = false;
    }

    [Header("Configurações de Wave")]
    public SpawnPoint[] spawnPoints;
    public GameObject[] alunoPrefabs;
    public float tempoEntreWaves = 5f;
    public int alunosPorWave = 5;
    public float variacaoTamanhoWave = 2f;

    [Header("Configurações de Spawn")]
    public float intervaloEntreSpawns = 0.5f;
    public bool spawnAleatorio = true;

    private List<GameObject> alunosAtuais = new List<GameObject>();
    private bool waveEmProgresso = false;
    private int waveAtual = 0;
    private bool spawningAtivo = true;
    private Coroutine waveRoutine;

    // Adicionado: Lista estática para manter todos os alunos entre cenas
    private static List<GameObject> todosAlunos = new List<GameObject>();

    void Start()
    {
        ValidarConfiguracao();
    }

    public void IniciarSpawn()
    {
        if (waveRoutine == null)
        {
            spawningAtivo = true;
            waveRoutine = StartCoroutine(GerenciadorDeWaves());
        }
    }

    public void PararSpawn()
    {
        spawningAtivo = false;

        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        foreach (var spawn in spawnPoints)
        {
            spawn.emUso = false;
        }

        waveEmProgresso = false;
    }

    void OnDisable()
    {
        // Para de spawnar novos alunos
        spawningAtivo = false;

        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        // Libera os spawn points mas mantém os alunos existentes
        foreach (var spawn in spawnPoints)
        {
            spawn.emUso = false;
        }

        waveEmProgresso = false;
    }

    void OnEnable()
    {
        spawningAtivo = true;
        ValidarConfiguracao();

        if (waveRoutine == null)
            waveRoutine = StartCoroutine(GerenciadorDeWaves());
    }

    void ValidarConfiguracao()
    {
        if (spawnPoints.Length == 0 || alunoPrefabs.Length == 0)
        {
            Debug.LogError("Configure pelo menos um spawn point e um prefab de aluno!");
            return;
        }
    }

    IEnumerator GerenciadorDeWaves()
    {
        while (spawningAtivo)
        {
            waveEmProgresso = true;
            waveAtual++;

            Debug.Log($"Esperando {tempoEntreWaves} segundos antes da wave {waveAtual}...");
            yield return new WaitForSeconds(tempoEntreWaves);

            yield return StartCoroutine(SpawnWave());

            waveEmProgresso = false;

            yield return new WaitForSeconds(1f);
        }
    }


    IEnumerator SpawnWave()
    {
        int quantidadeAlunos = Mathf.RoundToInt(alunosPorWave + Random.Range(-variacaoTamanhoWave, variacaoTamanhoWave));
        quantidadeAlunos = Mathf.Clamp(quantidadeAlunos, 1, alunoPrefabs.Length); // Impede mais alunos do que tipos

        Debug.Log($"Iniciando Wave {waveAtual} com {quantidadeAlunos} alunos");

        List<int> spawnsDisponiveis = ObterSpawnsDisponiveis();
        quantidadeAlunos = Mathf.Min(quantidadeAlunos, spawnsDisponiveis.Count);

        // Cria uma lista de índices únicos dos prefabs embaralhada
        List<int> indicesUnicosPrefabs = new List<int>();
        for (int i = 0; i < alunoPrefabs.Length; i++) indicesUnicosPrefabs.Add(i);
        Shuffle(indicesUnicosPrefabs);

        for (int i = 0; i < quantidadeAlunos; i++)
        {
            if (spawnsDisponiveis.Count == 0) break;

            int indiceSpawn;
            if (spawnAleatorio)
            {
                int randomIndex = Random.Range(0, spawnsDisponiveis.Count);
                indiceSpawn = spawnsDisponiveis[randomIndex];
                spawnsDisponiveis.RemoveAt(randomIndex);
            }
            else
            {
                indiceSpawn = spawnsDisponiveis[0];
                spawnsDisponiveis.RemoveAt(0);
            }

            spawnPoints[indiceSpawn].emUso = true;

            // Usa o tipo único correspondente
            int prefabIndex = indicesUnicosPrefabs[i];
            SpawnAluno(indiceSpawn, prefabIndex);

            yield return new WaitForSeconds(intervaloEntreSpawns);
        }
    }


    List<int> ObterSpawnsDisponiveis()
    {
        List<int> disponiveis = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!spawnPoints[i].emUso)
            {
                disponiveis.Add(i);
            }
        }
        return disponiveis;
    }

    void SpawnAluno(int indiceSpawn, int prefabIndex)
    {
        GameObject alunoPrefab = alunoPrefabs[prefabIndex];

        GameObject aluno = Instantiate(
            alunoPrefab,
            spawnPoints[indiceSpawn].spawnTransform.position,
            spawnPoints[indiceSpawn].spawnTransform.rotation
        );

        //DontDestroyOnLoad(aluno);

        AlunoMovimento movimento = aluno.GetComponent<AlunoMovimento>();
        if (movimento == null)
        {
            movimento = aluno.AddComponent<AlunoMovimento>();
        }

        movimento.DefinirWaypoints(spawnPoints[indiceSpawn].waypoints);

        movimento.OnCaminhoCompleto += () => {
            spawnPoints[indiceSpawn].emUso = false;
            RemoverAluno(aluno);
        };

        alunosAtuais.Add(aluno);
        todosAlunos.Add(aluno);
        Debug.Log($"Aluno spawnado no ponto {indiceSpawn}. Total de alunos: {alunosAtuais.Count}");
    }


    void RemoverAluno(GameObject aluno)
    {
        if (alunosAtuais.Contains(aluno))
        {
            alunosAtuais.Remove(aluno);
            todosAlunos.Remove(aluno); // Remove da lista estática
            Destroy(aluno);
            Debug.Log($"Aluno removido. Total restante: {alunosAtuais.Count}");
        }
    }

    IEnumerator VerificarCompletudeWave()
    {
        while (alunosAtuais.Count > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        waveEmProgresso = false;
        Debug.Log($"Wave {waveAtual} completa! Preparando próxima wave em {tempoEntreWaves} segundos...");
        yield return new WaitForSeconds(tempoEntreWaves);
    }

    public Transform[] GetWaypointsParaSpawn(int spawnIndex)
    {
        if (spawnIndex >= 0 && spawnIndex < spawnPoints.Length)
        {
            return spawnPoints[spawnIndex].waypoints;
        }
        return null;
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null)
            return;

        Gizmos.color = Color.cyan;

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint == null || spawnPoint.waypoints == null || spawnPoint.waypoints.Length == 0)
                continue;

            if (spawnPoint.spawnTransform != null && spawnPoint.waypoints[0] != null)
            {
                Gizmos.DrawLine(spawnPoint.spawnTransform.position, spawnPoint.waypoints[0].position);
            }

            for (int i = 0; i < spawnPoint.waypoints.Length - 1; i++)
            {
                if (spawnPoint.waypoints[i] != null && spawnPoint.waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(spawnPoint.waypoints[i].position, spawnPoint.waypoints[i + 1].position);
                }
            }
        }
    }

    // Adicionado: Método para limpar todos os alunos quando necessário
    public static void LimparTodosAlunos()
    {
        foreach (var aluno in todosAlunos)
        {
            if (aluno != null)
            {
                Destroy(aluno);
            }
        }
        todosAlunos.Clear();
    }


    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

}