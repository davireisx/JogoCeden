using UnityEngine;
using UnityEngine.UI;

public class Botao2 : MonoBehaviour
{
    [Header("Configuração do Botão")]
    public Button interactButton;       // Botão de interação
    public float detectionRadius;       // Raio de detecção da área
    public Transform player;            // Referência ao jogador (Player)
    private bool playerInRange = false; // Flag para detectar se o jogador está na área
    private bool desafioGerador1Concluido = false; // Flag para verificar conclusão do DesafioGerador1

    [Header("Configuração do Jogo")]
    public GameObject weapon;           // Referência ao GameObject da arma
    public DesafioGerador2 desafioGerador2; // Referência ao script do DesafioGerador2
    public SpawnElias spawnManager2;     // Referência ao SpawnManager

    void Start()
    {
        // Inicializa o estado do botão como inativo
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OnButtonClick2); // Adiciona evento ao botão
    }

    void Update()
    {
        // Só ativa o botão se o DesafioGerador1 foi concluído
        if (desafioGerador1Concluido)
        {
            // Verifica a distância entre o jogador e o botão
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= detectionRadius && !playerInRange)
            {
                playerInRange = true;
                interactButton.gameObject.SetActive(true); // Exibe o botão
                Debug.Log("BOTÃO 2 APARECENDO.");
            }
            else if (distanceToPlayer > detectionRadius && playerInRange)
            {
                playerInRange = false;
                interactButton.gameObject.SetActive(false); // Oculta o botão
                Debug.Log("BOTÃO 2 NÃO APARECENDO.");
            }
        }
    }

    public void OnButtonClick2()
    {
        Debug.Log("Botão 2 pressionado! Tentando ativar spawn e definir destino para Gerador 2.");

        if (spawnManager2 != null)
        {
            Debug.Log("spawnManager2 encontrado! Chamando AtivarSpawn...");
            spawnManager2.AtivarSpawn(); // Ativa o spawn
            spawnManager2.SetDestinationToGerador2(); // Define o destino
        }
        else
        {
            Debug.LogError("spawnManager2 não está configurado! Verifique no Inspector.");
        }

        desafioGerador2.IniciarDesafio();
        interactButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void DesafioGerador1Concluido()
    {
        // Método chamado pelo DesafioGerador1 para sinalizar a conclusão
        desafioGerador1Concluido = true;
        Debug.Log("DesafioGerador1 concluído! Botão 2 pode aparecer.");
    }

    private void OnDrawGizmos()
    {
        // Desenha o raio de detecção no editor para visualização
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
