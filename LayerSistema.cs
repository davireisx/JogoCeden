using UnityEngine;

public class LayerSistema : MonoBehaviour
{
    [Header("Configura��o de Layers")]
    public string layerFrente = "Personagem";
    public string layerAtras = "Objetos";
    public float offsetY = 0.1f; // Ajuste fino para diferen�as de altura

    [Header("Refer�ncias")]
    public Transform personagemPivot; // Objeto vazio nos p�s do personagem
    public Transform objetoPivot;     // Objeto vazio nos p�s do item/inimigo
    public SpriteRenderer personagemRenderer;
    public SpriteRenderer objetoRenderer;

    void Update()
    {
        // Calcula a diferen�a de posi��o Y considerando o piv�
        float diffY = personagemPivot.position.y - objetoPivot.position.y;

        // Aplica o offset para ajuste fino
        if (Mathf.Abs(diffY) < offsetY)
        {
            // Se estiverem muito pr�ximos, usa uma camada padr�o
            personagemRenderer.sortingLayerName = layerFrente;
            objetoRenderer.sortingLayerName = layerAtras;
        }
        else if (diffY > 0)
        {
            // Personagem est� na frente
            personagemRenderer.sortingLayerName = layerFrente;
            objetoRenderer.sortingLayerName = layerAtras;
        }
        else
        {
            // Objeto est� na frente
            personagemRenderer.sortingLayerName = layerAtras;
            objetoRenderer.sortingLayerName = layerFrente;
        }
    }
}