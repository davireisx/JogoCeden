using UnityEngine;
using System.Collections;

public class QueroQuero : MonoBehaviour
{
    [Header("Configura��o do Quero-Quero")]
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
    private bool primeiroAtaqueDisparado = false; // Novo: controla se o primeiro ataque j� foi
    private bool animacaoHitAtivada = false; // Novo: controla se a anima��o de "Hit" j� foi ativada


    private void Update()
    {
        // S� inicia o ataque sequencial se puder atacar o jogador, n�o foi atingido e j� fez o primeiro ataque
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
            // Primeiro ataque instant�neo
            PrimeiroAtaque();
        }
        else if (primeiroAtaqueDisparado && !atacando)
        {
            // Se j� fez o primeiro ataque e n�o est� atacando, pode come�ar nova sequ�ncia
            Debug.Log("Jogador atirou novamente - pode iniciar nova sequ�ncia");
        }
    }

    // Primeiro ataque instant�neo
    private void PrimeiroAtaque()
    {
        if (animacaoHitAtivada) return; // Evita que a anima��o de Hit seja tocada novamente

        esperandoPrimeiroTiro = false;
        primeiroAtaqueDisparado = true;
        animacaoHitAtivada = true; // Marca que a anima��o foi ativada

        // Dispara imediatamente sem anima��o de pr�-ataque
        DispararRaio();
        Debug.Log("Primeiro tiro disparado instantaneamente!");
    }


    // Chamado quando o raio atinge o jogador
    public void JogadorAtingido()
    {
        jogadorFoiAtingido = true;
        podeAtacarJogador = false;
        Debug.Log("Jogador atingido - parando ataques at� pr�ximo tiro");
    }

    private IEnumerator AtaqueSequencial()
    {
        atacando = true;

        Debug.Log("Esperando para iniciar sequ�ncia de ataques...");
        yield return new WaitForSeconds(intervaloAtaqueInicial);

        // S� ativa anima��o de pr�-ataque para sequ�ncia, n�o para o primeiro ataque
        if (!primeiroAtaqueDisparado)
        {
            animator.SetTrigger("PreAtaque");
            Debug.Log("Trigger PreAtaque ativado antes da sequ�ncia de ataques.");
            yield return new WaitForSeconds(tempoAlerta);
        }

        animator.SetTrigger("Atacar");
        Debug.Log("Trigger Atacar ativado!");

        for (int i = 0; i < 3; i++)
        {
            // Se o jogador foi atingido ou n�o pode ser atacado, interrompe
            if (jogadorFoiAtingido || !podeAtacarJogador) break;

            DispararRaio();
            Debug.Log($"Tiro {i + 1} da sequ�ncia disparado!");
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