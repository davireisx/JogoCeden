using UnityEngine;
using UnityEngine.InputSystem;

public class ComponenteLancado : MonoBehaviour
{
    public Transform[] caminho;
    public float velocidade = 5f;
    public float velocidadeRotacao = 360f;

    [Header("Referências de interação")]
    public Transform player;
    public GameObject imagemPainel; // Imagem a ser exibida ao clicar

    [Header("Ranges de interação")]
    public float highlightRange = 3f;
    public float interactionRange = 1.5f;

    [Header("Piscada")]
    public Color corPiscar = Color.yellow;
    public float velocidadePiscada = 2f;
    [Range(0f, 1f)]
    public float intensidadePiscada = 1f;

    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private bool chegouNoFinal = false;
    private bool devePiscar = false;

    private int indexAtual = 0;
    private GerenciadorDeComponentesLancados gerenciador;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        corOriginal = spriteRenderer.color;
        gerenciador = FindObjectOfType<GerenciadorDeComponentesLancados>();
        if (imagemPainel != null) imagemPainel.SetActive(false);
    }

    void Update()
    {
        if (!chegouNoFinal)
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
                    gerenciador?.NotificarChegada(this);
                }
            }
        }
        else
        {
            if (player == null || spriteRenderer == null) return;

            float distancia = Vector2.Distance(transform.position, player.position);

            // Piscada se dentro do range
            if (devePiscar && distancia <= highlightRange)
            {
                float pulse = Mathf.PingPong(Time.time * velocidadePiscada, 1f) * intensidadePiscada;
                spriteRenderer.color = Color.Lerp(corOriginal, corPiscar, pulse);
            }
            else
            {
                spriteRenderer.color = corOriginal;
            }

            // Clique via touchscreen ou mouse
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 toque = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
                VerificarClique(toque);
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 click = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                VerificarClique(click);
            }
        }
    }

    public void AtivarPiscada()
    {
        devePiscar = true;
    }

    private void VerificarClique(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            float distancia = Vector2.Distance(transform.position, player.position);
            if (distancia <= interactionRange && imagemPainel != null)
            {
                imagemPainel.SetActive(true);
                Debug.Log("?? Painel de imagem aberto após interação com componente.");
            }
        }
    }

    // (opcional) Mostrar o range no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, highlightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
