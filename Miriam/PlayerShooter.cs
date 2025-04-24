using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Movimenta��o do Jogador")]
    [SerializeField] private float moveSpeed; // Velocidade de movimento do jogador
    private Vector2 moveInput; // Input de movimento do jogador

    [Header("Componentes do Jogador")]
    private Animator anim; // Componente Animator para anima��es
    private SpriteRenderer spriteRenderer; // Componente SpriteRenderer para o sprite

    [Header("Configura��o da Arma")]
    [SerializeField] private GameObject weapon; // Refer�ncia ao GameObject da arma

    private bool isFacingLeft = false; // Controle para indicar se o jogador est� virado para a esquerda

    private void Start()
    {
        // Inicializa os componentes necess�rios
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleMovement(); // Gerencia o movimento do jogador
    }

    private void HandleMovement()
    {
        // Obt�m o input do jogador
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        // Move o jogador com base na velocidade e no input
        transform.Translate(moveInput * Time.deltaTime * moveSpeed);

        // Atualiza o par�metro de anima��o para indicar movimento
        anim.SetBool("isMoving", Mathf.Abs(moveInput.x) > 0 || Mathf.Abs(moveInput.y) > 0);

        // Atualiza o lado para o qual o jogador est� virado
        FlipCharacter();
    }

    private void FlipCharacter()
    {
        if (moveInput.x < 0)
        {
            // Virar para a esquerda
            spriteRenderer.flipX = true;
            isFacingLeft = true;
        }
        else if (moveInput.x > 0)
        {
            // Virar para a direita
            spriteRenderer.flipX = false;
            isFacingLeft = false;
        }
    }


    public bool IsFacingLeft()
    {
        // Retorna true se o jogador estiver virado para a esquerda
        return isFacingLeft;
    }

}