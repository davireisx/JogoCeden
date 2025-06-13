using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SenhaControllerDentroMuseu : MonoBehaviour
{

    [Header("Câmera")]
    public CameraManagerEsdras cameraManager;

    [Header("UI")]
    public Text visor;
    public GameObject imagemFechadura;
    public GameObject fechar;
    public GameObject joystick;
    public GameObject HUD;                    // <- Novo
    public Text checkText;
    public GameObject objetivos2;
    public GameObject objetivos3;                   // <- Novo
    public CanvasGroup fadeCanvasGroup;      // <- Novo

    [Header("Senha")]
    public string senhaCorreta;
    private string codigoDigitado = "";
    public int limiteDigitos = 6;
    private bool bloqueado = false;

    [Header("Transição")]
    public Transform jogador;                // <- Novo
    public Transform spawnPoint;             // <- Novo

    void Start()
    {
        AtualizarVisor();

        if (joystick != null)
            joystick.SetActive(false);

        if (checkText != null)
            checkText.gameObject.SetActive(false);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
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

        visor.text = "     E    R    R    0    R";
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

        if (joystick != null)
            joystick.SetActive(true);

        // Mostrar texto "CHECK"
        if (checkText != null)
            checkText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);


        if (HUD != null)
            HUD.SetActive(false);

        // Fade in
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f, 1f, 0.75f));


        // Altera a câmera
        if (cameraManager != null)
            cameraManager.SetScenarioBounds(4);

        // Teleporte
        if (jogador != null && spawnPoint != null)
            jogador.position = spawnPoint.position;

        // Fade out
        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1f, 0f, 0.75f));

        if (checkText != null)
            checkText.gameObject.SetActive(false);
        

        if (objetivos2 != null)
            objetivos2.SetActive(false);

        if (objetivos3 != null)
            objetivos3.SetActive(true);

        if (HUD != null)
            HUD.SetActive(true);



        bloqueado = false;
        codigoDigitado = "";
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
