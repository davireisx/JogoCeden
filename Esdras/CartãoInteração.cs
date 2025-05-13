using UnityEngine;
using UnityEngine.UI;

public class CartãoInteração : MonoBehaviour
{
    public float interactionRange = 0;

    public GameObject interactionButton;       // Botão de interação
    public Transform player;                   // Referência ao jogador

    public GameObject objetoParaDesativar1;    // Primeiro objeto a ser desativado
    public GameObject objetoParaDesativar2;    // Segundo objeto a ser desativado
    public GameObject cartaoAtiva;
    public GameObject cartaoDesativa;

    private bool foiClicado = false;           // Flag para saber se já clicou

    private void Start()
    {
        interactionButton.SetActive(false); // Esconde o botão no início

        // Adiciona o evento do botão
        Button btn = interactionButton.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(AoClicarBotao);
        }
    }

    private void Update()
    {
        if (player == null || foiClicado) return; // Se já clicou, não faz mais nada

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionRange)
        {
            interactionButton.SetActive(true);
        }
        else
        {
            interactionButton.SetActive(false);
        }
    }

    private void AoClicarBotao()
    {
        interactionButton.SetActive(false);    // Esconde o botão
        interactionRange = 0f;                 // Zera o alcance
        foiClicado = true;                     // Marca como clicado
        objetoParaDesativar1.SetActive(false);
        objetoParaDesativar2.SetActive(false);
        cartaoAtiva.SetActive(true);
        cartaoDesativa.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 size = new Vector3(interactionRange * 2, interactionRange * 2, 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
