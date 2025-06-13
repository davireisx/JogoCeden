using UnityEngine;

public class CameraSeguirEsdras : MonoBehaviour
{
    public Transform player;
    public float timeLerp = 0.1f;
    public float globalMinX, globalMaxX, globalMinY, globalMaxY;

    [Header("Configura��o de Tamanho")]
    public float targetWidthInUnits = 64f;
    public float padding = 2f;

    private Camera mainCamera;
    private int lastScreenWidth, lastScreenHeight;

    void Start()
    {
        mainCamera = Camera.main;
        AjustarCamera();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AjustarCamera();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    void AjustarCamera()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        // Calcula o tamanho baseado na largura desejada
        float desiredOrthographicSize = (targetWidthInUnits / aspectRatio) / 2f;

        // Aplica padding
        desiredOrthographicSize -= padding / aspectRatio;

        // Calcula o tamanho m�ximo permitido pelos limites verticais
        float maxVerticalSize = (globalMaxY - globalMinY) / 2f;

        // Calcula o tamanho m�ximo permitido pelos limites horizontais
        float maxHorizontalSize = (globalMaxX - globalMinX) / (2f * aspectRatio);

        // Usa o menor tamanho entre o desejado e o m�ximo permitido
        mainCamera.orthographicSize = Mathf.Min(desiredOrthographicSize, maxVerticalSize, maxHorizontalSize);

        // Garante um tamanho m�nimo
        mainCamera.orthographicSize = Mathf.Max(mainCamera.orthographicSize, 1f);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + new Vector3(-0.9f, 0.8f, -10);
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, timeLerp);

        // Calcula os limites efetivos da c�mera
        float verticalExtent = mainCamera.orthographicSize;
        float horizontalExtent = verticalExtent * mainCamera.aspect;

        float effectiveMinX = globalMinX + horizontalExtent;
        float effectiveMaxX = globalMaxX - horizontalExtent;
        float effectiveMinY = globalMinY + verticalExtent;
        float effectiveMaxY = globalMaxY - verticalExtent;

        // Garante que os limites n�o sejam invertidos
        if (effectiveMinX > effectiveMaxX) effectiveMinX = effectiveMaxX = (globalMinX + globalMaxX) / 2f;
        if (effectiveMinY > effectiveMaxY) effectiveMinY = effectiveMaxY = (globalMinY + globalMaxY) / 2f;

        newPosition.x = Mathf.Clamp(newPosition.x, effectiveMinX, effectiveMaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, effectiveMinY, effectiveMaxY);

        transform.position = newPosition;
    }
}