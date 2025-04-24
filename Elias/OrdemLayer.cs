using UnityEngine;

public class OrdemLayer : MonoBehaviour
{
    [Header("Configuração dos Objetos")]
    public Transform geradorParent; // Objeto pai do Gerador1 (contém os filhos)
    public GameObject personagem;    // Objeto do personagem

    private SpriteRenderer personagemRenderer;  // SpriteRenderer do personagem
    private SpriteRenderer[] netosRenderers;   // Array com os SpriteRenderers dos netos do Gerador1

    [Header("Configuração de Tolerância")]
    public float margemTolerancia = 0.001f; // Define uma margem para evitar alternância
    public string camadaDefaultNetos = "Objetos";
    public string camadaDefaultPersonagem = "Personagem";

    void Start()
    {
        // Obtém o SpriteRenderer do personagem
        personagemRenderer = personagem.GetComponent<SpriteRenderer>();

        // Encontra apenas os netos (filhos dos filhos de Gerador1) que possuem SpriteRenderer
        netosRenderers = GetNetosSpriteRenderers(geradorParent);
    }

    void Update()
    {
        if (netosRenderers.Length > 0)
        {
            var primeiroNetoRenderer = netosRenderers[0];
            float diferencaY = personagem.transform.position.y - primeiroNetoRenderer.transform.position.y;

            // Verifica o intervalo de tolerância
            if (Mathf.Abs(diferencaY) <= margemTolerancia)
            {
                personagemRenderer.sortingLayerName = camadaDefaultPersonagem;
                primeiroNetoRenderer.sortingLayerName = camadaDefaultNetos;
                Debug.Log("Dentro do intervalo de tolerância. Camadas permanecem padrão.");
                return;
            }

            // Ajustar camadas do primeiro neto e replicar para os demais
            string camadaPersonagem, camadaNetos;

            if (personagem.transform.position.y > primeiroNetoRenderer.transform.position.y)
            {
                camadaPersonagem = "Personagem";
                camadaNetos = "Objetos";
            }
            else
            {
                camadaPersonagem = "Objetos";
                camadaNetos = "Personagem";
            }

            personagemRenderer.sortingLayerName = camadaPersonagem;

            foreach (var netoRenderer in netosRenderers)
            {
                netoRenderer.sortingLayerName = camadaNetos;
            }
        }
    }

    // Método para obter SpriteRenderers apenas dos netos do objeto pai
    private SpriteRenderer[] GetNetosSpriteRenderers(Transform parent)
    {
        var netosList = new System.Collections.Generic.List<SpriteRenderer>();

        foreach (Transform filho in parent)
        {
            foreach (Transform neto in filho)
            {
                SpriteRenderer renderer = neto.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    netosList.Add(renderer);
                }
            }
        }

        return netosList.ToArray();
    }
}
