using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [Header("NPC Config")]
    public string actorName; // Nome do NPC
    public Sprite profile; // Imagem do perfil do NPC
    public string[] speechTxt; // Falas do primeiro di�logo
    public string[] speechTxt2; // Falas do segundo di�logo

    [Header("Intera��o")]
    public LayerMask playerLayer; // Camada do jogador para detectar colis�o
    public float radious; // Raio de intera��o
    public GameObject instrucoes; // Refer�ncia � caixa de instru��ess

    [Header("Bot�es de Intera��o")]
    public Button interactButton; // Bot�o do primeiro di�logo
    public Button secondInteractButton; // Bot�o do segundo di�logo
    public Button changeSceneButton; // Bot�o de mudan�a de cena

    [Header("Estado do Di�logo")]
    public bool isSecondDialogueAvailable = false; // Define se o segundo di�logo pode acontecer
    private bool hasInteracted = false; // Verifica se o jogador j� iniciou o primeiro di�logo
    private bool hasPressedSecondButton = false; // Impede m�ltiplas ativa��es do segundo di�logo
    private bool onRadious = false; // Verifica se o jogador est� dentro do raio de intera��o
    private bool wasOnRadious = false; // Estado anterior do jogador no raio

    private DialogueControl dc; // Refer�ncia ao sistema de controle de di�logo

    private void Start()
    {
        Debug.Log("Iniciando script Dialogue...");

        if (instrucoes == null)
        {
            Debug.LogError("A vari�vel 'instrucoes' n�o foi atribu�da no Inspector no objeto: " + gameObject.name);
        }
        else
        {
            instrucoes.SetActive(false); // Desativa a caixa de instru��es no in�cio
            Debug.Log("Caixa de instru��es desativada no in�cio.");
        }

        if (changeSceneButton == null)
        {
            Debug.LogError("A vari�vel 'changeSceneButton' n�o foi atribu�da no Inspector no objeto: " + gameObject.name);
        }
        else
        {
            changeSceneButton.gameObject.SetActive(false); // Desativa o bot�o de mudan�a de cena no in�cio
            changeSceneButton.onClick.AddListener(OnChangeSceneButtonPressed); // Configura o listener do bot�o
            Debug.Log("Listener do bot�o de mudan�a de cena configurado.");
        }

        dc = Object.FindFirstObjectByType<DialogueControl>(); // Busca o controle de di�logos na cena
        if (dc == null)
        {
            Debug.LogError("DialogueControl n�o encontrado na cena.");
        }

        // Desativa bot�es no in�cio
        interactButton?.gameObject.SetActive(false);
        secondInteractButton?.gameObject.SetActive(false);
        Debug.Log("Bot�es de intera��o desativados no in�cio.");
    }

    private void FixedUpdate()
    {
        Interact(); // Verifica se o jogador est� dentro do raio de intera��o
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

    // ==================== INTERA��O ====================

    private void Interact()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radious, playerLayer);
        onRadious = hit != null;
    }

    private void HandlePlayerEnterRadius()
    {
        Debug.Log("Jogador entrou no raio de intera��o.");

        if (!hasInteracted && interactButton != null && !interactButton.gameObject.activeSelf)
        {
            interactButton.gameObject.SetActive(true);
            Debug.Log("Bot�o de intera��o ativado.");
        }

        EnemyMovement enemy = Object.FindFirstObjectByType<EnemyMovement>();
        if (enemy != null && enemy.IsAtLastWaypoint() && !hasPressedSecondButton)
        {
            secondInteractButton?.gameObject.SetActive(true);
            Debug.Log("Bot�o de segundo di�logo ativado.");
        }
    }

    private void HandlePlayerExitRadius()
    {
        Debug.Log("Jogador saiu do raio de intera��o.");

        interactButton?.gameObject.SetActive(false);
        secondInteractButton?.gameObject.SetActive(false);
        Debug.Log("Bot�es de intera��o desativados.");
    }

    // ==================== DI�LOGOS ====================

    public void OnInteractButtonPressed()
    {
        if (!hasInteracted)
        {
            Debug.Log("Iniciando primeiro di�logo...");

            dc.StartDialogue(profile, speechTxt, actorName);
            interactButton?.gameObject.SetActive(false);
            hasInteracted = true;

            Personagem player = Object.FindFirstObjectByType<Personagem>();
            player?.SetDialogueState(true);
            player?.SetIdleFront();

            dc.OnDialogueEnd += OnDialogueEnd;
            Debug.Log("Primeiro di�logo iniciado.");
        }
    }

    public void OnSecondInteractButtonPressed()
    {
        if (isSecondDialogueAvailable && !hasPressedSecondButton)
        {
            Debug.Log("Iniciando segundo di�logo...");
            dc.StartDialogue(profile, speechTxt2, actorName);
            secondInteractButton?.gameObject.SetActive(false);

            Personagem player = Object.FindFirstObjectByType<Personagem>();
            player?.SetDialogueState(true);
            player?.SetIdleFront();

            hasPressedSecondButton = true;
            dc.OnDialogueEnd += OnDialogueEnd;
            Debug.Log("Segundo di�logo iniciado.");
        }
    }

    private void OnDialogueEnd()
    {
        Debug.Log("Di�logo terminou. Personagem pode se mover novamente.");

        Personagem player = Object.FindFirstObjectByType<Personagem>();
        player?.SetDialogueState(false);

        if (hasPressedSecondButton)
        {
            Debug.Log("Iniciando ativa��o da caixa de instru��es e bot�o de mudan�a de cena...");
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
            instrucoes.SetActive(true); // Ativa a caixa de instru��es
            Debug.Log("Caixa de instru��es ativada ap�s 3 segundos.");
        }
    }

    private IEnumerator EnableChangeSceneButtonAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (changeSceneButton != null)
        {
            changeSceneButton.gameObject.SetActive(true); // Ativa o bot�o de mudan�a de cena
            Debug.Log("Bot�o de mudan�a de cena ativado ap�s 3 segundos.");
        }
    }

    public void OnChangeSceneButtonPressed()
    {
        Debug.Log("Bot�o de mudan�a de cena pressionado.");
        if (!string.IsNullOrEmpty("Poder - Esdras"))
        {
            Debug.Log("Tentando carregar a cena Poder: " );
            SceneManager.LoadScene("Poder - Esdras"); // Carrega a cena especificada
        }
        else
        {
            Debug.LogError("O nome da cena n�o foi definido no Inspector.");
        }
    }
}