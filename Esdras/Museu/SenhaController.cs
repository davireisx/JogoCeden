using System.Collections;
using System.Collections.Generic;
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
    public GameObject hud; // HUD geral
    public GameObject objetivos1;
    public GameObject objetivos2;
    public Text checkText;

    [Header("Feedback")]
    public GameObject textoCheck; // "CHECK"
    private CanvasGroup fadeCanvasGroup;

    [System.Serializable]
    public class ConfiguracaoDigito
    {
        public Button botao;               // Botão do dígito
        public Text texto;                 // Texto dentro do botão
        public Color corFeedback = Color.green;
        public AudioClip somClique;
    }

    [Header("Configuração de cada dígito")]
    public List<ConfiguracaoDigito> configuracoes = new List<ConfiguracaoDigito>();
    public AudioSource audioSource;

    [Header("Senha")]
    public string senhaCorreta;
    private string codigoDigitado = "";
    public int limiteDigitos = 4;
    public float escalaPlayer = 0.68f;

    [Header("Teleportação")]
    public Transform jogador;
    public Transform spawnPoint;

    private bool bloqueado = false;

    // Controle de feedback para cada botão
    private Dictionary<Button, Coroutine> feedbacks = new Dictionary<Button, Coroutine>();

    void Start()
    {
        AtualizarVisor();

        // Configura os listeners de clique para cada botão
        foreach (var cfg in configuracoes)
        {
            if (cfg.botao != null)
            {
                cfg.botao.onClick.AddListener(() => OnBotaoClicado(cfg));
            }
        }


        if (joystick != null)
            joystick.SetActive(false);

        if (checkText != null)
            checkText.gameObject.SetActive(false);

        if (fadePanel != null)
        {
            fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f;
        }
    }

    private void OnBotaoClicado(ConfiguracaoDigito cfg)
    {
        if (bloqueado) return;

        string valor = cfg.texto.text;

        // Feedback visual e sonoro
        if (!feedbacks.ContainsKey(cfg.botao) || feedbacks[cfg.botao] == null)
            StartCoroutine(FeedbackDigito(cfg));

        // Se for número (0-9), adiciona
        if (char.IsDigit(valor, 0))
        {
            DigitarNumero(valor);
        }
        // Se for apagar
        else if (valor == "←")
        {
            Apagar();
        }
        // Se for OK
        else if (valor == "OK")
        {
            ValidarSenha();
        }
    }


    private void DigitarNumero(string numero)
    {
        if (bloqueado) return;

        if (codigoDigitado.Length < limiteDigitos)
        {
            codigoDigitado += numero[0]; // Adiciona apenas o primeiro caractere
            AtualizarVisor();
        }
    }


    private IEnumerator FeedbackDigito(ConfiguracaoDigito cfg)
    {
        if (cfg.somClique != null && audioSource != null)
            audioSource.PlayOneShot(cfg.somClique);

        Color corOriginal = cfg.texto.color;
        cfg.texto.color = cfg.corFeedback;

        // Armazena coroutine para evitar reentrada
        feedbacks[cfg.botao] = StartCoroutine(ResetarCor(cfg, corOriginal));
        yield return feedbacks[cfg.botao];
    }

    private IEnumerator ResetarCor(ConfiguracaoDigito cfg, Color corOriginal)
    {
        yield return new WaitForSeconds(0.2f); // Intervalo do feedback
        cfg.texto.color = corOriginal;
        feedbacks[cfg.botao] = null; // Libera para novo clique
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
        // Sempre mostra 4 posições
        char[] display = new char[limiteDigitos];

        for (int i = 0; i < limiteDigitos; i++)
        {
            if (i < codigoDigitado.Length)
                display[i] = codigoDigitado[i];
            else
                display[i] = '*';
        }

        // Monta a string final do visor
        visor.text = $"{display[0]}  {display[1]}  {display[2]}  {display[3]}";
    }


    private IEnumerator Erro()
    {
        bloqueado = true;

        for (int i = 0; i < 4; i++)
        {
            visor.text = " ";
            yield return new WaitForSeconds(0.15f);

            AtualizarVisor();
            yield return new WaitForSeconds(0.15f);
        }

        visor.text = "  E  R  R  0  R";
        yield return new WaitForSeconds(1f);

        codigoDigitado = " ";
        AtualizarVisor();
        bloqueado = false;
    }

    private IEnumerator Sucesso()
    {
        bloqueado = true;

        if (fechar != null) fechar.SetActive(false);
        if (imagemFechadura != null) imagemFechadura.SetActive(false);
        if (hud != null) hud.SetActive(true);
        if (textoCheck != null) textoCheck.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (hud != null) hud.SetActive(false);
        if (textoCheck != null) textoCheck.SetActive(false);

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(0f, 1.5f, 1.5f));

        if (jogador != null && spawnPoint != null)
        {
            jogador.position = spawnPoint.position;
            jogador.localScale = new Vector3(escalaPlayer, escalaPlayer, jogador.localScale.z);
            jogador.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
        }

        if (cameraManager != null)
            cameraManager.SetScenarioBounds(2);

        if (fadeCanvasGroup != null)
            yield return StartCoroutine(Fade(1.5f, 0f, 1.5f));

        if (joystick != null) joystick.SetActive(true);
        if (hud != null) hud.SetActive(true);
        if (objetivos1 != null) objetivos1.SetActive(false);
        if (objetivos2 != null) objetivos2.SetActive(true);

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
