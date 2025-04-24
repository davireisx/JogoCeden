using UnityEngine;

public class CameraSeguirEsdras : MonoBehaviour
{
    public Transform player; // Refer�ncia ao transform do jogador.
    public float timeLerp = 0.1f; // Suavidade da transi��o da c�mera.
    public Vector2 offset = new Vector2(0f, 0.5f); // Offset opcional para ajustar a posi��o da c�mera em rela��o ao jogador.

    public float globalMinX;
    public float globalMaxX;
    public float globalMinY;
    public float globalMaxY;

    public float baseOrthographicSize = 6f; // Valor base, aumente pra mais campo de vis�o.
    public float targetAspectRatio = 16f / 10f; // Propor��o desejada, ex: 16:10 � mais larga que 4:3.

    private Camera mainCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        mainCamera = Camera.main;
        AdjustCamera(); // Configura��o inicial do tamanho ortogr�fico.
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

    void LateUpdate() // Use LateUpdate para seguir suavemente o jogador ap�s ele se mover.
    {
        if (player == null) return;

        Vector3 targetPos = player.position + (Vector3)offset;
        targetPos.z = -10f; // Z fixo pra c�mera ortogr�fica

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
