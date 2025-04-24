using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private float speed;
    private GameObject player;
    private Animator anim;
    private bool isAlive = true;
    private Collider2D enemyCollider;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>(); // Obtém o componente Collider2D do inimigo
    }

    void Update()
    {
        if (player != null && isAlive)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && isAlive)
        {
            anim.SetTrigger("Dead");
            isAlive = false;
            enemyCollider.enabled = false; // Desativa o colisor para permitir que outras balas atravessem
            Destroy(gameObject, 0.2f);
            audioSource.Play();
        }
    }

}