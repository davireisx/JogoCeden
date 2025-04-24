using UnityEngine;
using UnityEngine.UI; // Necessário para trabalhar com UI

public class Roteador : MonoBehaviour
{
    [Header("Configuração do Roteador")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Referência ao SpriteRenderer do Roteador
    [SerializeField] private int metaCarga = 30; // Quantidade de carga necessária para ativar o roteador
    [SerializeField] private Text textoCargaUI; // Referência ao elemento de texto da UI para exibir a carga

    private int cargaAtual = 0; // Carga acumulada atual

    private void Awake()
    {
        // Certifique-se de que o SpriteRenderer está atribuído
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Define a cor inicial como preto
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.black;
        }

        // Atualiza o texto inicial da UI
        AtualizarTextoCargaUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu possui o script "BalaEffect"
        BalaEffect bala = other.GetComponent<BalaEffect>();
        if (bala != null)
        {
            // Adiciona a carga ao roteador
            AdicionarCarga(bala.GetCarga());
            Debug.Log($"Bala colidiu com o roteador e adicionou {bala.GetCarga()} de carga.");
        }
    }

    public void AdicionarCarga(int carga)
    {
        cargaAtual += carga;

        // Garante que a carga atual não exceda o valor máximo
        if (cargaAtual > metaCarga)
        {
            cargaAtual = metaCarga;
        }

        Debug.Log($"Carga Atual: {cargaAtual}/{metaCarga}");

        // Atualiza o texto da UI
        AtualizarTextoCargaUI();

        // Verifica se a carga atingiu ou excedeu a meta
        if (cargaAtual >= metaCarga)
        {
            AtivarRoteador(); // Ativa o roteador
        }
    }

    private void AtivarRoteador()
    {
        // Lógica para ativar o roteador
        Debug.Log("Roteador Ativado!");

        // Altera a cor para branco como indicação visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; // Muda a cor do SpriteRenderer para branco
        }
    }

    private void AtualizarTextoCargaUI()
    {
        if (textoCargaUI != null)
        {
            // Atualiza o texto com a carga atual e a meta
            textoCargaUI.text = $"Carga: {cargaAtual}/{metaCarga}";

            // Se a carga atingir o limite máximo, muda a cor do texto para verde
            if (cargaAtual >= metaCarga)
            {
                textoCargaUI.color = Color.green;
            }
            else
            {
                textoCargaUI.color = Color.white; // Cor padrão quando ainda não atingiu o limite
            }
        }
        else
        {
            Debug.LogError("Texto UI não atribuído no Inspector!");
        }
    }
}
