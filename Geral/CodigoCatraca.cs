using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CodigoCatraca : MonoBehaviour
{
    public float tempoLimite = 60f;
    public int errosPermitidos = 3;
    public Text[] linhasCodigo;
    public Button[] botoesCorrecao;
    public GameObject painelSucesso, painelFalha;
    public AudioClip somAlerta;
    public Text tooltipTexto;
    public GameObject tooltipPanel;

    private string[] codigoCorreto = {
        "function ",
            "verificarCatraca() {",
        "  if (cartaoValido) {",
        "    liberarAcesso();",
        "    return true;",
        "  }",
        "  return false;",
        "}"
    };

    private string[] codigoCorrompido;
    private int errosAtuais = 0;
    private float tempoRestante;
    private bool jogoAtivo = true;
    private int linhaTooltipAtual = -1;

    void Start()
    {   
        tempoRestante = tempoLimite;
        CorromperCodigo();
        MostrarCodigo();
        StartCoroutine(ContadorTempo());
        ConfigurarBotoesTooltip();

        // Inicia piscagem para linhas corrompidas
        for (int i = 0; i < codigoCorrompido.Length; i++)
        {
            if (codigoCorrompido[i] != codigoCorreto[i])
            {
                StartCoroutine(PiscarBotaoErrado(i));
            }
        }
    }

    void ConfigurarBotoesTooltip()
    {
        for (int i = 0; i < botoesCorrecao.Length; i++)
        {
            int index = i; // Captura local para closure
            botoesCorrecao[i].onClick.AddListener(() => VerificarCorrecao(index));

            // Configura eventos de mouse
            EventTrigger trigger = botoesCorrecao[i].gameObject.AddComponent<EventTrigger>();

            // Entrada do mouse
            var pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((e) => MostrarTooltip(index));
            trigger.triggers.Add(pointerEnter);

            // Saída do mouse
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((e) => EsconderTooltip());
            trigger.triggers.Add(pointerExit);
        }
    }

    void CorromperCodigo()
    {
        codigoCorrompido = (string[])codigoCorreto.Clone();

        // Corrompe randomicamente algumas linhas
        for (int i = 0; i < codigoCorrompido.Length; i++)
        {
            if (Random.value > 0.5f)
            {
                codigoCorrompido[i] = CorromperLinha(codigoCorrompido[i]);
            }
        }
    }

    string CorromperLinha(string linha)
    {
        // Tipos diferentes de corrupção
        if (Random.value > 0.7f)
        {
            // Remove ponto e vírgula
            return linha.Replace(";", "");
        }
        else if (Random.value > 0.4f)
        {
            // Troca parênteses por chaves
            return linha.Replace("(", "{").Replace(")", "}");
        }
        else
        {
            // Troca valores booleanos
            return linha.Replace("true", "false").Replace("false", "true");
        }
    }

    void MostrarCodigo()
    {
        for (int i = 0; i < linhasCodigo.Length; i++)
        {
            if (i < codigoCorrompido.Length)
            {
                // Se a linha estiver corrompida, mostra em vermelho
                if (codigoCorrompido[i] != codigoCorreto[i])
                {
                    linhasCodigo[i].text = codigoCorrompido[i];
                    linhasCodigo[i].color = Color.red;
                }
                // Se estiver correta, mostra em verde
                else
                {
                    linhasCodigo[i].text = codigoCorreto[i];
                    linhasCodigo[i].color = Color.green;
                }
            }
        }
    }

    IEnumerator ContadorTempo()
    {
        while (tempoRestante > 0 && jogoAtivo)
        {
            yield return new WaitForSeconds(1f);
            tempoRestante--;

            if (tempoRestante <= 0)
            {
                TerminarJogo(false);
            }
        }
    }

    IEnumerator PiscarBotaoErrado(int linhaIndex)
    {
        Button botao = botoesCorrecao[linhaIndex];
        Image imagemBotao = botao.GetComponent<Image>();
        Color corOriginal = imagemBotao.color;

        while (jogoAtivo && codigoCorrompido[linhaIndex] != codigoCorreto[linhaIndex])
        {
            imagemBotao.color = Color.yellow;
            if (somAlerta != null) AudioSource.PlayClipAtPoint(somAlerta, Camera.main.transform.position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            imagemBotao.color = corOriginal;
            yield return new WaitForSeconds(0.5f);
        }

        // Quando corrigido, muda para verde
        if (codigoCorrompido[linhaIndex] == codigoCorreto[linhaIndex])
        {
            imagemBotao.color = Color.green;
        }
    }

    public void VerificarCorrecao(int linhaIndex)
    {
        if (!jogoAtivo || codigoCorrompido[linhaIndex] == codigoCorreto[linhaIndex]) return;

        // Verifica se a correção está certa (75% de chance de acerto)
        bool correto = Random.value > 0.25f;

        if (correto)
        {
            // Corrige a linha
            codigoCorrompido[linhaIndex] = codigoCorreto[linhaIndex];
            linhasCodigo[linhaIndex].text = codigoCorreto[linhaIndex];
            linhasCodigo[linhaIndex].color = Color.green;
            VerificarVitoria();
        }
        else
        {
            errosAtuais++;
            if (errosAtuais >= errosPermitidos)
            {
                TerminarJogo(false);
            }
        }
    }

    void MostrarTooltip(int linhaIndex)
    {
        if (!jogoAtivo) return;

        linhaTooltipAtual = linhaIndex;
        tooltipTexto.text = "Corrigir para:\n" + codigoCorreto[linhaIndex];
        tooltipPanel.SetActive(true);
    }

    void EsconderTooltip()
    {
        tooltipPanel.SetActive(false);
        linhaTooltipAtual = -1;
    }

    void VerificarVitoria()
    {
        for (int i = 0; i < codigoCorreto.Length; i++)
        {
            if (codigoCorrompido[i] != codigoCorreto[i])
            {
                return;
            }
        }
        TerminarJogo(true);
    }

    void TerminarJogo(bool vitoria)
    {
        jogoAtivo = false;
        if (vitoria)
        {
            painelSucesso.SetActive(true);
        }
        else
        {
            painelFalha.SetActive(true);
        }

        // Para todas as corrotinas de piscar
        StopAllCoroutines();
    }

    public void ReiniciarMiniGame()
    {
        // Reinicia todas as variáveis
        errosAtuais = 0;
        tempoRestante = tempoLimite;
        jogoAtivo = true;

        // Limpa painéis
        painelSucesso.SetActive(false);
        painelFalha.SetActive(false);
        tooltipPanel.SetActive(false);

        // Recria o código corrompido
        CorromperCodigo();
        MostrarCodigo();

        // Reseta os botões
        foreach (Button botao in botoesCorrecao)
        {
            botao.GetComponent<Image>().color = Color.white;
        }

        // Inicia novas corrotinas
        StopAllCoroutines();
        StartCoroutine(ContadorTempo());

        for (int i = 0; i < codigoCorrompido.Length; i++)
        {
            if (codigoCorrompido[i] != codigoCorreto[i])
            {
                StartCoroutine(PiscarBotaoErrado(i));
            }
        }
    }
}