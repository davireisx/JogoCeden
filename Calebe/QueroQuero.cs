using UnityEngine;
using System.Collections;

public class QueroQuero : MonoBehaviour
{
    [Header("Configuração do Quero-Quero")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject raioPrefab;
    [SerializeField] private Transform spawnRaio;
    [SerializeField] private float intervaloEntreTiros = 2f;
    [SerializeField] private float intervaloAtaqueInicial = 2f;
    [SerializeField] private float tempoAlerta = 1f;

    private bool esperandoPrimeiroTiro = true;
    private bool atacando = false;
    private bool podeAtacarJogador = true;
    private bool jogadorFoiAtingido = false;
    private bool primeiroAtaqueDisparado = false; // Novo: controla se o primeiro ataque já foi
    private bool animacaoHitAtivada = false; // Novo: controla se a animação de "Hit" já foi ativada


    private void Update()
    {
        // Só inicia o ataque sequencial se puder atacar o jogador, não foi atingido e já fez o primeiro ataque
        if (!atacando && !esperandoPrimeiroTiro && podeAtacarJogador && !jogadorFoiAtingido && primeiroAtaqueDisparado)
        {
            Debug.Log("Iniciando ataque sequencial...");
            StartCoroutine(AtaqueSequencial());
        }
    }

    public void JogadorAtirou()
    {
        // Quando o jogador atira, reseta o estado de atingido
        jogadorFoiAtingido = false;
        podeAtacarJogador = true;

        if (esperandoPrimeiroTiro)
        {
            Debug.Log("Reagindo ao primeiro tiro do jogador...");
            // Primeiro ataque instantâneo
            PrimeiroAtaque();
        }
        else if (primeiroAtaqueDisparado && !atacando)
        {
            // Se já fez o primeiro ataque e não está atacando, pode começar nova sequência
            Debug.Log("Jogador atirou novamente - pode iniciar nova sequência");
        }
    }

    // Primeiro ataque instantâneo
    private void PrimeiroAtaque()
    {
        if (animacaoHitAtivada) return; // Evita que a animação de Hit seja tocada novamente

        esperandoPrimeiroTiro = false;
        primeiroAtaqueDisparado = true;
        animacaoHitAtivada = true; // Marca que a animação foi ativada

        // Dispara imediatamente sem animação de pré-ataque
        DispararRaio();
        Debug.Log("Primeiro tiro disparado instantaneamente!");
    }


    // Chamado quando o raio atinge o jogador
    public void JogadorAtingido()
    {
        jogadorFoiAtingido = true;
        podeAtacarJogador = false;
        Debug.Log("Jogador atingido - parando ataques até próximo tiro");
    }

    private IEnumerator AtaqueSequencial()
    {
        atacando = true;

        Debug.Log("Esperando para iniciar sequência de ataques...");
        yield return new WaitForSeconds(intervaloAtaqueInicial);

        // Só ativa animação de pré-ataque para sequência, não para o primeiro ataque
        if (!primeiroAtaqueDisparado)
        {
            animator.SetTrigger("PreAtaque");
            Debug.Log("Trigger PreAtaque ativado antes da sequência de ataques.");
            yield return new WaitForSeconds(tempoAlerta);
        }

        animator.SetTrigger("Atacar");
        Debug.Log("Trigger Atacar ativado!");

        for (int i = 0; i < 3; i++)
        {
            // Se o jogador foi atingido ou não pode ser atacado, interrompe
            if (jogadorFoiAtingido || !podeAtacarJogador) break;

            DispararRaio();
            Debug.Log($"Tiro {i + 1} da sequência disparado!");
            yield return new WaitForSeconds(intervaloEntreTiros);
        }

        animator.SetTrigger("Idle");
        Debug.Log("Voltando para o estado Idle.");
        atacando = false;
    }


    private void DispararRaio()
    {
        if (raioPrefab != null && spawnRaio != null)
        {
            GameObject raio = Instantiate(raioPrefab, spawnRaio.position, Quaternion.identity);
            raio.GetComponent<RaioQueroQuero>().SetQueroQuero(this);
        }
    }
}