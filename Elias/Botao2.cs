using UnityEngine;
using UnityEngine.UI;

public class Botao2 : MonoBehaviour
{
    [Header("Configura��o do Bot�o")]
    public Button interactButton;       // Bot�o de intera��o
    public float detectionRadius;       // Raio de detec��o da �rea
    public Transform player;            // Refer�ncia ao jogador (Player)
    private bool playerInRange = false; // Flag para detectar se o jogador est� na �rea
    private bool desafioGerador1Concluido = false; // Flag para verificar conclus�o do DesafioGerador1

    [Header("Configura��o do Jogo")]
    public GameObject weapon;           // Refer�ncia ao GameObject da arma
    public DesafioGerador2 desafioGerador2; // Refer�ncia ao script do DesafioGerador2
    public SpawnElias spawnManager2;     // Refer�ncia ao SpawnManager

    void Start()
    {
        // Inicializa o estado do bot�o como inativo
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OnButtonClick2); // Adiciona evento ao bot�o
    }

    void Update()
    {
        // S� ativa o bot�o se o DesafioGerador1 foi conclu�do
        if (desafioGerador1Concluido)
        {
            // Verifica a dist�ncia entre o jogador e o bot�o
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= detectionRadius && !playerInRange)
            {
                playerInRange = true;
                interactButton.gameObject.SetActive(true); // Exibe o bot�o
                Debug.Log("BOT�O 2 APARECENDO.");
            }
            else if (distanceToPlayer > detectionRadius && playerInRange)
            {
                playerInRange = false;
                interactButton.gameObject.SetActive(false); // Oculta o bot�o
                Debug.Log("BOT�O 2 N�O APARECENDO.");
            }
        }
    }

    public void OnButtonClick2()
    {
        Debug.Log("Bot�o 2 pressionado! Tentando ativar spawn e definir destino para Gerador 2.");

        if (spawnManager2 != null)
        {
            Debug.Log("spawnManager2 encontrado! Chamando AtivarSpawn...");
            spawnManager2.AtivarSpawn(); // Ativa o spawn
            spawnManager2.SetDestinationToGerador2(); // Define o destino
        }
        else
        {
            Debug.LogError("spawnManager2 n�o est� configurado! Verifique no Inspector.");
        }

        desafioGerador2.IniciarDesafio();
        interactButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void DesafioGerador1Concluido()
    {
        // M�todo chamado pelo DesafioGerador1 para sinalizar a conclus�o
        desafioGerador1Concluido = true;
        Debug.Log("DesafioGerador1 conclu�do! Bot�o 2 pode aparecer.");
    }

    private void OnDrawGizmos()
    {
        // Desenha o raio de detec��o no editor para visualiza��o
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
