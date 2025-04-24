using UnityEngine;

public class Catraca : MonoBehaviour
{
    [Header("Configura��es")]
    [SerializeField] private string corNormalHex = "#E6F0FF";    // Branco azulado
    [SerializeField] private string corCorrompidaHex = "#6A0DAD"; // Roxo escuro

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetColor(corNormalHex); // Inicia com cor normal
    }

    // M�todo principal de convers�o HEX para Color
    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        Debug.LogWarning($"Cor hexadecimal inv�lida: {hex}");
        return Color.magenta; // Cor fallback vis�vel
    }

    // Atualiza cor com tratamento de erro
    private void SetColor(string hexColor)
    {
        spriteRenderer.color = HexToColor(hexColor);
    }

    public void Corromper()
    {
        SetColor(corCorrompidaHex);
        // L�gica adicional de corrup��o...
    }

    public void Consertar()
    {
        SetColor(corNormalHex);
        // L�gica adicional de conserto...
    }
}