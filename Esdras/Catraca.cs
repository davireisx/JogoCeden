using UnityEngine;

public class Catraca : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private string corNormalHex = "#E6F0FF";    // Branco azulado
    [SerializeField] private string corCorrompidaHex = "#6A0DAD"; // Roxo escuro

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetColor(corNormalHex); // Inicia com cor normal
    }

    // Método principal de conversão HEX para Color
    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        Debug.LogWarning($"Cor hexadecimal inválida: {hex}");
        return Color.magenta; // Cor fallback visível
    }

    // Atualiza cor com tratamento de erro
    private void SetColor(string hexColor)
    {
        spriteRenderer.color = HexToColor(hexColor);
    }

    public void Corromper()
    {
        SetColor(corCorrompidaHex);
        // Lógica adicional de corrupção...
    }

    public void Consertar()
    {
        SetColor(corNormalHex);
        // Lógica adicional de conserto...
    }
}