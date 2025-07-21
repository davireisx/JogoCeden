using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Catraca : MonoBehaviour
{
    public enum Dificuldade { Facil, Medio, Dificil }

    [Header("Dificuldade da catraca")]
    public Dificuldade dificuldade;
    private CatracaManager manager;

    [Header("Texto de confirma��o")]
    [SerializeField] private GameObject textoCheck;


    [Header("Configura��es B�sicas")]
    [SerializeField] private Color corNormal;
    [SerializeField] private Color corCorrompida;

    [Header("Cores de Vit�ria")]
    [SerializeField] private Color corPiscadaVenceu;
    [SerializeField] private Color corFinalVenceu;

    [Header("Cores de Derrota")]
    [SerializeField] private Color corPiscadaDerrota;
    [SerializeField] private Color corFinalDerrota;

    [Header("Configura��es de Efeito")]
    [SerializeField] private float duracaoPiscar = 3f;
    [SerializeField] private float intervaloPiscada = 0.2f;

    [Header("Cores que Ativam Bot�o")]
    [SerializeField]
    private List<Color> coresQueAtivamBotao = new List<Color>()
    {
        new Color(29f/255f, 173f/255f, 105f/255f), // 1DAD69
        new Color(200f/255f, 61f/255f, 42f/255f),  // C83D2A
        new Color(202f/255f, 192f/255f, 30f/255f)  // CAC01E
    };

    private SpriteRenderer spriteRenderer;
    private Color corAtual;
    public bool ficouCorrompidaAgora { get; set; }
    private Coroutine efeitoDeCor;
    public Color CurrentColor => corAtual;

    private void Awake()
    {
        // Certifica-se de que o componente SpriteRenderer est� presente no objeto
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer n�o encontrado na catraca! Verifique se o objeto possui este componente.");
            return;
        }

        // Inicializa
        corAtual = corNormal;
        spriteRenderer.color = corAtual;
        ficouCorrompidaAgora = false;

        // IMPORTANTE: encontrar o CatracaManager
        manager = FindObjectOfType<CatracaManager>();
    }


    public void MudarCorFinal(bool venceu)
    {
        // Pare todas as corrotinas para evitar sobreposi��o
        if (efeitoDeCor != null)
        {
            StopCoroutine(efeitoDeCor);
        }

        Color corPiscar = venceu ? corPiscadaVenceu : corPiscadaDerrota;
        Color corFinal = venceu ? corFinalVenceu : corFinalDerrota;

        // Reinicie a cor atual antes de come�ar a piscar
        spriteRenderer.color = corAtual;

        // Inicie a corrotina de piscar (ela mesma vai finalizar com a cor final)
        efeitoDeCor = StartCoroutine(PiscarCor(corPiscar, corFinal));

        // Chama o manager ap�s iniciar o efeito
        if (manager != null)
        {
            if (venceu)
                manager.CatracaResolvida(dificuldade);
            else
                manager.CatracaFalhou(dificuldade);
        }
    }


    private IEnumerator PiscarCor(Color corPiscar, Color corFinal)
    {
        Debug.Log("Iniciando a corrotina de piscar...");
        // Garanta que temos um sprite renderer v�lido
        if (spriteRenderer == null) yield break;

        Color original = spriteRenderer.color;
        float tempoDecorrido = 0f;
        bool estadoPiscado = false;

        // Fase de piscar
        while (tempoDecorrido < duracaoPiscar)
        {
            Debug.Log("Piscando...");

            if (spriteRenderer != null) // Verifica��o adicional de seguran�a
            {
                spriteRenderer.color = estadoPiscado ? corPiscar : original;
                estadoPiscado = !estadoPiscado;
            }

            yield return new WaitForSeconds(intervaloPiscada);
            tempoDecorrido += intervaloPiscada;
        }

        // Fase final - estabiliza na cor
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corFinal;
            corAtual = corFinal;
            Debug.Log("Cor final estabilizada: " + corAtual);
        }
    }

    // Define cor e detecta transi��o normal -> corrompida
    public void SetColor(Color novaCor)
    {
        ficouCorrompidaAgora = CoresIguais(corAtual, corNormal) &&
                               CoresIguais(novaCor, corCorrompida);
        corAtual = novaCor;
        spriteRenderer.color = corAtual;
    }

    public void Consertar()
    {
        SetColor(corNormal);
    }

    public string GetCurrentHex()
    {
        Color32 c = spriteRenderer.color;
        return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
    }

    // Compara��o com toler�ncia
    private bool CoresIguais(Color a, Color b, float tol = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tol &&
               Mathf.Abs(a.g - b.g) < tol &&
               Mathf.Abs(a.b - b.b) < tol;
    }

    // Novo: Verifica se a cor atual ativa o bot�o
    public bool DeveAtivarBotao()
    {
        foreach (Color cor in coresQueAtivamBotao)
        {
            if (CoresIguais(corAtual, cor))
                return true;
        }
        return false;
    }
}
