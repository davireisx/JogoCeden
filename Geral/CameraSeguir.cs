using UnityEngine;

public class CameraSeguir : MonoBehaviour
{


    public Transform player; // Refer�ncia ao transform do jogador.
    public float timeLerp = 0.1f; // Velocidade de interpola��o para suavizar o movimento da c�mera.
    public float globalMinX = -0.9f; // Limite m�nimo no eixo X
    public float globalMaxX = 89f;   // Limite m�ximo no eixo X
    public float globalMinY = -0.84f; // Limite m�nimo no eixo Y
    public float globalMaxY = 3.59f;  // Limite m�ximo no eixo Y
    private Camera mainCamera; // Refer�ncia para a c�mera principal.
    private float targetOrthographicSize = 5f; // Tamanho ortogr�fico desejado para a c�mera.
    private float targetAspect = 1.33333f; // Propor��o de aspecto desejada (4:3).
    private int lastScreenWidth; // Armazena a �ltima largura da tela.
    private int lastScreenHeight; // Armazena a �ltima altura da tela.

    void Start() // Chamado no in�cio para configurar a c�mera.
    {
        mainCamera = Camera.main; // Obt�m a refer�ncia da c�mera principal.
        AdjustCamera(); // Ajusta a c�mera com base na propor��o da tela.
    }

    void Update() // Chamado a cada frame para verificar mudan�as na resolu��o da tela.
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) // Verifica se a resolu��o da tela mudou.
        {
            AdjustCamera(); // Ajusta a c�mera novamente.
            lastScreenWidth = Screen.width; // Atualiza a largura da tela.
            lastScreenHeight = Screen.height; // Atualiza a altura da tela.
        }
    }

    private void AdjustCamera() // Fun��o para ajustar a c�mera conforme a propor��o da tela.
    {
        float deviceAspect = (float)Screen.width / Screen.height; // Calcula a propor��o da tela do dispositivo.
        mainCamera.orthographicSize = targetOrthographicSize * (targetAspect / deviceAspect) + 1f; // Ajusta o tamanho ortogr�fico da c�mera.
    }

    private void FixedUpdate() // Chamado a cada fixa��o de f�sica, para mover a c�mera de forma suave.
    {
        if (player == null) return; // Se n�o houver refer�ncia ao jogador, sai da fun��o.

        Vector3 targetPosition = player.position + new Vector3(-0.9f, 0.8f, -10); // Define a posi��o desejada da c�mera.
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, timeLerp); // Suaviza o movimento da c�mera.

        newPosition.x = Mathf.Clamp(newPosition.x, globalMinX, globalMaxX); // Limita a posi��o da c�mera no eixo X.
        newPosition.y = Mathf.Clamp(newPosition.y, globalMinY, globalMaxY); // Limita a posi��o da c�mera no eixo Y.

        transform.position = newPosition; // Aplica a nova posi��o � c�mera.
    }
}

