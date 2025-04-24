using UnityEngine;

public class LayerOrderController : MonoBehaviour
{
    [Header("Configuração dos Objetos")]
    public Transform objeto1; // Referência ao transform do personagem
    public Transform objeto2; // Referência ao transform do inimigo

    private SpriteRenderer objeto1Renderer; // SpriteRenderer do personagem
    private SpriteRenderer objeto2Renderer; // SpriteRenderer do inimigo

    void Start()
    {
        // Obtém os componentes SpriteRenderer do personagem e do inimigo
        objeto1Renderer = objeto1.GetComponent<SpriteRenderer>();
        objeto2Renderer = objeto2.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Corrigido: Compara as posições Y para ajustar a ordem das layers
        if (objeto1.position.y > objeto2.position.y)
        {
            // Personagem está na frente do inimigo
            objeto1Renderer.sortingLayerName = "Personagem"; // Personagem na frente
            objeto2Renderer.sortingLayerName = "Objetos";   // Inimigo atrás
        }
        else
        {
            // Inimigo está na frente do personagem
            objeto1Renderer.sortingLayerName = "Objetos";   // Personagem atrás
            objeto2Renderer.sortingLayerName = "Personagem"; // Inimigo na frente
        }
    }
}
