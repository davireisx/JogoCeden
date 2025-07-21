using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class InteragirTrocaCenario : MonoBehaviour
{
    public enum CenarioDestino
    {
        Cenario3 = 3,
        Cenario4 = 4,
        Cenario5 = 5
        // Adicione mais conforme necessário
    }

    [Header("Configurações de Interação")]
    public Transform player;
    public float interactionRange = 3f;

    [Header("Câmera e Spawn")]
    public CameraManagerEsdras cameraManager;
    public Transform spawnPoint;
    public GameObject joystick;
    public CenarioDestino destinoDoCenario; // Seletor de cenário via Inspector

    [Header("Fade (UI Image)")]
    public Image telaFade;
    public float fadeDuration = 1f;
    public GameObject HUD;

    [Header("Visual")]
    public Color highlightColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool playerInRange = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (telaFade == null)
            Debug.LogError("Imagem de fade não atribuída!");

        if (telaFade != null)
        {
            Color c = telaFade.color;
            c.a = 0;
            telaFade.color = c;
            telaFade.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        VerificarDistancia();
        AplicarBrilho();

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            VerificarCliqueOuToque(touchPos);
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            VerificarCliqueOuToque(clickPos);
        }
    }

    void VerificarDistancia()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;
    }

    void AplicarBrilho()
    {
        if (spriteRenderer == null) return;

        if (playerInRange)
        {
            float pulse = Mathf.PingPong(Time.time * 2f, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, highlightColor, pulse);
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    void VerificarCliqueOuToque(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject && playerInRange)
        {
            StartCoroutine(FadeTrocaCenario());
        }
    }

    IEnumerator FadeTrocaCenario()
    {
        telaFade.gameObject.SetActive(true);
        HUD.gameObject.SetActive(false);
        yield return StartCoroutine(FadeIn());

        player.position = spawnPoint.position;
        player.gameObject.SetActive(false);
        joystick?.SetActive(false);
        cameraManager?.SetScenarioBounds((int)destinoDoCenario); // Usando enum

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(FadeOut());

        telaFade.gameObject.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        Color c = telaFade.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            telaFade.color = c;
            yield return null;
        }
        c.a = 1;
        telaFade.color = c;
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Color c = telaFade.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            telaFade.color = c;
            yield return null;
        }
        c.a = 0;
        telaFade.color = c;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
