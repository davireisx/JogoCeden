using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering.LookDev;

public class Vagao : MonoBehaviour
{
    [Header("Referências")]
    public GameObject joystick;
    public GameObject[] destinos;
    public int indiceDestinoCorreto = 0;
    public float limiteXMin = -5f;
    public float limiteXMax = 5f;
    public float limiteYMin = -3f;
    public float limiteYMax = 3f;

    [Header("Check e objetivos")]
    public GameObject check;
    public GameObject objetivo1;
    public GameObject objetivo2;
    public GameObject objetivo3;
    public GameObject cercaDesativada;

    [Header("Transição final")]
    public GameObject hud;
    public GameObject personagem;
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1f;
    public float fadeStayTime = 5f;
    public string proximaCena; 


    [Header("Piscada e Interação")]
    public Transform player;
    public float highlightRange = 3f;
    public float interactionRange = 1.5f;
    public Color highlightColor = Color.black;
    public float blinkSpeed = 2f;
    [Range(0f, 1f)] public float blinkIntensity = 1f;

    private bool podeClicarParaFinal = false;
    private bool finalJaFoiExecutado = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Vector3 posicaoInicial;
    private Quaternion rotacaoOriginal;
    private Vector2 offset;
    private bool arrastando = false;
    private bool joystickFoiDesativado = false;
    private bool bloqueado = false;
    private Collider2D destinoCorreto;
    private Coroutine piscando;
    private Camera cam;

    private static Vagao[] todosVagoes;
    private static bool todosBrancosNotificados = false;
    private static bool todosEncaixadosNotificados = false;

    private static Coroutine piscadaGlobal;
    private static MonoBehaviour executorCorrotina;

    void Start()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        if (originalColor == Color.black)
            originalColor = Color.white;

        posicaoInicial = transform.position;

        foreach (var destino in destinos)
            destino.SetActive(false);

        if (destinos.Length > indiceDestinoCorreto)
            destinoCorreto = destinos[indiceDestinoCorreto].GetComponent<Collider2D>();

        if (todosVagoes == null || todosVagoes.Length == 0)
            todosVagoes = FindObjectsOfType<Vagao>();

