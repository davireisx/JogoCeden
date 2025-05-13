using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueControl : MonoBehaviour
{
    [Header("Components")]
    public GameObject dialogueObj;  // Objeto que cont�m a UI do di�logo
    public Image profile;           // Imagem do perfil do NPC
    public Text speechText;         // Texto onde a fala ser� exibida
    public Text actorNameText;      // Texto que exibe o nome do NPC

    [Header("Settings")]
    public float typingSpeed;       // Velocidade de digita��o das letras
    private string[] sentences;     // Array de frases do di�logo
    private int index;              // �ndice da frase atual

    public Button nextButton;   // Refer�ncia ao bot�o de intera��o para esconder ap�s o di�logo
    public delegate void DialogueEndHandler();
    public event DialogueEndHandler OnDialogueEnd;


    private bool isSecondDialogue = false; // Flag para verificar se o segundo di�logo foi ativado

    // M�todo para iniciar o di�logo
    public void StartDialogue(Sprite p, string[] txt, string actorName, bool isSecondDialogue = false)

    {
        dialogueObj.SetActive(true); // Ativa o objeto de di�logo

        profile.sprite = p; // Atualiza a imagem do perfil
        this.isSecondDialogue = isSecondDialogue; // Define se � o segundo di�logo
        actorNameText.text = actorName; // Define o nome do NPC

        sentences = txt; // Define o conjunto de falas
        index = 0; // Reinicia o �ndice para a primeira frase
        speechText.text = ""; // Limpa o texto da fala

        StopAllCoroutines(); // Para qualquer digita��o em andamento
        StartCoroutine(TypeSentence()); // Inicia a anima��o do novo di�logo

        // Verifica se � o segundo di�logo e ativa o bot�o de "Next Sentence"
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true); // Garante que o bot�o "Next Sentence" sempre apare�a
        }

    }

    // Corrotina para exibir a frase letra por letra
    IEnumerator TypeSentence()
    {
        foreach (char letter in sentences[index].ToCharArray()) // Itera sobre cada caractere da frase
        {
            speechText.text += letter; // Adiciona o caractere ao texto da fala
            yield return new WaitForSeconds(typingSpeed); // Aguarda um tempo baseado na velocidade de digita��o
        }
    }

    // M�todo para avan�ar para a pr�xima frase ou fechar o di�logo
    public void NextSentence()
    {
        // Verifica se o texto atual foi completamente exibido
        if (speechText.text == sentences[index])
        {
            // Se houver mais frases, avan�a
            if (index < sentences.Length - 1)
            {
                index++; // Avan�a para a pr�xima frase
                speechText.text = ""; // Limpa o texto da fala
                StartCoroutine(TypeSentence()); // Inicia a anima��o da nova frase
            }
            else // Se n�o houver mais frases
            {
                // Limpa o texto da fala
                speechText.text = "";
                index = 0; // Reseta o �ndice

                dialogueObj.SetActive(false); // Fecha o di�logo

                if (nextButton != null)  // Verifica se o bot�o de intera��o existe
                {
                   nextButton.gameObject.SetActive(false); // Esconde o bot�o de intera��o
                }

                OnDialogueEnd?.Invoke(); // Dispara o evento de fim de di�logo
            }

        }
    }
}
