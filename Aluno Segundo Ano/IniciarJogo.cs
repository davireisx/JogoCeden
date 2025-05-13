using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // Para carregar cenas
using System.Collections;

public class IniciarJogo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoContainer;
    public CanvasGroup fadeCanvasGroup;   // Canvas Group no painel preto
    public string nextSceneName = "Scene2"; // Nome da pr�xima cena a ser carregada
    public float fadeDuration = 1.5f;

    void Start()
    {
        // Come�a preto total
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1;

        // Toca o v�deo normalmente
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(WaitAudioThenFade());
    }

    IEnumerator WaitAudioThenFade()
    {
        // Suaviza o volume do �udio do v�deo
        float vol = 1f;
        while (vol > 0f)
        {
            vol -= Time.deltaTime / 0.5f; // volume reduzido em 0.5 segundos
            videoPlayer.SetDirectAudioVolume(0, Mathf.Clamp01(vol));
            yield return null;
        }

        // Aguarda o v�deo parar completamente (imagem + som)
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        // Desativa o container de v�deo
        if (videoContainer != null)
            videoContainer.SetActive(false);

        StartCoroutine(FadeOutAndStartGame());
    }

    IEnumerator FadeOutAndStartGame()
    {
        // Espera 1.5 segundos com a tela preta
        yield return new WaitForSeconds(fadeDuration);

        // Fade out (de preto para transparente)
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Carrega a pr�xima cena ap�s o fade
        SceneManager.LoadScene(nextSceneName);
    }
}
