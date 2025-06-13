using UnityEngine;

public class LayerTeste : MonoBehaviour
{
    [Header("Configura��o de Layers")]
    public string layerFrente = "Personagem";
    public string layerAtras = "Objetos";
    public float offsetY = 0.1f;

    [Header("Refer�ncias")]
    public Transform personagemPivot; // N�O precisa arrastar
    public Transform objetoPivot;     // Este sim, arraste no Inspector
    public SpriteRenderer personagemRenderer;
    public SpriteRenderer objetoRenderer;

    void Start()
    {
        // Se o personagemPivot n�o estiver preenchido no Inspector, busque automaticamente
        if (personagemPivot == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Transform pivot = player.transform.Find("P�");
                if (pivot != null)
                {
                    personagemPivot = pivot;
                }
                else
                {
                    Debug.LogWarning("N�o foi encontrado um filho chamado 'Pivot' dentro do Player.");
                }

                if (personagemRenderer == null)
                {
                    personagemRenderer = player.GetComponent<SpriteRenderer>();
                }
            }
            else
            {
                Debug.LogWarning("Nenhum GameObject com tag 'Player' foi encontrado.");
            }
        }
    }

    void Update()
    {
        if (personagemPivot == null || objetoPivot == null || personagemRenderer == null || objetoRenderer == null)
            return;

        float diffY = personagemPivot.position.y - objetoPivot.position.y;

        if (Mathf.Abs(diffY) < offsetY)
        {
            personagemRenderer.sortingLayerName = layerFrente;
            objetoRenderer.sortingLayerName = layerAtras;
        }
        else if (diffY > 0)
        {
            personagemRenderer.sortingLayerName = layerFrente;
            objetoRenderer.sortingLayerName = layerAtras;
        }
        else
        {
            personagemRenderer.sortingLayerName = layerAtras;
            objetoRenderer.sortingLayerName = layerFrente;
        }
    }
}
