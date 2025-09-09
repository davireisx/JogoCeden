using System.Collections;
using UnityEngine;

public class JezabelController : MonoBehaviour
{
    public CenarioDoJezabel[] cenarios;
    public float velocidade = 3f;
    public float distanciaMinima = 0.2f;
    public AlunoSegundoAno player; // Script que sabe em qual cenário o Player está

    private int indiceCenario = 0;
    private int indiceWaypoint = 0;
    private bool estaParado = false;
    private bool aguardandoJogador = false;

    [System.Serializable]
    public class CenarioDoJezabel
    {
        public string nomeCenario;
        public int numeroCenario;             // Número do cenário correspondente
        public Transform[] waypoints;
        public Transform nextSpawnJezabel;    // Spawn onde só o Jezabel vai
        public bool[] pararNoWaypoint;        // Array de bool para indicar parada em cada waypoint
    }

    void Start()
    {
        IniciarCenario(indiceCenario);
    }

    void Update()
    {
        if (indiceCenario >= cenarios.Length) return;

        var cenarioAtual = cenarios[indiceCenario];

        // 1) Espera pelo jogador
        if (aguardandoJogador)
        {
            if (player != null && player.cenarioAtual == cenarioAtual.numeroCenario)
            {
                aguardandoJogador = false;
                Debug.Log("Player chegou, Jezabel vai continuar.");
            }
            else
            {
                return; // continua esperando
            }
        }

        // 2) Se está parado no waypoint, não faz nada
        if (estaParado) return;

        // 3) Movimenta entre waypoints
        if (indiceWaypoint < cenarioAtual.waypoints.Length)
        {
            Transform alvo = cenarioAtual.waypoints[indiceWaypoint];
            Vector3 direcao = (alvo.position - transform.position).normalized;
            transform.position += direcao * velocidade * Time.deltaTime;

            if (Vector3.Distance(transform.position, alvo.position) < distanciaMinima)
            {
                StartCoroutine(ChecarParada(cenarioAtual, indiceWaypoint));
                indiceWaypoint++;
            }
        }
        else
        {
            // 4) Chegou no último waypoint, teleporta se houver spawn
            if (cenarioAtual.nextSpawnJezabel != null)
            {
                transform.position = cenarioAtual.nextSpawnJezabel.position;
            }

            // 5) Prepara próximo cenário
            indiceCenario++;
            indiceWaypoint = 0;

            if (indiceCenario < cenarios.Length)
            {
                aguardandoJogador = true; // só continua quando o player chegar
                IniciarCenario(indiceCenario);
            }
        }
    }

    void IniciarCenario(int indice)
    {
        if (indice >= cenarios.Length) return;

        var cenario = cenarios[indice];
        indiceWaypoint = 0;
        Debug.Log("Jezabel iniciou cenário: " + cenario.nomeCenario);
    }

    IEnumerator ChecarParada(CenarioDoJezabel cenario, int index)
    {
        if (cenario.pararNoWaypoint.Length > index && cenario.pararNoWaypoint[index])
        {
            estaParado = true;
            Debug.Log("Jezabel parou no waypoint " + index);

            // tempo de parada no waypoint
            yield return new WaitForSeconds(3f);

            estaParado = false;
        }
    }

    // ?? Visualização dos waypoints no Editor
    private void OnDrawGizmos()
    {
        if (cenarios == null) return;

        Gizmos.color = Color.yellow;

        foreach (var cenario in cenarios)
        {
            if (cenario.waypoints == null) continue;

            for (int i = 0; i < cenario.waypoints.Length; i++)
            {
                if (cenario.waypoints[i] == null) continue;

                // desenha esfera no waypoint
                Gizmos.DrawSphere(cenario.waypoints[i].position, 0.2f);

                // liga os waypoints em sequência
                if (i < cenario.waypoints.Length - 1 && cenario.waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(cenario.waypoints[i].position, cenario.waypoints[i + 1].position);
                }
            }

            // desenha o próximo spawn do Jezabel
            if (cenario.nextSpawnJezabel != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(cenario.nextSpawnJezabel.position, Vector3.one * 0.3f);
                Gizmos.color = Color.yellow;
            }
        }
    }
}
