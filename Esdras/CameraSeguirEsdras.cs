using UnityEngine;

public class CameraSeguirEsdras : MonoBehaviour
{
    public Transform player; // Referência ao transform do jogador.
    public float timeLerp = 0.1f; // Suavidade da transição da câmera.
    public Vector2 offset = new Vector2(0f, 0.5f); // Offset opcional para ajustar a posição da câmera em relação ao jogador.

    public float globalMinX;
    public float globalMaxX;
    public float globalMinY;
    public float globalMaxY;

    public float baseOrthographicSize = 6f; // Valor base, aumente pra mais campo de visão.
    public float targetAspectRatio = 16f / 10f; // Proporção desejada, ex: 16:10 é mais larga que 4:3.

    private Camera mainCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        mainCamera = Camera.main;
        AdjustCamera(); // Configuração inicial do tamanho ortográfico.
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustCamera();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    void LateUpdate() // Use LateUpdate para seguir suavemente o jogador após ele se mover.
    {
        if (player == null) return;

        Vector3 targetPos = player.position + (Vector3)offset;
        targetPos.z = -10f; // Z fixo pra câmera ortográfica

        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, timeLerp);

        smoothedPos.x = Mathf.Clamp(smoothedPos.x, globalMinX, globalMaxX);
        smoothedPos.y = Mathf.Clamp(smoothedPos.y, globalMinY, globalMaxY);

        transform.position = smoothedPos;
    }

    void AdjustCamera()
    {
        mainCamera.orthographicSize = baseOrthographicSize;
    }

}
