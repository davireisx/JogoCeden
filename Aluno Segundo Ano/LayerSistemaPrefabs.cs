using UnityEngine;

public class LayerSistemaPrefabs : MonoBehaviour
{
    [Header("Configura��o de Layers")]
    public string layerFrente = "Personagem";
    public string layerAtras = "Objetos";
    public float offsetY = 0.1f; // Ajuste fino para diferen�as de altura

    [Header("Refer�ncias")]
    public GameObject personagemBase; // Arraste o GameObject PAI do "P� 1"
    public GameObject objetoBase;     // Arraste o GameObject PAI do "P� 1" deste prefab
    public SpriteRenderer personagemRenderer;
    public SpriteRenderer objetoRenderer;

    private Transform personagemPivot;
    private Transform objetoPivot;

    void Start()
    {
        // Pega o primeiro filho do GameObject personagem
        if (personagemBase != null && personagemBase.transform.childCount > 0)
        {
            personagemPivot = personagemBase.transform.GetChild(0);
        }
        else
        {
            Debug.LogWarning("O personagemBase n�o possui filho (P� 1).");
        }

        // Pega o primeiro filho do GameObject objeto
        if (objetoBase != null && objetoBase.transform.childCount > 0)
        {
            objetoPivot = objetoBase.transform.GetChild(0);
        }
        else
        {
            Debug.LogWarning("O objetoBase n�o possui filho (P� 1).");
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
