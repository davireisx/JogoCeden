using UnityEngine;
using System.Collections;

public class EnemyElias : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private AudioSource audioSource;
    private Transform currentGerador; // Gerador dinâmico
    private Animator anim;
    private bool isAlive = true;
    private bool shouldMove = false;
    private Collider2D enemyCollider;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (currentGerador != null && isAlive && shouldMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentGerador.position, speed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform gerador)
    {
        currentGerador = gerador; // Define o gerador como destino
        shouldMove = true; // Permite o movimento
    }

    public void StartMovingAfterDelay(float delay)
    {
        StartCoroutine(DelayMovement(delay));
    }

    private IEnumerator DelayMovement(float delay)
    {
        yield return new WaitForSeconds(delay);
        shouldMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && isAlive)
        {
            anim.SetTrigger("Dead");
            isAlive = false;
            enemyCollider.enabled = false;
            Destroy(gameObject, 0.2f);
            audioSource.Play();
        }
    }
}
