using UnityEngine;

public class BalaEffect : MonoBehaviour
{
    [Header("Configuração da Bala")]
    [SerializeField] private float speed = 10f; // Velocidade do projetil
    [SerializeField] private AudioSource audiosource; // Áudio do impacto
    [SerializeField] private int carga = 1; // Carga que esta bala gera ao colidir com o roteador

    private Animator animator;
    private bool isHit = false; // Flag para controlar se a bala foi atingida

    private void Start()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();

        // Define que a bala será destruída após 2 segundos, caso não colida antes
        Invoke("AutoDestroy", 2f);
    }

    private void Update()
    {
        if (!isHit) // Apenas move se a bala não foi atingida
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit) return; // Se já foi atingido, não executa mais

        // Verifica se a bala do jogador está colidindo com a bala do Quero-Quero
        if (collision.CompareTag("Bullet"))
        {
            Debug.Log("Bala do jogador colidiu com o raio do Quero-Quero. Nenhuma animação será ativada.");
            Destroy(gameObject); // Apenas destrói a bala do jogador
            return;
        }

        isHit = true; // Define que a bala foi atingida
        speed = 0f; // Interrompe o movimento da bala

        // Toca a animação de hit, exceto em colisão com o raio do Quero-Quero
        animator.SetTrigger("Hit");

        // Toca o som do impacto, se configurado
        if (audiosource != null && !audiosource.isPlaying)
        {
            audiosource.Play();
        }

        // Aguarda a animação antes de destruir a bala
        float animationLength = (animator != null) ? animator.GetCurrentAnimatorStateInfo(0).length : 0.5f;
        Destroy(gameObject, animationLength > 0 ? animationLength : 0.5f);
    }

    private void AutoDestroy()
    {
        // Se a bala ainda não foi destruída pela colisão, destrua-a automaticamente
        if (!isHit)
        {
            isHit = true;
            Destroy(gameObject);
            if (audiosource != null && !audiosource.isPlaying)
            {
                audiosource.Play();
            }
        }
    }

    // Retorna a carga associada a esta bala
    public int GetCarga()
    {
        return carga;
    }
}
