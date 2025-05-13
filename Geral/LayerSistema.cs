using UnityEngine;

public class LayerSistema : MonoBehaviour
{
    [Header("Configuração de Layers")]
    public string layerFrente = "Personagem";
    public string layerAtras = "Objetos";
    public float offsetY = 0.1f; // Ajuste fino para diferenças de altura

    [Header("Referências")]
    public Transform personagemPivot; // Objeto vazio nos pés do personagem
    public Transform objetoPivot;     // Objeto vazio nos pés do item/inimigo
    public SpriteRenderer personagemRenderer;
    public SpriteRenderer objetoRenderer;

    void Update()
    {
        // Calcula a diferença de posição Y considerando o pivô
        float diffY = personagemPivot.position.y - objetoPivot.position.y;

        // Aplica o offset para ajuste fino
        if (Mathf.Abs(diffY) < offsetY)
        {
            // Se estiverem muito próximos, usa uma camada padrão
            personagemRenderer.sortingLayerName = layerFrente;
            objetoRenderer.sortingLayerName = layerAtras;
        }
        else if (diffY > 0)
        {
            // Personagem está na frente
            personagemRenderer.sortingLayerName = layerFrente;
            objetoRenderer.sortingLayerName = layerAtras;
        }
        else
        {
            // Objeto está na frente
            personagemRenderer.sortingLayerName = layerAtras;
            objetoRenderer.sortingLayerName = layerFrente;
        }
    }
}