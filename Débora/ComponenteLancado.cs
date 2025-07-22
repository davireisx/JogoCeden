using UnityEngine;
using UnityEngine.InputSystem;

public class ComponenteLancado : MonoBehaviour
{
    public Transform[] caminho;
    public float velocidade = 5f;
    public float velocidadeRotacao = 360f;
    public float highlightRange = 3f;
    public float interactionRange = 1.5f;
    public Color corPiscar = Color.yellow;
    public float velocidadePiscada = 2f;
    private Transform playerTransform;
    public GameObject painelInteracao; // ? atribuir no Inspector
    private bool jogadorProximo = false;

    [Range(0f, 1f)] public float intensidadePiscada = 1f;

    public System.Action<ComponenteLancado> OnChegouNoFinal;

    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private bool chegouNoFinal = false;
    private bool devePiscar = false;
    private int indexAtual = 0;
    private Collider2D colisor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        corOriginal = spriteRenderer.color;
        colisor = GetComponent<Collider2D>();
        if (colisor != null) colisor.enabled = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (!chegouNoFinal && spriteRenderer.enabled)
        {
            MoverComponente();
        }
        else if (chegouNoFinal)
        {
            AtualizarPiscada();
            VerificarInteracao();
            VerificarCliqueDoUsuario(); // ? nova função adicionada
        }
    }

    void VerificarInteracao()
    {
        if (playerTransform == null) return;

        float distancia = Vector2.Distance(transform.position, playerTransform.position);
        jogadorProximo = distancia <= interactionRange;

        if (jogadorProximo && (Mouse.current.leftButton.wasPressedThisFrame || Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true))
        {
            if (painelInteracao != null && !painelInteracao.activeSelf)
            {
                painelInteracao.SetActive(true);
            }

            devePiscar = false;
            spriteRenderer.color = corOriginal;
        }
    }

    void VerificarCliqueDoUsuario()
    {
        if (playerTransform == null || colisor == null || !colisor.enabled) return; // ? novo: só processa se o colisor estiver ativo

        Vector2 toqueOuClique = Vector2.zero;
        bool clicou = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            toqueOuClique = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            clicou = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            toqueOuClique = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            clicou = true;
        }

        if (!clicou) return;

        RaycastHit2D hit = Physics2D.Raycast(toqueOuClique, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            float distancia = Vector2.Distance(transform.position, playerTransform.position);
            if (distancia <= interactionRange && painelInteracao != null)
            {
                painelInteracao.SetActive(true);
                Debug.Log("? Painel ativado via clique/toque — com colisor ativo.");
                devePiscar = false;
                spriteRenderer.color = corOriginal;
            }
        }
    }


    void MoverComponente()
    {
        if (caminho == null || caminho.Length == 0) return;

        Transform destino = caminho[indexAtual];
        transform.position = Vector3.MoveTowards(transform.position, destino.position, velocidade * Time.deltaTime);
        transform.Rotate(Vector3.forward * velocidadeRotacao * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino.position) < 0.1f)
        {
            indexAtual++;
            if (indexAtual >= caminho.Length)
            {
                chegouNoFinal = true;
                OnChegouNoFinal?.Invoke(this);
            }
        }
    }

    void AtualizarPiscada()
    {
        if (!devePiscar) return;

        float pulse = Mathf.PingPong(Time.time * velocidadePiscada, 1f) * intensidadePiscada;
        spriteRenderer.color = Color.Lerp(corOriginal, corPiscar, pulse);
    }

    public void AtivarPiscada()
    {
        devePiscar = true;

        // Ativa o colisor
        if (colisor != null)
            colisor.enabled = true;

        // Atualiza o range de interação
        interactionRange = 7f;
    }


    public void IniciarMovimento()
    {
        chegouNoFinal = false;
        indexAtual = 0;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, highlightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
