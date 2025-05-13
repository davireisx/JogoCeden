using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [Header("NPC Config")]
    public string actorName; // Nome do NPC
    public Sprite profile; // Imagem do perfil do NPC
    public string[] speechTxt; // Falas do primeiro diálogo
    public string[] speechTxt2; // Falas do segundo diálogo

    [Header("Interação")]
    public LayerMask playerLayer; // Camada do jogador para detectar colisão
    public float radious; // Raio de interação
    public GameObject instrucoes; // Referência à caixa de instruçõess

    [Header("Botões de Interação")]
    public Button interactButton; // Botão do primeiro diálogo
    public Button secondInteractButton; // Botão do segundo diálogo
    public Button changeSceneButton; // Botão de mudança de cena

    [Header("Estado do Diálogo")]
    public bool isSecondDialogueAvailable = false; // Define se o segundo diálogo pode acontecer
    private bool hasInteracted = false; // Verifica se o jogador já iniciou o primeiro diálogo
    private bool hasPressedSecondButton = false; // Impede múltiplas ativações do segundo diálogo
    private bool onRadious = false; // Verifica se o jogador está dentro do raio de interação
    private bool wasOnRadious = false; // Estado anterior do jogador no raio

    private DialogueControl dc; // Referência ao sistema de controle de diálogo

    private void Start()
    {
        Debug.Log("Iniciando script Dialogue...");

        if (instrucoes == null)
        {
            Debug.LogError("A variável 'instrucoes' não foi atribuída no Inspector no objeto: " + gameObject.name);
        }
        else
        {
            instrucoes.SetActive(false); // Desativa a caixa de instruções no início
            Debug.Log("Caixa de instruções desativada no início.");
        }

        if (changeSceneButton == null)
        {
            Debug.LogError("A variável 'changeSceneButton' não foi atribuída no Inspector no objeto: " + gameObject.name);
        }
        else
        {
            changeSceneButton.gameObject.SetActive(false); // Desativa o botão de mudança de cena no início
            changeSceneButton.onClick.AddListener(OnChangeSceneButtonPressed); // Configura o listener do botão
            Debug.Log("Listener do botão de mudança de cena configurado.");
        }

        dc = Object.FindFirstObjectByType<DialogueControl>(); // Busca o controle de diálogos na cena
        if (dc == null)
        {
            Debug.LogError("DialogueControl não encontrado na cena.");
        }

        // Desativa botões no início
        interactButton?.gameObject.SetActive(false);
        secondInteractButton?.gameObject.SetActive(false);
        Debug.Log("Botões de interação desativados no início.");
    }

    private void FixedUpdate()
    {
        Interact(); // Verifica se o jogador está dentro do raio de interação
    }

    private void Update()
    {
        if (onRadious != wasOnRadious)
        {
            if (onRadious)
            {
                HandlePlayerEnterRadius();
            }
            else
            {
                HandlePlayerExitRadius();
            }

            wasOnRadious = onRadious; // Atualiza o estado anterior
        }
    }

    // ==================== INTERAÇÃO ====================

    private void Interact()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radious, playerLayer);
        onRadious = hit != null;
    }

    private void HandlePlayerEnterRadius()
    {
        Debug.Log("Jogador entrou no raio de interação.");

        if (!hasInteracted && interactButton != null && !interactButton.gameObject.activeSelf)
        {
            interactButton.gameObject.SetActive(true);
            Debug.Log("Botão de interação ativado.");
        }

        EnemyMovement enemy = Object.FindFirstObjectByType<EnemyMovement>();
        if (enemy != null && enemy.IsAtLastWaypoint() && !hasPressedSecondButton)
        {
            secondInteractButton?.gameObject.SetActive(true);
            Debug.Log("Botão de segundo diálogo ativado.");
        }
    }

    private void HandlePlayerExitRadius()
    {
        Debug.Log("Jogador saiu do raio de interação.");

        interactButton?.gameObject.SetActive(false);
        secondInteractButton?.gameObject.SetActive(false);
        Debug.Log("Botões de interação desativados.");
    }

    // ==================== DIÁLOGOS ====================

    public void OnInteractButtonPressed()
    {
        if (!hasInteracted)
        {
            Debug.Log("Iniciando primeiro diálogo...");

            dc.StartDialogue(profile, speechTxt, actorName);
            interactButton?.gameObject.SetActive(false);
            hasInteracted = true;

            Personagem player = Object.FindFirstObjectByType<Personagem>();
            player?.SetDialogueState(true);
            player?.SetIdleFront();

            dc.OnDialogueEnd += OnDialogueEnd;
            Debug.Log("Primeiro diálogo iniciado.");
        }
    }

    public void OnSecondInteractButtonPressed()
    {
        if (isSecondDialogueAvailable && !hasPressedSecondButton)
        {
            Debug.Log("Iniciando segundo diálogo...");
            dc.StartDialogue(profile, speechTxt2, actorName);
            secondInteractButton?.gameObject.SetActive(false);

            Personagem player = Object.FindFirstObjectByType<Personagem>();
            player?.SetDialogueState(true);
            player?.SetIdleFront();

            hasPressedSecondButton = true;
            dc.OnDialogueEnd += OnDialogueEnd;
            Debug.Log("Segundo diálogo iniciado.");
        }
    }

    private void OnDialogueEnd()
    {
        Debug.Log("Diálogo terminou. Personagem pode se mover novamente.");

        Personagem player = Object.FindFirstObjectByType<Personagem>();
        player?.SetDialogueState(false);

        if (hasPressedSecondButton)
        {
            Debug.Log("Iniciando ativação da caixa de instruções e botão de mudança de cena...");
            StartCoroutine(EnableInstrucoesAfterDelay());
            StartCoroutine(EnableChangeSceneButtonAfterDelay());
        }
    }

    private IEnumerator EnableInstrucoesAfterDelay()
    {
        Personagem player = Object.FindFirstObjectByType<Personagem>();
        yield return new WaitForSeconds(3f); // Aguarda 3 segundos
        player?.SetDialogueState(true);
        player?.SetIdleFront();

        if (instrucoes != null)
        {
            instrucoes.SetActive(true); // Ativa a caixa de instruções
            Debug.Log("Caixa de instruções ativada após 3 segundos.");
        }
    }

    private IEnumerator EnableChangeSceneButtonAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (changeSceneButton != null)
        {
            changeSceneButton.gameObject.SetActive(true); // Ativa o botão de mudança de cena
            Debug.Log("Botão de mudança de cena ativado após 3 segundos.");
        }
    }

    public void OnChangeSceneButtonPressed()
    {
        Debug.Log("Botão de mudança de cena pressionado.");
        if (!string.IsNullOrEmpty("Poder - Esdras"))
        {
            Debug.Log("Tentando carregar a cena Poder: " );
            SceneManager.LoadScene("Poder - Esdras"); // Carrega a cena especificada
        }
        else
        {
            Debug.LogError("O nome da cena não foi definido no Inspector.");
        }
    }
}