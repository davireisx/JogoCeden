using UnityEngine;

public class Miriam : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 mov;
    [SerializeField] private Animator anim;

    // Update is called once per frame
    void Update()
    {
        mov.x = Input.GetAxisRaw("Horizontal");
        mov.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("Horizontal",mov.x);
        anim.SetFloat("Vertical", mov.y);
        anim.SetFloat("Speed", mov.sqrMagnitude);


        mov.Normalize();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + mov * speed * Time.fixedDeltaTime);
    }
}
