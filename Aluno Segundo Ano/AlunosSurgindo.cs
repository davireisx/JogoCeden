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

    // Modifique a classe GerenciadorDeCenario para incluir um tempo de última mudança
    public static class GerenciadorDeCenario
    {
        public static int idCenarioAtualDoJogador = -1;
        public static float ultimaMudancaCenario = -Mathf.Infinity;
        public static float cooldownMudancaCenario = 0.5f; // Tempo mínimo entre mudanças
    }

    [Header("Controle de Cenário")]
    [SerializeField] private int idCenario;

    [Header("Cooldown de cenário")]
    private static Dictionary<int, float> cooldownPorCenario = new Dictionary<int, float>();
    [SerializeField] private float cooldownGlobalPorCenario = 5f;

    [Header("Configurações de Wave")]
    public SpawnPoint[] spawnPoints;
    public GameObject[] alunoPrefabs;
    public float tempoEntreWaves = 5f;
    public int alunosPorWave = 5;
    public float variacaoTamanhoWave = 2f;

    [Header("Configurações de Spawn")]
    public float intervaloEntreSpawns = 0.5f;
    public bool spawnAleatorio = true;

    [Header("Espaçamento entre alunos")]
    public float distanciaEntreAlunos = 0.5f;

    private List<GameObject> alunosAtuais = new List<GameObject>();
    private bool waveEmProgresso = false;
    private int waveAtual = 0;
    private bool spawningAtivo = true;
    private float ultimaChamadaSpawn = -Mathf.Infinity;
    private Coroutine waveRoutine;

    private float tempoParaProximoDano = 0f;
    private float tempoUltimoSpawn = -Mathf.Infinity;
    [SerializeField] private float cooldownEntreSpawns = 2f;

    private static List<GameObject> todosAlunos = new List<GameObject>();

    void Start()
    {
        ValidarConfiguracao();
    }

    public void IniciarSpawn()
    {
        // Verificar se este é realmente o cenário atual
        if (GerenciadorDeCenario.idCenarioAtualDoJogador != idCenario)
        {
            Debug.Log($"Tentativa de spawn no cenário {idCenario} falhou - cenário atual é {GerenciadorDeCenario.idCenarioAtualDoJogador}");
            return;
        }

        // Verificar cooldown global
        if (cooldownPorCenario.TryGetValue(idCenario, out float ultimoTempo))
        {
            float tempoDecorrido = Time.time - ultimoTempo;
            if (tempoDecorrido < cooldownGlobalPorCenario)
            {
                Debug.Log($"Cooldown global do cenário {idCenario} ativo: {tempoDecorrido:F2}/{cooldownGlobalPorCenario} segundos");
                return;
            }
        }

        // Verificar cooldown entre spawns
        if (Time.time - tempoUltimoSpawn < cooldownEntreSpawns)
        {
            Debug.Log("Aguardando cooldown entre spawns...");
            return;
        }

        // Se já está spawnando, não iniciar novamente
        if (waveRoutine != null)
        {
            Debug.Log("Já existe uma wave em progresso");
            return;
        }

        // Registrar tempo e iniciar
        cooldownPorCenario[idCenario] = Time.time;
        tempoUltimoSpawn = Time.time;
        spawningAtivo = true;
        waveRoutine = StartCoroutine(GerenciadorDeWaves());
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

    void OnEnable()
    {
        spawningAtivo = true;
        ValidarConfiguracao();

        if (Time.time - tempoUltimoSpawn >= cooldownEntreSpawns && waveRoutine == null)
        {
            tempoUltimoSpawn = Time.time;
            waveRoutine = StartCoroutine(GerenciadorDeWaves());
        }
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
        quantidadeAlunos = Mathf.Clamp(quantidadeAlunos, 1, alunoPrefabs.Length);

        Debug.Log($"Iniciando Wave {waveAtual} com {quantidadeAlunos} alunos");

        List<int> spawnsDisponiveis = ObterSpawnsDisponiveis();
        quantidadeAlunos = Mathf.Min(quantidadeAlunos, spawnsDisponiveis.Count);

        List<int> indicesUnicosPrefabs = new List<int>();
        for (int i = 0; i < alunoPrefabs.Length; i++) indicesUnicosPrefabs.Add(i);
        Shuffle(indicesUnicosPrefabs);

        for (int i = 0; i < quantidadeAlunos; i++)
        {
            if (spawnsDisponiveis.Count == 0) break;

            int indiceSpawn = spawnAleatorio ?
                spawnsDisponiveis[Random.Range(0, spawnsDisponiveis.Count)] :
                spawnsDisponiveis[0];

            spawnsDisponiveis.Remove(indiceSpawn);
            spawnPoints[indiceSpawn].emUso = true;

            int prefabIndex = indicesUnicosPrefabs[i];
            Vector2 offset = Random.insideUnitCircle.normalized * distanciaEntreAlunos;
            SpawnAluno(indiceSpawn, prefabIndex, offset);

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

    void SpawnAluno(int indiceSpawn, int prefabIndex, Vector2 offset)
    {
        GameObject alunoPrefab = alunoPrefabs[prefabIndex];

        GameObject aluno = Instantiate(
            alunoPrefab,
            (Vector2)spawnPoints[indiceSpawn].spawnTransform.position + offset,
            spawnPoints[indiceSpawn].spawnTransform.rotation);

        AlunoMovimento movimento = aluno.GetComponent<AlunoMovimento>();
        if (movimento == null)
            movimento = aluno.AddComponent<AlunoMovimento>();

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
            todosAlunos.Remove(aluno);
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
            return spawnPoints[spawnIndex].waypoints;

        return null;
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.cyan;

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint == null || spawnPoint.waypoints == null || spawnPoint.waypoints.Length == 0)
                continue;

            if (spawnPoint.spawnTransform != null && spawnPoint.waypoints[0] != null)
                Gizmos.DrawLine(spawnPoint.spawnTransform.position, spawnPoint.waypoints[0].position);

            for (int i = 0; i < spawnPoint.waypoints.Length - 1; i++)
            {
                if (spawnPoint.waypoints[i] != null && spawnPoint.waypoints[i + 1] != null)
                    Gizmos.DrawLine(spawnPoint.waypoints[i].position, spawnPoint.waypoints[i + 1].position);
            }
        }
    }

    public static void LimparTodosAlunos()
    {
        foreach (var aluno in todosAlunos)
        {
            if (aluno != null)
                Destroy(aluno);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Verificar cooldown de mudança de cenário
            if (Time.time - GerenciadorDeCenario.ultimaMudancaCenario < GerenciadorDeCenario.cooldownMudancaCenario)
            {
                Debug.Log($"Mudança de cenário muito rápida. Ignorando entrada no cenário {idCenario}");
                return;
            }

            // Se já está neste cenário, não fazer nada
            if (GerenciadorDeCenario.idCenarioAtualDoJogador == idCenario)
                return;

            // Parar o cenário anterior
            if (GerenciadorDeCenario.idCenarioAtualDoJogador != -1)
            {
                // Aqui você precisaria ter uma referência ao script do cenário anterior
                // Ou implementar um sistema de notificação para cenários
                Debug.Log($"Saindo do cenário {GerenciadorDeCenario.idCenarioAtualDoJogador} para entrar no {idCenario}");
            }

            // Atualizar cenário atual
            GerenciadorDeCenario.idCenarioAtualDoJogador = idCenario;
            GerenciadorDeCenario.ultimaMudancaCenario = Time.time;

            // Iniciar spawn
            IniciarSpawn();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Se o jogador sair deste cenário e ele era o cenário atual
            if (GerenciadorDeCenario.idCenarioAtualDoJogador == idCenario)
            {
                // Não definir imediatamente como -1, pois pode estar entrando em outro
                // Em vez disso, adicione uma verificação atrasada
                StartCoroutine(VerificarSaidaCenario());
            }
        }
    }

    IEnumerator VerificarSaidaCenario()
    {
        yield return new WaitForSeconds(0.1f); // Pequeno delay para permitir entrada em outro cenário

        // Se após o delay o jogador não entrou em outro cenário
        if (GerenciadorDeCenario.idCenarioAtualDoJogador == idCenario)
        {
            GerenciadorDeCenario.idCenarioAtualDoJogador = -1;
            PararSpawn();
        }
    }


}
