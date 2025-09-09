using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

using static Catraca;

public class CatracaManager : MonoBehaviour
{
    [Header("Contadores de HUD")]
    public Text textoFacil;
    public Text textoMedio;
    public Text textoDificil;

    [Header("Checks de confirmação")]
    public GameObject checkFacil;
    public GameObject checkMedio;
    public GameObject checkDificil;

    [Header("Check geral de vitória")]
    public GameObject checkGeral;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public Image fadeImage;
    public float fadeDuration = 2f;

    [Header("Objetos finais a desativar")]
    public GameObject trapCatraca;
    public GameObject arquiteto;

    [Header("Cenas")]
    public string nomeCenaVitoria;
    public string nomeCenaDerrota;

    private Dictionary<Dificuldade, int> resolvidas = new Dictionary<Dificuldade, int>();
    private Dictionary<Dificuldade, int> falhas = new Dictionary<Dificuldade, int>();

    private Color corVitoria = new Color(230f / 255f, 240f / 255f, 255f / 255f); // E6F0FF

    private void Start()
    {
        resolvidas[Dificuldade.Facil] = 0;
        resolvidas[Dificuldade.Medio] = 0;
        resolvidas[Dificuldade.Dificil] = 0;

        falhas[Dificuldade.Facil] = 0;
        falhas[Dificuldade.Medio] = 0;
        falhas[Dificuldade.Dificil] = 0;

        AtualizarHUD();
    }

    public void CatracaResolvida(Dificuldade dificuldade)
    {
        if (resolvidas[dificuldade] == 0) // conta só a primeira de cada tipo
        {
            resolvidas[dificuldade]++;
            AtualizarHUD();
            VerificarVitoria();

            switch (dificuldade)
            {
                case Dificuldade.Facil: if (checkFacil != null) checkFacil.SetActive(true); break;
                case Dificuldade.Medio: if (checkMedio != null) checkMedio.SetActive(true); break;
                case Dificuldade.Dificil: if (checkDificil != null) checkDificil.SetActive(true); break;
            }
        }
    }



    public void CatracaFalhou(Dificuldade dificuldade)
    {
        falhas[dificuldade]++;
        if (resolvidas[dificuldade] == 0 && falhas[dificuldade] >= 2)
        {
            StartCoroutine(DerrotaFinal());
        }
    }

    IEnumerator DerrotaFinal()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nomeCenaDerrota);
    }



    void AtualizarHUD()
    {
        if (textoFacil) textoFacil.text = $"Fácil:   {resolvidas[Dificuldade.Facil]}  /  1";
        if (textoMedio) textoMedio.text = $"Médio:   {resolvidas[Dificuldade.Medio]}  /  1";
        if (textoDificil) textoDificil.text = $"Difícil:   {resolvidas[Dificuldade.Dificil]}  /  1";
    }

    void VerificarVitoria()
    {
        if (resolvidas[Dificuldade.Facil] >= 1 &&
            resolvidas[Dificuldade.Medio] >= 1 &&
            resolvidas[Dificuldade.Dificil] >= 1)
        {
            if (trapCatraca) trapCatraca.SetActive(false);
            StartCoroutine(PiscarEDiminuirArquiteto()); 


            // Ativa o check geral
            if (checkGeral != null) checkGeral.SetActive(true);

            // Reinicia a cor de TODAS as catracas para cor normal
            Catraca[] catracas = FindObjectsOfType<Catraca>();
            foreach (Catraca c in catracas)
            {
                c.SetColor(corVitoria);
            }

            // Espera 2 segundos e carrega a cena de vitória
            StartCoroutine(VitoriaFinal());
        }
    }


    IEnumerator PiscarEDiminuirArquiteto()
    {
        if (arquiteto == null)
            yield break;

        SpriteRenderer sr = arquiteto.GetComponent<SpriteRenderer>();
        if (sr == null)
            yield break;

        Animator anim = arquiteto.GetComponent<Animator>();

        float duracao = 3f; // duração total do efeito
        float tempoDecorrido = 0f;
        Vector3 escalaInicial = arquiteto.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 0.5f; // reduzindo para metade

        Color preto = new Color(0f, 0f, 0f, 1f);
        Color branco = new Color(1f, 1f, 1f, 1f);
        Color transparente = new Color(0f, 0f, 0f, 0f);
        Color[] cores = { preto, branco, transparente };
        int indexCor = 0;

        float intervaloPiscada = 0.2f; // tempo entre cada troca de cor
        float proxTroca = 0f;

        while (tempoDecorrido < duracao)
        {
            // Troca de cor
            if (tempoDecorrido >= proxTroca)
            {
                sr.color = cores[indexCor];
                indexCor = (indexCor + 1) % cores.Length;
                proxTroca += intervaloPiscada;
            }

            // Atualiza escala
            arquiteto.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tempoDecorrido / duracao);

            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        // Fim do efeito: cor preta, escala final, sorting layer ajustada
        sr.color = preto;
        arquiteto.transform.localScale = escalaFinal;
        sr.sortingLayerName = "Trap";
        sr.sortingOrder = 1;

        // Desativa o Animator para não ter mais animação
        if (anim != null)
            anim.enabled = false;
    }


    IEnumerator VitoriaFinal()
    {
        yield return new WaitForSeconds(5f); // tempo de espera antes do fade começar
        yield return StartCoroutine(FadeIn()); // faz o fade
        SceneManager.LoadScene(nomeCenaVitoria); // só troca a cena depois do fade
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }


    public bool TodasEstaoCorrompidas()
    {
        Catraca[] catracas = FindObjectsOfType<Catraca>();
        foreach (Catraca c in catracas)
        {
            if (!c.DeveAtivarBotao())
                return false;
        }
        return true;
    }


}
