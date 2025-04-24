using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DisparoCodigo : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 20f;
    public Color corCorrompida; // Roxo escuro para corrupção
    public float tempoDestruicao = 5f; // Destrói automaticamente após 5 segundos

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private float tempoVida;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        tempoVida = 0f;
    }

    public void SetTarget(Vector2 target)
    {
        targetPosition = target;
        Vector2 direcao = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direcao * speed;
    }

    void Update()
    {
        tempoVida += Time.deltaTime;

        // Destrói se atingir o alvo ou se passar muito tempo
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f || tempoVida >= tempoDestruicao)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Catraca"))
        {
            Catraca catraca = collision.gameObject.GetComponent<Catraca>();
            if (catraca != null)
            {
                catraca.Corromper(); // Assume que a catraca tem um método para ser corrompida
            }
            Destroy(gameObject);
        }
    }
}