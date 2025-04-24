using UnityEngine;

public class bulleti : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] ParticleSystem effect;

    private void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed); // Movimenta na direção da rotação da arma
    }


private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Apenas destrói ao colidir com objetos de tag "Enemy"
        {
            Instantiate(effect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
