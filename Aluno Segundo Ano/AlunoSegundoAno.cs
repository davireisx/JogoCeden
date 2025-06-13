using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AlunoSegundoAno : MonoBehaviour
{

    private Vector2 moveInput;
    private Vector2 ultimaDirecao = Vector2.down;
    private Vector2 direcaoAnterior = Vector2.zero;
    private bool isFacingLeft = false;
    private bool inputLocked = false;
    private float inputLockTimer = 0f;

    private Rigidbody2D rb;
    private int vida;
    private bool podeTomarDano = true;
    private bool dentroDoInimigo = false;
    private float tempoParaProximoDano = 0f;



    [Header("Movimenta��o do Jogador")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Joystick joystick;

    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMaxima = 3;
    [SerializeField] private Image[] coracoesHUD;
    [SerializeField] private Sprite coracaoCheio;
    [SerializeField] private Sprite coracaoVazio;
    [SerializeField] private float tempoInvencivel = 1f;
    [SerializeField] private float intervaloPiscar = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioDano;
    [SerializeField] private AudioSource audioVida;

    [Header("Componentes")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string telaGameOver;
    private Color corOriginal;

    [Header("Knockback")]
    [SerializeField] private float forcaKnockback = 8f;
    [SerializeField] private float duracaoKnockback = 0.5f;
    [SerializeField] private AnimationCurve curvaKnockback;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        corOriginal = spriteRenderer.color;

        if (joystick == null)
            Debug.LogError("Joystick n�o est� configurado!");

        vida = vidaMaxima;
        AtualizarHUDVida();
        InicializarCurvaKnockback();
    }

    private void Update()
    {
        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (moveInput.magnitude > 0.1f)
            direcaoAnterior = moveInput.normalized;

        VerificarContatoComInimigo();
        anim.SetBool("walking", moveInput.magnitude > 0.1f);
        FlipCharacter();
    }


    private void FixedUpdate()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        if (inputLocked)
        {
            inputLockTimer -= Time.deltaTime;
            if (inputLockTimer <= 0f) inputLocked = false;
            return;
        }

        if (moveInput.magnitude > 0.1f)
        {
            ultimaDirecao = moveInput.normalized;
            float suavizacao = Mathf.Clamp01(moveInput.magnitude);
            float inputModulado = Mathf.Pow(suavizacao, 1.5f); // curva exponencial
            Vector2 direcaoFinal = moveInput.normalized * inputModulado;
            rb.MovePosition(rb.position + direcaoFinal * moveSpeed * Time.fixedDeltaTime);

        }
    }


    private void FlipCharacter()
    {
        if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
            isFacingLeft = true;
        }
        else if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
            isFacingLeft = false;
        }
    }

    private void VerificarContatoComInimigo()
    {
        if (dentroDoInimigo && podeTomarDano)
        {
            tempoParaProximoDano -= Time.deltaTime;
            if (tempoParaProximoDano <= 0f)
            {
                TomarDano(1);
                tempoParaProximoDano = intervaloPiscar;
            }
        }
    }

    private void AtualizarHUDVida()
    {
        for (int i = 0; i < coracoesHUD.Length; i++)
        {
            coracoesHUD[i].sprite = i < vida ? coracaoCheio : coracaoVazio;
            coracoesHUD[i].enabled = i < vidaMaxima;
        }
    }

    public void TomarDano(int dano)
    {
        if (!podeTomarDano) return;

        vida = Mathf.Clamp(vida - dano, 0, vidaMaxima);
        audioDano.Play();
        AtualizarHUDVida();
        Debug.Log($"Vida: {vida}/{vidaMaxima}");

        if (vida <= 0)
            Morrer();
        else
            StartCoroutine(EfeitoDano());
    }

    private IEnumerator EfeitoDano()
    {
        podeTomarDano = false;
        float tempo = 0f;

        while (tempo < tempoInvencivel)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(intervaloPiscar);

            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(intervaloPiscar);

            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(intervaloPiscar);

            spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(intervaloPiscar);

            tempo += intervaloPiscar * 4;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.color = corOriginal;
        podeTomarDano = true;
    }

    private void Morrer()
    {
        Debug.Log("Jogador morreu!");
        // Troca de cena ap�s um pequeno atraso (recomendado)
        StartCoroutine(MudarCenaAposMorte());
    }

    private IEnumerator MudarCenaAposMorte()
    {
        yield return new WaitForSeconds(0.1f); // Tempo para mostrar anima��o ou som de morte
        SceneManager.LoadScene(telaGameOver); // Troque "GameOver" pelo nome exato da sua cena
    }

    public void RecuperarVida(int quantidade)
    {
        vida = Mathf.Clamp(vida + quantidade, 0, vidaMaxima);
        audioVida.Play();
        AtualizarHUDVida();
    }

    private IEnumerator AplicarKnockback(Vector2 direcao)
    {
        LockInput(duracaoKnockback);

        float timer = 0f;
        while (timer < duracaoKnockback)
        {
            float intensidade = curvaKnockback.Evaluate(timer / duracaoKnockback);
            transform.Translate(direcao * forcaKnockback * intensidade * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void LockInput(float duration)
    {
        inputLocked = true;
        inputLockTimer = duration;
    }

    private void InicializarCurvaKnockback()
    {
        if (curvaKnockback == null || curvaKnockback.length == 0)
        {
            curvaKnockback = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.2f, 1f, 0f, 2f),
                new Keyframe(1f, 0f)
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            dentroDoInimigo = true;
            tempoParaProximoDano = 0f;

            Vector2 direcao = direcaoAnterior == Vector2.zero ? -ultimaDirecao : -direcaoAnterior.normalized;
            StartCoroutine(AplicarKnockback(direcao));
        }

        // Recuperar vida ao encostar em "Vida"
        if (other.CompareTag("Vida"))
        {
            if (vida < vidaMaxima)
            {
                RecuperarVida(1);
                Destroy(other.gameObject); // remove o item de vida da cena
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            dentroDoInimigo = false;
    }

    // Acessores P�blicos
    public bool IsFacingLeft() => isFacingLeft;
    public bool IsMoving() => moveInput.magnitude > 0.1f;
    public void SetInput(Vector2 input) => moveInput = input;
    public Vector2 GetCurrentInput() => moveInput;

    public void TeleportarPara(Vector2 novaPosicao)
    {
        transform.position = novaPosicao;
    }

}
