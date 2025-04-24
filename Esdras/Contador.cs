using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Contador : MonoBehaviour
{
    public int contador = 0;
    public Button[] botoes;
    public Text palavraText;
    public Text timerText;
    public AudioSource ticTacSom, somAcerto, somErro, musicaFundo;
    public GameObject telaVitoria; // Refer�ncia para a tela de vit�ria
    public GameObject telaDerrota; // Refer�ncia para a tela de derrota
    public Button botaoReiniciar; // Refer�ncia para o bot�o de reiniciar
    private string palavraOriginal = "ETE FMC";
    private string[] todasPalavrasEmbaralhadas;
    private int indicePalavraAtual = 0;
    private bool podeClicarLaranja = true;
    private float tempoRestante = 60f;
    private bool jogoAtivo = true;
    private bool modoFrenetico = false;
    public Button botaoReiniciarVitoria; // Bot�o na tela de vit�ria
    public Button botaoReiniciarDerrota; // Bot�o na tela de derrota


    private int cliquesLaranja = 0; // Rastreia os cliques laranja
    private int botoesVerdesTransformados = 0; // Rastreia quantos bot�es verdes j� foram transformados

    void Start()
    {
        todasPalavrasEmbaralhadas = new string[] {
            "MEC EFT",
            "CET EFM",
            "ECT TEF",
            "ETM EFC",
            "ETE CFM",
            "ETE FCM",
            palavraOriginal
        };

        AtualizarPalavraComCores();
        AtualizarTimerUI();
        musicaFundo.Play();

        foreach (Button botao in botoes)
        {
            botao.onClick.AddListener(BotaoClicado);
        }

        MudarCorDosBotoes();
        StartCoroutine(TimerJogo());

        // Desativa as telas de vit�ria e derrota no in�cio do jogo
        telaVitoria.SetActive(false);
        telaDerrota.SetActive(false);

        // Configura o bot�o de reiniciar
        botaoReiniciar.onClick.AddListener(ReiniciarJogo);
    }

    void BotaoClicado()
    {
        if (!jogoAtivo) return;

        GameObject botaoClicado = EventSystem.current.currentSelectedGameObject;
        Image corBotao = botaoClicado.GetComponent<Image>();
        Button botao = botaoClicado.GetComponent<Button>();

        // Se o jogador clicar em um bot�o laranja
        if (corBotao.color == HexToColor("#FD8500"))
        {
            IncrementarContador();
            somAcerto.Play(); // Som de acerto
            cliquesLaranja++; // Aumenta o n�mero de cliques nos bot�es laranja

            // Mudar a cor do bot�o para branco imediatamente e mant�-lo interativo
            botao.GetComponent<Image>().color = HexToColor("#000000");
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(BotaoClicado); // Retorna a fun��o de clique para o bot�o

            // A cada 10 cliques laranja, transforma 3 novos bot�es em verdes
            if (cliquesLaranja % 10 == 0)
            {
                FazerBotoesVerdes();
            }
        }

        if (corBotao.color == HexToColor("#000000"))
        {
            somErro.Play();
        }

        // Verifica se todos os bot�es est�o verdes
        VerificarVitoria();
    }

    // Fun��o que transforma 3 bot�es em verdes progressivamente
    void FazerBotoesVerdes()
    {
        int contagemVerde = 0;
        foreach (Button botao in botoes)
        {
            if (contagemVerde < 3 && botao.GetComponent<Image>().color != HexToColor("#09B420"))
            {
                botao.GetComponent<Image>().color = HexToColor("#09B420");
                botao.interactable = false; // Torna o bot�o verde n�o clic�vel
                contagemVerde++;
            }
        }
        botoesVerdesTransformados += contagemVerde; // Atualiza o contador de bot�es verdes transformados
    }

    void IncrementarContador()
    {
        if (contador < 10)
        {
            contador += 1;
        }

        if (contador >= 10)
        {
            contador = 0;
            MudarPalavraEmbaralhada();
            MudarCorDosBotoes();
        }
    }

    void MudarPalavraEmbaralhada()
    {
        if (indicePalavraAtual < todasPalavrasEmbaralhadas.Length - 1)
        {
            indicePalavraAtual++;
        }

        AtualizarPalavraComCores();
    }

    void AtualizarPalavraComCores()
    {
        string palavraAtual = todasPalavrasEmbaralhadas[indicePalavraAtual];
        string palavraFormatada = "";

        for (int i = 0; i < palavraAtual.Length; i++)
        {
            if (i < palavraOriginal.Length && palavraAtual[i] == palavraOriginal[i])
            {
                palavraFormatada += $"<color=#09B420>{palavraAtual[i]}</color>";
            }
            else
            {
                palavraFormatada += $"<color=#FF0012>{palavraAtual[i]}</color>";
            }
        }

        palavraText.text = palavraFormatada;
    }

    void MudarCorDosBotoes()
    {
        StartCoroutine(TrocarBotaoLaranja());
    }

    IEnumerator TrocarBotaoLaranja()
    {
        while (jogoAtivo)
        {
            float tempoEspera = modoFrenetico ? 1.5f : tempoRestante > 30 ? 1.5f : 1f;

            // Conta quantos bot�es est�o laranja
            int botaoLaranjaCount = 0;
            foreach (Button botao in botoes)
            {
                if (botao.GetComponent<Image>().color == HexToColor("#FD8500"))
                {
                    botaoLaranjaCount++;
                }
            }

            // Se houver menos de 4 bot�es laranja, muda alguns bot�es para laranja
            if (botaoLaranjaCount < 4)
            {
                int numeroBotaoLaranja = Random.Range(1, 5 - botaoLaranjaCount); // Limita o n�mero de bot�es laranja a 4
                for (int i = 0; i < numeroBotaoLaranja; i++)
                {
                    int indiceBotaoCorreto = Random.Range(0, botoes.Length);
                    Button botaoCorreto = botoes[indiceBotaoCorreto];

                    // Evita que bot�es verdes sejam alterados para laranja
                    if (botaoCorreto.GetComponent<Image>().color != HexToColor("#09B420"))
                    {
                        // Muda a cor do bot�o para laranja
                        botaoCorreto.GetComponent<Image>().color = HexToColor("#FD8500");
                        botaoCorreto.onClick.RemoveAllListeners();
                        botaoCorreto.onClick.AddListener(BotaoClicado);

                        // Inicia uma corrotina para mudar a cor de volta para branco ap�s 1.5 segundos
                        StartCoroutine(VoltarBotaoParaBranco(botaoCorreto, 1f));
                    }
                }
            }

            yield return new WaitForSeconds(tempoEspera);
        }
    }

    IEnumerator VoltarBotaoParaBranco(Button botao, float tempo)
    {
        yield return new WaitForSeconds(tempo);

        // Verifica se o bot�o ainda est� laranja (n�o foi clicado e transformado em verde)
        if (botao.GetComponent<Image>().color == HexToColor("#FD8500"))
        {
            botao.GetComponent<Image>().color = HexToColor("#000000");
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(BotaoClicado);
        }
    }

    IEnumerator TimerJogo()
    {
        while (tempoRestante > 0)
        {
            yield return new WaitForSeconds(1f);
            tempoRestante--;
            AtualizarTimerUI();

            if (tempoRestante == 10)
            {
                ticTacSom.Play();
            }
            else if (tempoRestante <= 30 && !modoFrenetico)
            {
                modoFrenetico = true;
                StartCoroutine(TrocarBotaoLaranja());
            }
        }

        FimDoJogo();
    }

    void AtualizarTimerUI()
    {
        int minutos = Mathf.FloorToInt(tempoRestante / 60);
        int segundos = Mathf.FloorToInt(tempoRestante % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (tempoRestante <= 10)
        {
            timerText.color = Color.red;
        }
    }

    void FimDoJogo()
    {
        jogoAtivo = false;
        timerText.text = "Tempo Esgotado!";
        musicaFundo.Stop();
        ticTacSom.Stop();

        foreach (Button botao in botoes)
        {
            botao.interactable = false;
        }

        // Verifica se todos os bot�es est�o verdes
        VerificarVitoria();
    }

    public void OnRestartButtonPressed()
    {
        Debug.Log("Bot�o de mudan�a de cena pressionado.");
        if (!string.IsNullOrEmpty("Poder - Esdras"))
        {
            Debug.Log("Tentando carregar a cena Poder: ");
            SceneManager.LoadScene("Poder - Esdras"); // Carrega a cena especificada
        }
        else
        {
            Debug.LogError("O nome da cena n�o foi definido no Inspector.");
        }
    }

    void VerificarVitoria()
    {
        bool todosVerdes = true;
        foreach (Button botao in botoes)
        {
            if (botao.GetComponent<Image>().color != HexToColor("#09B420"))
            {
                todosVerdes = false;
                break;
            }
        }

        if (todosVerdes)
        {
            telaVitoria.SetActive(true); // Ativa a tela de vit�ria
        }
        else if (tempoRestante <= 0)
        {
            telaDerrota.SetActive(true); // Ativa a tela de derrota
        }
    }

    void ReiniciarJogo()
    {
        // Reinicia todas as vari�veis e estados do jogo
        contador = 0;
        indicePalavraAtual = 0;
        tempoRestante = 60f;
        jogoAtivo = true;
        modoFrenetico = false;
        cliquesLaranja = 0;
        botoesVerdesTransformados = 0;

        // Reseta os bot�es
        foreach (Button botao in botoes)
        {
            botao.GetComponent<Image>().color = HexToColor("#000000");
            botao.interactable = true;
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(BotaoClicado);
        }

        // Reseta as telas de vit�ria e derrota
        telaVitoria.SetActive(false);
        telaDerrota.SetActive(false);

        // Reinicia o jogo
        AtualizarPalavraComCores();
        AtualizarTimerUI();
        musicaFundo.Play();
        StartCoroutine(TimerJogo());
    }

    Color HexToColor(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

        return new Color(r, g, b);
    }
}