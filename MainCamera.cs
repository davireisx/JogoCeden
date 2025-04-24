using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Camera mainCamera;
    private float baseOrthographicSize = 5f; // Valor padr�o do tamanho ortogr�fico.

    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        mainCamera = Camera.main;
        AdjustCamera();
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

    private void AdjustCamera()
    {
        float baseAspect = 4f / 3f; // Aspecto de refer�ncia (4:3)
        float deviceAspect = (float)Screen.width / Screen.height;

        if (deviceAspect >= baseAspect)
        {
            // A tela � mais larga � mant�m a altura, adiciona "barras laterais"
            mainCamera.orthographicSize = baseOrthographicSize;
        }
        else
        {
            // A tela � mais estreita � aumenta a altura para caber tudo
            float scale = baseAspect / deviceAspect;
            mainCamera.orthographicSize = baseOrthographicSize * scale;
        }
    }
}
