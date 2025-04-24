using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
       Destroy (gameObject);
    }

}