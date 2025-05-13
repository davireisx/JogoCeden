using UnityEngine;

public class AlunosCatracas : MonoBehaviour
{
    [Header("Waypoints")]
    private Transform[] waypoints;
    private int waypointAtual = 0;

    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidade = 2f;
    [SerializeField] private float tempoEspera = 1f;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    private bool andando;

    [Header("Gerenciamento de Inimigos")]
    public static int inimigosNaCena = 0;
    public static int limiteInimigos = 10;

    private bool esperando = false;

    private void OnEnable()
    {
        inimigosNaCena++;
    }

    private void OnDisable()
    {
        inimigosNaCena--;
    }

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("Waypoints não foram atribuídos.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (esperando || waypoints == null || waypointAtual >= waypoints.Length) return;

        Vector2 direcao = (waypoints[waypointAtual].position - transform.position);
        transform.position += (Vector3)(direcao.normalized * velocidade * Time.deltaTime);

        if (direcao.magnitude < 0.1f)
        {
            transform.position = waypoints[waypointAtual].position;
            waypointAtual++;

            andando = false;
            animator?.SetBool("Andando", false);

            if (waypointAtual >= waypoints.Length)
            {
                AutoDestruir();
            }
            else
            {
                esperando = true;
                Invoke(nameof(ContinuarMovimento), tempoEspera);
            }
        }
        else
        {
            andando = true;
            animator?.SetBool("Andando", true);
        }
    }

    private void ContinuarMovimento()
    {
        esperando = false;
    }

    private void AutoDestruir()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Método público para definir os waypoints externamente (usado pelo spawner).
    /// </summary>
    /// <param name="novosWaypoints"></param>
    public void SetarWaypoints(Transform[] novosWaypoints)
    {
        waypoints = novosWaypoints;
        waypointAtual = 0;

        if (waypoints != null && waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }
}
