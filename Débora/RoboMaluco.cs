using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class RoboMaluco : MonoBehaviour
{
    [Header("Interação")]
    public Transform player;
    public GameObject playerObject;

    [Tooltip("Distância para começar a piscar")]
    public float highlightRange = 3f;

    [Tooltip("Distância permitida para clicar")]
    public float interactionRange = 1.5f;

    [Header("Piscada")]
    public Color highlightColor = Color.red;

    [Tooltip("Velocidade da piscada (quanto maior, mais rápido)")]
    public float blinkSpeed = 2f;

    [Tooltip("Intensidade da piscada (0 a 1)")]
    [Range(0f, 1f)]
    public float blinkIntensity = 1f;

    [Tooltip("Ativar/desativar piscada quando nocauteado")]
    public bool enableBlink = true;

    public Transform spawnPoint;

    [Header("UI e Controles")]
    public GameObject HUD;
    public GameObject joystick;
    public CameraManagerEsdras cameraManager;

    private Animator animator;
    private string tagOriginal;
    private bool estaNocauteado = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        animator = GetComponent<Animator>();
        tagOriginal = gameObject.tag;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (player == null) Debug.LogError("Transform do player não atribuído!");
        if (playerObject == null) Debug.LogError("GameObject do player não atribuído!");

        // Oculta HUD e joystick no início
        if (HUD != null) HUD.SetActive(false);
        if (joystick != null) joystick.SetActive(false);

        StartCoroutine(AtacarInicialmente()); // inicia ataque por 5 segundos
    }




    void Update()
    {
        if (!estaNocauteado) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Destacar visualmente com piscada ajustável
        if (distance <= highlightRange && enableBlink)
        {
            float pulse = Mathf.PingPong(Time.time * blinkSpeed, 1f) * blinkIntensity;
            spriteRenderer.color = Color.Lerp(originalColor, highlightColor, pulse);
        }
        else
        {
            spriteRenderer.color = originalColor;
        }

        // Clique via touchscreen ou mouse
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            VerificarClique(touchPos);
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            VerificarClique(clickPos);
        }
    }

    void VerificarClique(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= interactionRange)
            {
                RealizarInteracao();
            }
        }
    }

    void RealizarInteracao()
    {
        Debug.Log("? Jogador interagiu com o robô nocauteado!");

        if (spawnPoint != null && playerObject != null)
        {
            playerObject.transform.position = spawnPoint.position;
            playerObject.SetActive(false);
            HUD.SetActive(false);
            joystick.SetActive(false);
        }

        if (cameraManager != null)
        {
            cameraManager.SetScenarioBounds(2);
        }
        else
        {
            Debug.LogWarning("?? CameraManagerEsdras não atribuído no RoboMaluco!");
        }
    }

    public void LevarNocaute()
    {
        Debug.Log("?? Robo levou nocaute!");
        animator.SetTrigger("Nocaute");
        gameObject.tag = "Untagged";
        estaNocauteado = true;
    }

    public void VoltarParaIdle()
    {
        Debug.Log("?? Robo voltou ao Idle.");
        animator.SetTrigger("Idle");
        gameObject.tag = tagOriginal;
        estaNocauteado = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private IEnumerator AtacarInicialmente()
    {
        animator.SetTrigger("Ataque");
        Debug.Log("?? Robo iniciou em modo de ATAQUE!");

        yield return new WaitForSeconds(4f);

        animator.SetTrigger("Idle");
        Debug.Log("?? Robo voltou para o estado IDLE.");

        // Agora mostra HUD e joystick
        if (HUD != null) HUD.SetActive(true);
        if (joystick != null) joystick.SetActive(true);
    }




    // GIZMOS — Mostrar os ranges no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, highlightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }


}
