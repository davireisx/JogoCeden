using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SenhaController : MonoBehaviour
{
    [Header("Câmera")]
    public CameraManagerEsdras cameraManager;

    [Header("UI")]
    public Text visor;
    public GameObject fadePanel;
    public GameObject imagemFechadura;
    public GameObject fechar;
    public GameObject joystick;
    public GameObject hud; // HUD geral (vida, mapa, inventário, etc)
    public GameObject objetivos1;
    public GameObject objetivos2;


    [Header("Feedback")]
    public GameObject textoCheck; // Referência ao GameObject que exibe o "CHECK"

    private CanvasGroup fadeCanvasGroup;

    [Header("Senha")]
    public string senhaCorreta;
    private string codigoDigitado = "";
    public int limiteDigitos = 4;

    [Header("Teleportação")]
    public Transform jogador;
    public Transform spawnPoint;

    private bool bloqueado = false;

    void Start()
    {
        AtualizarVisor();

        if (fadePanel != null)
        {
            fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f; // começa invisível
        }
    }

    public void DigitarNumero(string numero)
    {
        if (bloqueado) return;

        if (codigoDigitado.Length < limiteDigitos)
        {
            codigoDigitado += numero;
            AtualizarVisor();
        }
    }

    public void Apagar()
    {
        if (bloqueado) return;

        if (codigoDigitado.Length > 0)
        {
            codigoDigitado = codigoDigitado.Substring(0, codigoDigitado.Length - 1);
            AtualizarVisor();
        }
    }

    public void ValidarSenha()
    {
        if (bloqueado || codigoDigitado.Length < limiteDigitos)
            return;

        if (codigoDigitado == senhaCorreta)
        {
            StartCoroutine(Sucesso());
        }
        else
        {
            StartCoroutine(Erro());
        }
    }

    private void AtualizarVisor()
    {
        string texto = "";

        for (int i = 0; i < codigoDigitado.Length; i++)
        {
            texto += "  " + codigoDigitado[i] + "  ";
        }

        int asteriscosRestantes = limiteDigitos - codigoDigitado.Length;
        for (int i = 0; i < asteriscosRestantes; i++)
        {
            texto += "  *  ";
        }

        visor.text = texto.TrimEnd();
    }

    private IEnumerator Erro()
    {
        bloqueado = true;

        for (int i = 0; i < 4; i++)
        {
            visor.text = "";
            yield return new WaitForSeconds(0.15f);

            AtualizarVisor();
            yield return new WaitForSeconds(0.15f);
        }

        visor.text = "   E R R 0 R";
        yield return new WaitForSeconds(1f);

        codigoDigitado = "";
        AtualizarVisor();
        bloqueado = false;
    }

    private IEnumerator Sucesso()
    {
        bloqueado = true;

        if (fechar != null)
            fechar.SetActive(false);

        if (imagemFechadura != null)
            imagemFechadura.SetActive(false);

        if (hud != null)
            hud.SetActive(true); // <-- HUD fica visível aqui


        // Ativa o texto "CHECK"
        if (textoCheck != null)
            textoCheck.SetActive(true);

        // Espera 1 segundo antes de continuar
        yield return new WaitForSeconds(1f);


        if (hud != null)
            hud.SetActive(false);

        if (textoCheck != null)
            textoCheck.SetActive(false);


        // Fade in
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f, 1.5f, 1.5f));


        // Teleporta jogador
        if (jogador != null && spawnPoint != null)
        {
            jogador.position = spawnPoint.position;
            jogador.localScale = new Vector3(0.6f, 0.6f, jogador.localScale.z);
            jogador.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);

        }


        // Altera a câmera
        if (cameraManager != null)
            cameraManager.SetScenarioBounds(2);

        // Fade out
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1.5f, 0f, 1.5f));

        if (joystick != null)
            joystick.SetActive(true);

        if (hud != null)
            hud.SetActive(true);

        if (objetivos1 != null)
            objetivos1.SetActive(false);

        if (objetivos2 != null)
            objetivos2.SetActive(true);


        codigoDigitado = "";
        bloqueado = false;
        AtualizarVisor();
    }


    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = endAlpha;
    }
}
