using UnityEngine;
using System.Collections;

public class RaioQueroQuero : MonoBehaviour
{
    [Header("Configura��es do Raio")]
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float duracaoVida = 1f;
    [SerializeField] private float tempoPararMovimento = 0.5f;

    private Transform alvo;
    private Vector2 direcao;
    private bool pararMovimento = false;
    private bool atingiuAlvo = false;
    private Animator animator;
    private QueroQuero queroQuero;
    private Collider2D collider2D;

    public void SetQueroQuero(QueroQuero qq)
    {
        queroQuero = qq;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();

        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        if (jogador != null)
        {
            alvo = jogador.transform;
        }

        Invoke("PararMovimento", tempoPararMovimento);
        Destroy(gameObject, duracaoVida); // Destrui��o ap�s o tempo de vida
    }

    private void Update()
    {
        if (!pararMovimento && alvo != null && !atingiuAlvo)
        {
            direcao = (alvo.position - transform.position).normalized;
            transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);

            float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void PararMovimento()
    {
        pararMovimento = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (atingiuAlvo) return; // Evita m�ltiplos hits

        if (other.CompareTag("Player"))
        {
            AtingirJogador(other);
        }
        else if (other.CompareTag("Bullet"))
        {
            AtingirProjetil(other);
        }
    }

    private void AtingirJogador(Collider2D jogador)
    {
        atingiuAlvo = true;

        if (queroQuero != null)
        {
            queroQuero.JogadorAtingido();
        }

        ArmaCalebe arma = jogador.GetComponentInChildren<ArmaCalebe>();
        if (arma != null)
        {
            arma.DesativarEReativarElementos(3f); // Desativa os elementos do jogador e agenda a reativa��o
        }

        TriggerHitAnimation();
    }

    private void AtingirProjetil(Collider2D projetil)
    {
        atingiuAlvo = true;

        // Inicia a anima��o de "Hit"
        TriggerHitAnimation();

        // Aguarda at� o final da anima��o para destruir os proj�teis
        StartCoroutine(DestruirAposHit(projetil.gameObject));
    }

    private void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit"); // Toca a anima��o de "Hit"
            Debug.Log("Anima��o 'Hit' ativada.");
        }

        // Desativa o collider para evitar m�ltiplos hits
        if (collider2D != null)
        {
            collider2D.enabled = false;
        }
    }

    private IEnumerator DestruirAposHit(GameObject projetil)
    {
        if (animator != null)
        {
            // Obt�m o tempo de dura��o do estado de anima��o "Hit"
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animDuration = stateInfo.length > 0 ? stateInfo.length : 0.2f;

            // Aguarda a anima��o de "Hit" ser conclu�da
            yield return new WaitForSeconds(animDuration);
        }
        else
        {
            // Caso n�o haja anima��o, aplica um tempo padr�o
            yield return new WaitForSeconds(0.2f);
        }

        // Destroi a bala do Quero-Quero e a do jogador
        Destroy(projetil);
        Destroy(gameObject);
    }
}
