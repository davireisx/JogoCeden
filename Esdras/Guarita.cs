using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guarita : MonoBehaviour
{
    [Header("Imagens")]
    public GameObject VSCODE;
    public GameObject protocolo;
    public GameObject botaoDebuggar;
    public GameObject acertos;
    public GameObject guaritaGeral;

    [Header("Objetivos e Checks")]
    public GameObject objetivos2;
    public GameObject check2;
    public GameObject fundo1;
    public GameObject fundo2;
    public GameObject fundomonitor;
    public GameObject objetivos3;
    public GameObject check3; 
    public GameObject objetivos4;
    public GameObject check4;

    [Header("Protocolo - Mecânica")]
    public List<Text> linhasBinarias;
    public Image protocoloImage;
    public Text textoFinal;
    public float velocidadeDigitacao = 0.0001f;

    [Header("Debug - Mecânica")]
    public Image debugImage;
    public Text textoDebug;
    public List<Graphic> componentesParaDesaparecer;
    public List<Graphic> componentesParaAparecer;
    public Text mensagemSucesso;

    [Header("Traps que vão piscar")]
    public List<SpriteRenderer> trapsPiscam;
    public GameObject trapCollider1;
    public GameObject trapCollider2;
    public GameObject trapCollider3;
    public GameObject trapCollider4;

    [Header("Referências")]
    public Arquiteto arquitetas;
    public Joystick joystick;
    public CanvasGroup fade;
    public AudioSource audioTeclando;
    public AudioSource audioPendrive;
    public AudioSource audioDebug;
    public AudioSource audioCodigoDesaparecendo;
    public AudioSource audioCodigoAparecendo;
    public AudioSource audioCodigoFim;
    public AudioSource audioCatracaDesativa;
    public float velocidadeFade;
    public float tempoPiscada = 0.2f;
    public float trapPiscar = 3f;
    public float intervaloPiscada = 0.2f;

    private bool iniciarFade = false;
    private bool iniciarProtocolo = false;
    private bool protocoloCompleto = false;



    void Start()
    {

        InicializarComponentes();
    }

    void InicializarComponentes()
    {       

        if (protocoloImage != null)
            protocoloImage.color = new Color(protocoloImage.color.r, protocoloImage.color.g, protocoloImage.color.b, 0f);

        if (protocolo != null)
            protocolo.SetActive(false);

        foreach (Text linha in linhasBinarias)
            if (linha != null) linha.gameObject.SetActive(false);

        if (textoFinal != null)
            textoFinal.gameObject.SetActive(false);

        if (botaoDebuggar != null)
            botaoDebuggar.SetActive(false);

        if (debugImage != null)
            debugImage.gameObject.SetActive(false);

        if (textoDebug != null)
            textoDebug.gameObject.SetActive(false);

        foreach (Graphic componente in componentesParaAparecer)
            if (componente != null) componente.gameObject.SetActive(false);

        if (mensagemSucesso != null)
            mensagemSucesso.gameObject.SetActive(false);

        if (objetivos2 != null) objetivos2.SetActive(false);
        if (check2 != null) check2.SetActive(false);
        if (objetivos3 != null) objetivos3.SetActive(false);
        if (check3 != null) check3.SetActive(false);
    }

    public void Inicio()
    {
        joystick.gameObject.SetActive(false);

        if (fade != null)
        {
            fade.alpha = 1f;
            iniciarFade = true;
        }
    }

    public void Update()
    {
        if (iniciarFade)
            ProcessarFade();

        if (iniciarProtocolo && protocoloImage != null)
            ProcessarProtocolo();

        // ?? Desativa objetivos3 e check3 quando guaritaGeral some
        if (guaritaGeral != null && !guaritaGeral.activeSelf)
        {
            //if (objetivos3 != null) objetivos3.SetActive(false);
           // if (check3 != null) check3.SetActive(false);
        }
    }

    void ProcessarFade()
    {
        check2.SetActive(true);
       // yield return new WaitForSeconds(0.5f);
        objetivos2.SetActive(false);
        fundo1.SetActive(false);
        fade.alpha -= Time.deltaTime * velocidadeFade;

        if (fade.alpha <= 0f)
        {
            fade.alpha = 0f;
            iniciarFade = false;
            fade.gameObject.SetActive(false);
            fundomonitor.SetActive(true);
            if (objetivos3 != null) objetivos3.SetActive(true);
        }
    }

    void ProcessarProtocolo()
    {
        StartCoroutine(AtivarComponentesHUD());
        Color currentColor = protocoloImage.color;
        float newAlpha = currentColor.a + (Time.deltaTime * velocidadeFade);
        protocoloImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

        if (protocoloImage.color.a >= 1f && !protocoloCompleto)
        {
            protocoloImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            iniciarProtocolo = false;
            protocoloCompleto = true;
            StartCoroutine(SequenciaLinhasBinarias());
        }
    }

    public void Ativar()
    {
        audioPendrive.Play();   
        Protocolo();
    }

    //======================== PROTOCOLO - MECÂNICA ================================

    public void Protocolo()
    {
        if (protocoloImage != null)
        {
            if (protocolo != null)
                protocolo.SetActive(true);

            protocoloImage.color = new Color(protocoloImage.color.r, protocoloImage.color.g, protocoloImage.color.b, 0f);
            iniciarProtocolo = true;
        }
    }

    private IEnumerator SequenciaLinhasBinarias()
    {
        yield return StartCoroutine(DigitarLinhas());
        yield return StartCoroutine(PiscarVerde());
        yield return StartCoroutine(ManterVerde());
        yield return StartCoroutine(RemoverLetras());
        yield return StartCoroutine(ProcessarTextoFinal());
        yield return StartCoroutine(FadeOutProtocolo());
    }

    private IEnumerator FadeOutProtocolo()
    {
        // Fade out da imagem do protocolo
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * velocidadeFade;
            protocoloImage.color = new Color(protocoloImage.color.r, protocoloImage.color.g, protocoloImage.color.b, alpha);
            yield return null;
        }
        protocoloImage.color = new Color(protocoloImage.color.r, protocoloImage.color.g, protocoloImage.color.b, 0f);

        // Espera 1.5 segundos e ativa botão debug
        yield return new WaitForSeconds(1.5f);
        protocolo.SetActive(false);


        if (botaoDebuggar != null)
        {
            botaoDebuggar.SetActive(true);
            if (debugImage != null) debugImage.gameObject.SetActive(true);
            if (textoDebug != null) textoDebug.gameObject.SetActive(true);
        }
    }

 public IEnumerator AtivarComponentesHUD()
    {
        check3.SetActive(true);
        yield return new WaitForSeconds(20);
        objetivos3.SetActive(false);
        objetivos4.SetActive(true);
    }

    public void AcionarDebug()
    {
        audioDebug.Play();
        StartCoroutine(SequenciaDebug());
    }

    private IEnumerator SequenciaDebug()
    {
        audioCodigoDesaparecendo.Play();
        check4.SetActive(true);
        // Desativa botão debug
        if (botaoDebuggar != null) botaoDebuggar.SetActive(false);

        // Desaparece componentes um por um
        foreach (Graphic componente in componentesParaDesaparecer)
        {
            if (componente != null)
            {
                componente.gameObject.SetActive(true);
                float alpha = 1f;
                while (alpha > 0f)
                {
                    alpha -= Time.deltaTime * 4f;
                    componente.color = new Color(componente.color.r, componente.color.g, componente.color.b, alpha);
                    yield return null;
                }
                componente.color = new Color(componente.color.r, componente.color.g, componente.color.b, 0f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        // Desativa todos os componentes
        foreach (Graphic componente in componentesParaDesaparecer)
            if (componente != null) componente.gameObject.SetActive(false);

        audioCodigoDesaparecendo.Stop();
        yield return new WaitForSeconds(1.5f);

        audioCodigoAparecendo.Play();
        // Aparece componentes um por um
        for (int i = 0; i < componentesParaAparecer.Count; i++)
        {
            Graphic componente = componentesParaAparecer[i];
            if (componente != null)
            {
                componente.gameObject.SetActive(true);
                float targetAlpha = (i == componentesParaAparecer.Count - 1) ? 0.25f : 1f;

                float alpha = 0f;
                while (alpha < targetAlpha)
                {
                    alpha += Time.deltaTime * 4f;
                    componente.color = new Color(componente.color.r, componente.color.g, componente.color.b, alpha);
                    yield return null;
                }
                componente.color = new Color(componente.color.r, componente.color.g, componente.color.b, targetAlpha);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Ativa mensagem de sucesso
        if (mensagemSucesso != null)
        {
            audioCodigoAparecendo.Stop();
            audioCodigoFim.Play();  
            Debug.Log("Mensagem de sucesso ativada: " + mensagemSucesso.text);
            mensagemSucesso.gameObject.SetActive(true);
            mensagemSucesso.color = new Color(mensagemSucesso.color.r, mensagemSucesso.color.g, mensagemSucesso.color.b, 1f);

            string textoOriginal = mensagemSucesso.text.Trim();
            if (string.IsNullOrEmpty(textoOriginal))
            {
                Debug.LogError("Texto da mensagem de sucesso está vazio!");
                yield break;
            }

            mensagemSucesso.text = "";
            Debug.Log("Iniciando digitação com texto: '" + textoOriginal + "'");

            foreach (char letra in textoOriginal)
            {
                mensagemSucesso.text += letra;
                yield return new WaitForSeconds(velocidadeDigitacao);
            }

            Debug.Log("Digitação concluída: '" + mensagemSucesso.text + "'");

            yield return new WaitForSeconds(2f);

            // Fade final
            if (fade != null)
            {
                fade.gameObject.SetActive(true);
                float alpha = 0f;
                while (alpha < 1f)
                {
                    alpha += Time.deltaTime * velocidadeFade;
                    fade.alpha = alpha;
                    yield return null;
                }
                fade.alpha = 1f;

                if (guaritaGeral != null) guaritaGeral.SetActive(false);
                fundomonitor.SetActive(false);
                fundo2.SetActive(true);
                fade.gameObject.SetActive(false);
                joystick.gameObject.SetActive(true);
                StartCoroutine(IniciarPiscada());

            }
        }
    }


   private IEnumerator IniciarPiscada()
    {
        audioCatracaDesativa.Play();
        StartCoroutine(PiscarSprites());
        yield return new WaitForSeconds(3f);
        audioCatracaDesativa.Stop();
        trapCollider1.SetActive(false);
        trapCollider2.SetActive(false);
        trapCollider3.SetActive(false);
        trapCollider4.SetActive(false);
        yield return new WaitForSeconds(1f);
        arquitetas.IniciarMovimento();
    }

    private IEnumerator PiscarSprites()
    {
        if (trapsPiscam == null || trapsPiscam.Count == 0)
            yield break;

        float tempoTotal = 0f;

        // Definição das cores (alpha < 120/255f ? 0.47f)
        Color preto = new Color(0f, 0f, 0f, 120f / 255f);
        Color branco = new Color(1f, 1f, 1f, 120f / 255f);
        Color transparente = new Color(0f, 0f, 0f, 0f);

        // Ciclo principal
        Color[] cicloNormal = { preto, branco, transparente };
        int indexNormal = 0;

        while (tempoTotal < trapPiscar)
        {
            Color corAtual = cicloNormal[indexNormal];

            for (int i = 0; i < trapsPiscam.Count; i++)
            {
                if (trapsPiscam[i] != null)
                {
                    if (i == trapsPiscam.Count - 1)
                    {
                        // Último sprite segue a regra
                        if (corAtual == branco)
                            trapsPiscam[i].color = preto; // mantém preto quando os outros estão brancos
                        else
                            trapsPiscam[i].color = corAtual; // segue normal (preto ou transparente)
                    }
                    else
                    {
                        trapsPiscam[i].color = corAtual;
                    }
                }
            }

            // Avança ciclo
            indexNormal = (indexNormal + 1) % cicloNormal.Length;

            yield return new WaitForSeconds(intervaloPiscada);
            tempoTotal += intervaloPiscada;
        }

        // Desativa todos no final
        foreach (SpriteRenderer sr in trapsPiscam)
        {
            if (sr != null)
                sr.gameObject.SetActive(false);
        }
    }





    // ======================== EFEITOS DE TEXTO ==================================

    private IEnumerator DigitarLinhas()
    {
        audioTeclando.Play();

        foreach (Text linha in linhasBinarias)
        {
            if (linha != null)
            {
                linha.gameObject.SetActive(true);
                string textoOriginal = linha.text;
                linha.text = "";

                foreach (char letra in textoOriginal)
                {
                    linha.text += letra;
                    yield return new WaitForSeconds(velocidadeDigitacao);
                }
                yield return new WaitForSeconds(0.005f);
            }
        }
    }

    private IEnumerator PiscarVerde()
    {
        audioTeclando.Stop();
        for (int i = 0; i < 2; i++)
        {
            foreach (Text linha in linhasBinarias)
                if (linha != null) linha.color = Color.green;

            yield return new WaitForSeconds(tempoPiscada);

            foreach (Text linha in linhasBinarias)
                if (linha != null) linha.gameObject.SetActive(false);

            yield return new WaitForSeconds(tempoPiscada);

            foreach (Text linha in linhasBinarias)
                if (linha != null) linha.gameObject.SetActive(true);
        }
    }

    private IEnumerator ManterVerde()
    {
        foreach (Text linha in linhasBinarias)
            if (linha != null) linha.color = Color.green;

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator RemoverLetras()
    {
        audioTeclando.Play();
        foreach (Text linha in linhasBinarias)
        {
            if (linha != null)
            {
                string textoCompleto = linha.text;
                int comprimento = textoCompleto.Length;
                linha.supportRichText = true;

                for (int i = 0; i <= comprimento; i++)
                {
                    string textoOculto = "";
                    for (int j = 0; j < comprimento; j++)
                    {
                        if (j < i)
                            textoOculto += "<color=#00000000>" + textoCompleto[j] + "</color>";
                        else
                            textoOculto += textoCompleto[j];
                    }
                    linha.text = textoOculto;
                    yield return new WaitForSeconds(velocidadeDigitacao);
                }
                linha.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ProcessarTextoFinal()
    {
        audioTeclando.Stop();
        if (textoFinal != null)
        {
            textoFinal.gameObject.SetActive(true);
            string textoOriginalFinal = textoFinal.text;
            textoFinal.text = "";

            foreach (char letra in textoOriginalFinal)
            {
                textoFinal.text += letra;
                yield return new WaitForSeconds(velocidadeDigitacao);
            }

            yield return new WaitForSeconds(2f);

            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * 2f;
                textoFinal.color = new Color(textoFinal.color.r, textoFinal.color.g, textoFinal.color.b, alpha);
                yield return null;
            }

            textoFinal.gameObject.SetActive(false);
            
        }
    }
}
