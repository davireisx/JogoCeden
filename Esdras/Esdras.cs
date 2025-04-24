using UnityEngine;

public class Esdras : MonoBehaviour
{
    [Header("Movimentação do Jogador")]
    [SerializeField] private float moveSpeed; // Velocidade de movimento do jogador
    private Vector2 moveInput; // Input de movimento do jogador

    [Header("Joystick")]
    [SerializeField] private Joystick joystick; // Referência ao joystick no Canvas

    [Header("Componentes do Jogador")]
    [SerializeField] private Animator anim; // Componente Animator para animações
    [SerializeField] private SpriteRenderer spriteRenderer; // Componente SpriteRenderer para o sprite

    private bool isFacingLeft = false; // Controle para indicar se o jogador está virado para a esquerda

    private void Start()
    {
        // Inicializa os componentes necessários
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (joystick == null)
        {
            Debug.LogError("Joystick não está configurado! Arraste o joystick para o campo no Inspector.");
        }
    }

    private void Update()
    {
        HandleMovement(); // Gerencia o movimento do jogador
    }

    private void HandleMovement()
    {
        if (joystick != null)
        {
            // Obtém o input do joystick
            moveInput.x = joystick.Horizontal;
            moveInput.y = joystick.Vertical;

            // Move o jogador com base na velocidade e no input
            transform.Translate(moveInput * Time.deltaTime * moveSpeed);

            // Atualiza o parâmetro de animação para indicar movimento
            anim.SetBool("walking", Mathf.Abs(moveInput.x) > 0 || Mathf.Abs(moveInput.y) > 0);

            // Atualiza o lado para o qual o jogador está virado
            FlipCharacter();
        }
        else
        {
            Debug.LogWarning("Joystick não configurado. O jogador não irá se mover.");
        }
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