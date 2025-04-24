using UnityEngine;

public class LayerOrderController : MonoBehaviour
{
    [Header("Configura��o dos Objetos")]
    public Transform objeto1; // Refer�ncia ao transform do personagem
    public Transform objeto2; // Refer�ncia ao transform do inimigo

    private SpriteRenderer objeto1Renderer; // SpriteRenderer do personagem
    private SpriteRenderer objeto2Renderer; // SpriteRenderer do inimigo

    void Start()
    {
        // Obt�m os componentes SpriteRenderer do personagem e do inimigo
        objeto1Renderer = objeto1.GetComponent<SpriteRenderer>();
        objeto2Renderer = objeto2.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Corrigido: Compara as posi��es Y para ajustar a ordem das layers
        if (objeto1.position.y > objeto2.position.y)
        {
            // Personagem est� na frente do inimigo
            objeto1Renderer.sortingLayerName = "Personagem"; // Personagem na frente
            objeto2Renderer.sortingLayerName = "Objetos";   // Inimigo atr�s
        }
        else
        {
            // Inimigo est� na frente do personagem
            objeto1Renderer.sortingLayerName = "Objetos";   // Personagem atr�s
            objeto2Renderer.sortingLayerName = "Personagem"; // Inimigo na frente
        }
    }
}
