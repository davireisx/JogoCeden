using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueControl : MonoBehaviour
{
    [Header("Components")]
    public GameObject dialogueObj;  // Objeto que contém a UI do diálogo
    public Image profile;           // Imagem do perfil do NPC
    public Text speechText;         // Texto onde a fala será exibida
    public Text actorNameText;      // Texto que exibe o nome do NPC

    [Header("Settings")]
    public float typingSpeed;       // Velocidade de digitação das letras
    private string[] sentences;     // Array de frases do diálogo
    private int index;              // Índice da frase atual

    public Button nextButton;   // Referência ao botão de interação para esconder após o diálogo
    public delegate void DialogueEndHandler();
    public event DialogueEndHandler OnDialogueEnd;


    private bool isSecondDialogue = false; // Flag para verificar se o segundo diálogo foi ativado

    // Método para iniciar o diálogo
    public void StartDialogue(Sprite p, string[] txt, string actorName, bool isSecondDialogue = false)

    {
        dialogueObj.SetActive(true); // Ativa o objeto de diálogo

        profile.sprite = p; // Atualiza a imagem do perfil
        this.isSecondDialogue = isSecondDialogue; // Define se é o segundo diálogo
        actorNameText.text = actorName; // Define o nome do NPC

        sentences = txt; // Define o conjunto de falas
        index = 0; // Reinicia o índice para a primeira frase
        speechText.text = ""; // Limpa o texto da fala

        StopAllCoroutines(); // Para qualquer digitação em andamento
        StartCoroutine(TypeSentence()); // Inicia a animação do novo diálogo

        // Verifica se é o segundo diálogo e ativa o botão de "Next Sentence"
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true); // Garante que o botão "Next Sentence" sempre apareça
        }

    }

    // Corrotina para exibir a frase letra por letra
    IEnumerator TypeSentence()
    {
        foreach (char letter in sentences[index].ToCharArray()) // Itera sobre cada caractere da frase
        {
            speechText.text += letter; // Adiciona o caractere ao texto da fala
            yield return new WaitForSeconds(typingSpeed); // Aguarda um tempo baseado na velocidade de digitação
        }
    }

    // Método para avançar para a próxima frase ou fechar o diálogo
    public void NextSentence()
    {
        // Verifica se o texto atual foi completamente exibido
        if (speechText.text == sentences[index])
        {
            // Se houver mais frases, avança
            if (index < sentences.Length - 1)
            {
                index++; // Avança para a próxima frase
                speechText.text = ""; // Limpa o texto da fala
                StartCoroutine(TypeSentence()); // Inicia a animação da nova frase
            }
            else // Se não houver mais frases
            {
                // Limpa o texto da fala
                speechText.text = "";
                index = 0; // Reseta o índice

                dialogueObj.SetActive(false); // Fecha o diálogo

                if (nextButton != null)  // Verifica se o botão de interação existe
                {
                   nextButton.gameObject.SetActive(false); // Esconde o botão de interação
                }

                OnDialogueEnd?.Invoke(); // Dispara o evento de fim de diálogo
            }

        }
    }
}
