using UnityEngine;
using UnityEngine.UI;

public class CartãoInteração : MonoBehaviour
{
    public GameObject interactionButton; // Botão de interação
    public float interactionRange = 2f;  // Distância máxima para interagir
    public Transform player;             // Referência ao jogador

    private bool foiClicado = false;      // Flag para saber se já clicou

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
        interactionButton.SetActive(false); // Esconde o botão
        interactionRange = 0f;              // Zera o alcance
        foiClicado = true;                  // Marca como clicado
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 size = new Vector3(interactionRange * 2, interactionRange * 2, 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
