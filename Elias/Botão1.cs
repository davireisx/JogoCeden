using UnityEngine;
using UnityEngine.UI;

public class Botao1 : MonoBehaviour
{
    [Header("Configura��o do Bot�o")]
    public Button interactButton;       // Bot�o de intera��o
    public float detectionRadius;       // Raio de detec��o da �rea
    public Transform player;            // Refer�ncia ao jogador (Player)
    private bool playerInRange = false; // Flag para detectar se o jogador est� na �rea

    [Header("Configura��o do Jogo")]
    public GameObject weapon;           // Refer�ncia ao GameObject da arma
    public DesafioGerador1 desafioGerador; // Refer�ncia ao script do desafio
    public SpawnElias spawnManager;     // Refer�ncia ao SpawnManager
    public float delayMovimentoInimigos = 2f; // Tempo de atraso para o movimento dos inimigos

    void Start()
    {
        // Inicializa o estado do bot�o e da arma
        interactButton.gameObject.SetActive(false);
        if (weapon != null)
        {
            weapon.SetActive(false); // Garante que a arma esteja desativada no in�cio
        }
    }

    void Update()
    {
        // Verifica a dist�ncia entre o jogador e o bot�o
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= detectionRadius && !playerInRange)
        {
            playerInRange = true;
            interactButton.gameObject.SetActive(true); // Exibe o bot�o
            Debug.Log("BOT�O 1 APARECENDO.");
        }
        else if (distanceToPlayer > detectionRadius && playerInRange)
        {
            playerInRange = false;
            interactButton.gameObject.SetActive(false); // Oculta o bot�o
            Debug.Log("BOT�O 1 N�O APARECENDO.");
        }
    }

    public void OnButtonClick1()
    {
        Debug.Log("Primeiro bot�o pressionado! A��o iniciada.");

        // Ativa o spawn de inimigos
        spawnManager.AtivarSpawn();

        // Define o destino dos inimigos para o Gerador 1
        spawnManager.SetDestinationToGerador1();

        // Inicia o desafio do gerador
        desafioGerador.IniciarDesafio();

        // Ativa a arma, caso exista
        if (weapon != null)
        {
            weapon.SetActive(true);
            Debug.Log("Arma ativada!");
        }

        // Desativa permanentemente o bot�o e seu GameObject
        interactButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        // Desenha o raio de detec��o no editor para visualiza��o
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
