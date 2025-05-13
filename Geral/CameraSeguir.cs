using UnityEngine;

public class CameraSeguir : MonoBehaviour
{


    public Transform player; // Referência ao transform do jogador.
    public float timeLerp = 0.1f; // Velocidade de interpolação para suavizar o movimento da câmera.
    public float globalMinX = -0.9f; // Limite mínimo no eixo X
    public float globalMaxX = 89f;   // Limite máximo no eixo X
    public float globalMinY = -0.84f; // Limite mínimo no eixo Y
    public float globalMaxY = 3.59f;  // Limite máximo no eixo Y
    private Camera mainCamera; // Referência para a câmera principal.
    private float targetOrthographicSize = 5f; // Tamanho ortográfico desejado para a câmera.
    private float targetAspect = 1.33333f; // Proporção de aspecto desejada (4:3).
    private int lastScreenWidth; // Armazena a última largura da tela.
    private int lastScreenHeight; // Armazena a última altura da tela.

    void Start() // Chamado no início para configurar a câmera.
    {
        mainCamera = Camera.main; // Obtém a referência da câmera principal.
        AdjustCamera(); // Ajusta a câmera com base na proporção da tela.
    }

    void Update() // Chamado a cada frame para verificar mudanças na resolução da tela.
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) // Verifica se a resolução da tela mudou.
        {
            AdjustCamera(); // Ajusta a câmera novamente.
            lastScreenWidth = Screen.width; // Atualiza a largura da tela.
            lastScreenHeight = Screen.height; // Atualiza a altura da tela.
        }
    }

    private void AdjustCamera() // Função para ajustar a câmera conforme a proporção da tela.
    {
        float deviceAspect = (float)Screen.width / Screen.height; // Calcula a proporção da tela do dispositivo.
        mainCamera.orthographicSize = targetOrthographicSize * (targetAspect / deviceAspect) + 1f; // Ajusta o tamanho ortográfico da câmera.
    }

    private void FixedUpdate() // Chamado a cada fixação de física, para mover a câmera de forma suave.
    {
        if (player == null) return; // Se não houver referência ao jogador, sai da função.

        Vector3 targetPosition = player.position + new Vector3(-0.9f, 0.8f, -10); // Define a posição desejada da câmera.
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, timeLerp); // Suaviza o movimento da câmera.

        newPosition.x = Mathf.Clamp(newPosition.x, globalMinX, globalMaxX); // Limita a posição da câmera no eixo X.
        newPosition.y = Mathf.Clamp(newPosition.y, globalMinY, globalMaxY); // Limita a posição da câmera no eixo Y.

        transform.position = newPosition; // Aplica a nova posição à câmera.
    }
}