        if (executorCorrotina == null)
            executorCorrotina = this;
    }

    void Update()
    {
        if (!todosBrancosNotificados && TodosVagoesBrancos())
        {
            todosBrancosNotificados = true;
            StartCoroutine(NotificarTodosBrancos());
        }

        if (!todosEncaixadosNotificados && TodosVagoesEncaixados())
        {
            todosEncaixadosNotificados = true;
            StartCoroutine(NotificarTodosEncaixados());
        }

        if (bloqueado)
        {
            BlinkHighlight();
            VerificarCliqueFinal();
            return;
        }

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 pointerPos = GetPointerWorldPosition();
            if (spriteRenderer.color == Color.white &&
                GetComponent<Collider2D>() == Physics2D.OverlapPoint(pointerPos) &&
                TodosVagoesBrancos())
            {
                offset = (Vector2)transform.position - pointerPos;
                arrastando = true;

                rotacaoOriginal = transform.rotation;
                transform.rotation = Quaternion.Euler(0, 0, 0);

                if (!joystickFoiDesativado)
                {
                    joystick.SetActive(false);
                    joystickFoiDesativado = true;
                }

                foreach (var destino in destinos)
                    destino.SetActive(true);

                if (piscando == null && destinoCorreto != null)
                    piscando = StartCoroutine(PiscarDestino(destinoCorreto.GetComponent<SpriteRenderer>()));
            }
        }

        if (arrastando)
        {
            Vector2 pointerPos = GetPointerWorldPosition();
            Vector2 novaPos = pointerPos + offset;

            novaPos = new Vector2(
                Mathf.Clamp(novaPos.x, limiteXMin, limiteXMax),
                Mathf.Clamp(novaPos.y, limiteYMin, limiteYMax)
            );

            transform.position = novaPos;

            if (Pointer.current != null && !Pointer.current.press.isPressed)
                StartCoroutine(SoltarComDelay());
        }
    }

    void BlinkHighlight()
    {
        if (!podeClicarParaFinal || finalJaFoiExecutado || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= highlightRange)
        {
            float pulse = Mathf.PingPong(Time.time * blinkSpeed, 1f) * blinkIntensity;
            spriteRenderer.color = Color.Lerp(originalColor, highlightColor, pulse);
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    void VerificarCliqueFinal()
    {
        if (!podeClicarParaFinal || finalJaFoiExecutado || Pointer.current == null) return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 pointerPos = GetPointerWorldPosition();
            if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(pointerPos))
            {
                Vector2 playerPos = player.position;

                // NOVA VERIFICAÇÃO DE ÁREA
                if (playerPos.x >= 119.82f && playerPos.x <= 162.33f &&
                    playerPos.y >= -30.12f && playerPos.y <= -20.83f)
                {
                    StartCoroutine(FinalizarVagao());
                    finalJaFoiExecutado = true;
                }
            }
        }
    }


    IEnumerator SoltarComDelay()
    {
        yield return new WaitForEndOfFrame();
        Soltar();
    }

    void Soltar()
    {
        arrastando = false;

        bool encostouNoDestinoCorreto = false;

        if (destinoCorreto != null)
        {
            Collider2D meuColisor = GetComponent<Collider2D>();
            if (meuColisor != null && destinoCorreto.bounds.Intersects(meuColisor.bounds))
                encostouNoDestinoCorreto = true;
        }

        if (encostouNoDestinoCorreto)
        {
            transform.position = destinoCorreto.transform.position;
            bloqueado = true;

            originalColor = Color.white;

        }

        else
        {
            transform.position = posicaoInicial;
            transform.rotation = rotacaoOriginal;
        }

        foreach (var destino in destinos)
            destino.SetActive(false);

        if (piscando != null)
        {
            StopCoroutine(piscando);
            destinoCorreto.GetComponent<SpriteRenderer>().color = Color.black;
            piscando = null;
        }

        joystick.SetActive(true);
        joystickFoiDesativado = false;
    }

    Vector2 GetPointerWorldPosition()
    {
        if (Pointer.current == null) return Vector2.zero;
        Vector2 screenPos = Pointer.current.position.ReadValue();
        return cam.ScreenToWorldPoint(screenPos);
    }

    IEnumerator PiscarDestino(SpriteRenderer sr)
    {
        while (true)
        {
            sr.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            sr.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    bool TodosVagoesBrancos()
    {
        foreach (Vagao v in todosVagoes)
        {
            if (v == null || v.spriteRenderer.color != Color.white)
                return false;
        }
        return true;
    }

    bool TodosVagoesEncaixados()
    {
        foreach (Vagao v in todosVagoes)
        {
            if (v == null || !v.bloqueado)
                return false;
        }
        return true;
    }

    IEnumerator NotificarTodosBrancos()
    {
        if (check != null) check.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        if (check != null) check.SetActive(false);
        if (objetivo1 != null) objetivo1.SetActive(false);
        if (objetivo2 != null) objetivo2.SetActive(true);
    }

    IEnumerator NotificarTodosEncaixados()
    {
        if (check != null) check.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (check != null) check.SetActive(false);
        if (objetivo2 != null) objetivo2.SetActive(false);
        if (objetivo3 != null) objetivo3.SetActive(true);
        if (cercaDesativada != null) cercaDesativada.SetActive(false);

        podeClicarParaFinal = true;

        foreach (var v in todosVagoes)
        {
            if (v != null)
            {
                v.bloqueado = true;

                // ?? REATIVA O COLLIDER AQUI
                var col = v.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;
            }
        }

        if (piscadaGlobal == null && executorCorrotina != null)
            piscadaGlobal = executorCorrotina.StartCoroutine(PiscarTodosVagoes());
    }

    private IEnumerator FinalizarVagao()
    {
        if (check != null) check.SetActive(true);
        if (personagem != null) personagem.SetActive(false);
        if (joystick != null) joystick.SetActive(false);
        if (hud != null) hud.SetActive(false);

        // Início do fade (igual ao outro código)
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = true;
            fadeGroup.interactable = false;

            float tempo = 0f;
            while (tempo < fadeDuration)
            {
                tempo += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(0f, 1f, tempo / fadeDuration);
                yield return null;
            }

            fadeGroup.alpha = 1f;

            yield return new WaitForSeconds(1);
        }

        // Pós fade
        if (check != null) check.SetActive(false);

        if (piscadaGlobal != null)
        {
            executorCorrotina.StopCoroutine(piscadaGlobal);
            piscadaGlobal = null;

            foreach (var v in todosVagoes)
            {
                if (v != null)
                {
                    v.spriteRenderer.color = Color.white;
                }
            }
        }

        if (joystick != null) joystick.SetActive(false);
        if (hud != null) hud.SetActive(false);
        SceneManager.LoadScene(proximaCena);
    }



    private static IEnumerator PiscarTodosVagoes()
    {
        float velocidade = 2f;
        Color corPiscar = Color.black;
        Color[] coresOriginais = new Color[todosVagoes.Length];

        for (int i = 0; i < todosVagoes.Length; i++)
        {
            if (todosVagoes[i] != null)
                coresOriginais[i] = todosVagoes[i].spriteRenderer.color;
        }

        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * velocidade, 1f);
            for (int i = 0; i < todosVagoes.Length; i++)
            {
                if (todosVagoes[i] != null)
                {
                    var sr = todosVagoes[i].spriteRenderer;
                    sr.color = Color.Lerp(coresOriginais[i], corPiscar, pulse);
                }
            }
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, highlightRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        Gizmos.color = Color.magenta;
        Vector3 centro = new Vector3((limiteXMin + limiteXMax) / 2, (limiteYMin + limiteYMax) / 2, 0);
        Vector3 tamanho = new Vector3(Mathf.Abs(limiteXMax - limiteXMin), Mathf.Abs(limiteYMax - limiteYMin), 0);
        Gizmos.DrawWireCube(centro, tamanho);

        Gizmos.color = Color.green;
        Vector3 centroInteracao = new Vector3((119.82f + 162.33f) / 2f, (-30.12f + -20.83f) / 2f, 0);
        Vector3 tamanhoInteracao = new Vector3(Mathf.Abs(162.33f - 129.82f), Mathf.Abs(-20.83f - -30.12f), 0);
        Gizmos.DrawWireCube(centroInteracao, tamanhoInteracao);

    }
}
