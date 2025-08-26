using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class WireDragComLimite : MonoBehaviour
{
    [Header("Referências")]
    public Transform pontoFixo;
    public Transform holderVisual;
    public SpriteRenderer parteVisual;
    public Transform pontaFinal;

    [Header("Configurações")]
    public float distanciaMaxima = 3f;
    public float anguloMax = 60f;
    public float sensibilidade = 0.7f;

    [Header("Destinos de Conexão")]
    public Collider2D destino1;
    public Collider2D destino2;
    public Collider2D destino3;
    public Collider2D destino4;
    public Collider2D destinoCorreto;

    [Header("Luz/Audio")]
    public GameObject luz;
    public AudioSource audioFio;

    private Camera cam;
    private bool foiConectadoAutomaticamente = false;
    private bool estaArrastando = false;
    private bool conectado = false;
    private bool conectadoNoErrado = false;
    private bool conexaoTravada = false;
    private float tamanhoInicial;
    private Collider2D destinoAtual = null;
    private Collider2D meuCollider;

    public static WireDragComLimite fioSendoArrastado = null;

    private static readonly Dictionary<Collider2D, bool> destinosOcupados = new Dictionary<Collider2D, bool>();

    void Awake()
    {
        cam = Camera.main;
        tamanhoInicial = parteVisual.size.x;
        meuCollider = GetComponent<Collider2D>();

        Collider2D[] todosDestinos = { destino1, destino2, destino3, destino4, destinoCorreto };
        foreach (var d in todosDestinos)
        {
            if (d != null && !destinosOcupados.ContainsKey(d) && !d.CompareTag("NaoToca"))
                destinosOcupados[d] = false;
        }
    }

    void Update()
    {
        if (conexaoTravada) return;

        Vector3 inputMundo = Vector3.zero;
        bool pressionando = false;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            inputMundo = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            pressionando = true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            inputMundo = cam.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            pressionando = true;
        }

        inputMundo.z = 0f;

        // Verifica se já há um fio sendo arrastado (impede múltiplos arrastes)
        if (fioSendoArrastado != null && fioSendoArrastado != this && pressionando)
        {
            return;
        }

        if (pressionando && !estaArrastando && (conectado || conectadoNoErrado) && !foiConectadoAutomaticamente)
        {
            Collider2D hit = Physics2D.OverlapPoint(inputMundo);
            if (hit != null && hit.transform.IsChildOf(transform))
            {
                LiberarDestinoAtual();
                estaArrastando = true;
                conectado = false;
                conectadoNoErrado = false;
                fioSendoArrastado = this;
                if (luz != null) luz.SetActive(false);
            }
        }


        if (pressionando)
        {
            if (!estaArrastando && !conectado && !conectadoNoErrado)
            {
                Collider2D hit = Physics2D.OverlapPoint(inputMundo);
                if (hit != null && hit.transform.IsChildOf(transform))
                {
                    if (fioSendoArrastado == null)
                    {
                        estaArrastando = true;
                        fioSendoArrastado = this;
                    }
                }
            }

            if (estaArrastando)
            {
                Vector3 dir = inputMundo - pontoFixo.position;

                if (dir.x <= 0 && parteVisual.size.x < 2f)
                {
                    AtualizarVisual(Vector3.right, parteVisual.size.x);
                    return;
                }

                float angulo = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
                angulo = Mathf.Clamp(angulo, -anguloMax, anguloMax);

                Vector3 dirLimitado = Quaternion.Euler(0, 0, angulo) * Vector3.right;
                float distanciaTotal = Vector3.Distance(pontoFixo.position, inputMundo) * sensibilidade;
                float distancia = Mathf.Clamp(distanciaTotal, 2f, distanciaMaxima);

                AtualizarVisual(dirLimitado, distancia);

                // Desativa a colisão temporariamente enquanto arrasta
                if (meuCollider != null)
                {
                    meuCollider.enabled = false;
                }
            }
        }
        else if (estaArrastando)
        {
            estaArrastando = false;
            fioSendoArrastado = null;

            // Reativa a colisão ao soltar
            if (meuCollider != null)
            {
                meuCollider.enabled = true;
            }

            // ?? Se existirem fios e uma luz na cena, desligar ao soltar
            FiacaoRoboInvertido fiacao = FindObjectOfType<FiacaoRoboInvertido>();
            if (fiacao != null && fiacao.lightObject != null)
                fiacao.lightObject.SetActive(false);

            if (pontaFinal == null) return;

            if (destinoCorreto != null &&
                destinoCorreto.OverlapPoint(pontaFinal.position) &&
                !destinosOcupados[destinoCorreto] &&
                !ColidiuComOutroFio() &&
                !destinoCorreto.CompareTag("NaoToca"))
            {
                ConectarNoDestino(destinoCorreto);
                return;
            }

            Collider2D[] destinos = { destino1, destino2, destino3, destino4 };
            foreach (var destino in destinos)
            {
                if (destino == null || destino == destinoCorreto || destinosOcupados.ContainsKey(destino) && destinosOcupados[destino]) continue;
                if (destino.CompareTag("NaoToca")) continue;

                if (destino.OverlapPoint(pontaFinal.position) && !ColidiuComOutroFio())
                {
                    ConectarNoDestino(destino);
                    return;
                }
            }

            StartCoroutine(VoltarParaOrigem());
        }
    }

    void AtualizarVisual(Vector3 direcao, float comprimento)
    {
        float anguloZ = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        holderVisual.eulerAngles = new Vector3(0f, 0f, anguloZ);
        parteVisual.size = new Vector2(comprimento, parteVisual.size.y);

        if (pontaFinal != null)
            pontaFinal.localPosition = new Vector3(parteVisual.size.x, 0f, 0f);
    }

    IEnumerator VoltarParaOrigem()
    {
        LiberarDestinoAtual();

        float tempo = 0f;
        float duracao = 0.25f;
        float tamanhoAtual = parteVisual.size.x;

        while (tempo < 1f)
        {
            tempo += Time.deltaTime / duracao;
            float novoTamanho = Mathf.Lerp(tamanhoAtual, tamanhoInicial, tempo);
            parteVisual.size = new Vector2(novoTamanho, parteVisual.size.y);
            if (pontaFinal != null)
                pontaFinal.localPosition = new Vector3(parteVisual.size.x, 0f, 0f);
            yield return null;
        }

        parteVisual.size = new Vector2(tamanhoInicial, parteVisual.size.y);
        holderVisual.rotation = Quaternion.identity;
        if (pontaFinal != null)
            pontaFinal.localPosition = new Vector3(tamanhoInicial, 0f, 0f);
    }

    void ConectarNoDestino(Collider2D destino)
    {
        if (destino.CompareTag("NaoToca")) return;

        destinoAtual = destino;

        if (destino == destinoCorreto)
        {
            conectado = true;
            if (luz != null) luz.SetActive(true);
        }
        else
        {
            conectadoNoErrado = true;
        }

        destinosOcupados[destino] = true;

        audioFio.Play();
    }

    void LiberarDestinoAtual()
    {
        if (destinoAtual != null && destinosOcupados.ContainsKey(destinoAtual))
        {
            destinosOcupados[destinoAtual] = false;
            destinoAtual = null;
        }

        conectadoNoErrado = false;
        conectado = false;
    }

    bool ColidiuComOutroFio()
    {
        if (pontaFinal == null) return false;

        // Cria um pequeno círculo de verificação para evitar colisões
        Collider2D[] hits = Physics2D.OverlapCircleAll(pontaFinal.position, 0.1f);
        foreach (var hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject && hit.GetComponent<WireDragComLimite>() != null)
            {
                return true;
            }
        }
        return false;
    }

    public void ConectarAutomaticamente(Transform destino)
    {
        Collider2D destinoCol = destino.GetComponent<Collider2D>();
        if (destinoCol == null) return;
        if (destinoCol.CompareTag("NaoToca")) return;
        if (destinosOcupados.ContainsKey(destinoCol) && destinosOcupados[destinoCol]) return;

        if (pontaFinal != null)
        {
            pontaFinal.position = destino.position;
            SincronizarVisualComPontaFinal();
        }

        destinoAtual = destinoCol;

        if (destinoCol == destinoCorreto)
        {
            conectado = true;
            if (luz != null) luz.SetActive(true);
        }
        else
        {
            conectadoNoErrado = true;
        }

        destinosOcupados[destinoCol] = true;

        // ?? marca que foi automático
        foiConectadoAutomaticamente = true;
    }

    public void SincronizarVisualComPontaFinal()
    {
        if (pontaFinal == null || parteVisual == null || holderVisual == null) return;

        Vector3 diferenca = holderVisual.InverseTransformPoint(pontaFinal.position);
        float comprimento = Mathf.Max(diferenca.x, 0.1f);

        parteVisual.size = new Vector2(comprimento, parteVisual.size.y);
        pontaFinal.localPosition = new Vector3(comprimento, 0, 0);
    }

    public void TravarConexao()
    {
        conexaoTravada = true;
    }

    public bool GetEstaConectado()
    {
        return conectado || conectadoNoErrado;
    }

    public bool EstaNoDestinoCorreto()
    {
        return conectado;
    }
}