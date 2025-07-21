using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompletarFiacaoInversa : MonoBehaviour
{
    [Header("Fios a verificar")]
    public WireDragComLimite[] fios; // Atribua todos os fios no Inspector

    [Header("Refer�ncias para troca")]
    public Transform player;
    public Transform novoSpawnPoint;
    public GameObject joystick;
    public CameraManagerEsdras cameraManager;
    public Image telaFade;
    public GameObject HUD;

    [Header("Configura��es")]
    public float fadeDuration = 1f;
    public int novoCenarioIndex = 2;

    private bool trocaFeita = false;

    void Start()
    {
        if (fios.Length == 0)
        {
            Debug.LogWarning("Nenhum fio atribu�do no array de fios.");
        }

        if (telaFade != null)
        {
            Color c = telaFade.color;
            c.a = 0;
            telaFade.color = c;
            telaFade.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (trocaFeita) return;

        bool todosConectadosErrado = true;

        foreach (WireDragComLimite fio in fios)
        {
            if (fio == null)
            {
                todosConectadosErrado = false;
                break;
            }

            // Se n�o est� conectado ou est� conectado no correto, falha a condi��o
            if (!fioConectadoErrado(fio))
            {
                todosConectadosErrado = false;
                break;
            }
        }

        if (todosConectadosErrado)
        {
            StartCoroutine(FazerTransicao());
            trocaFeita = true;
        }
    }

    bool fioConectadoErrado(WireDragComLimite fio)
    {
        // Ele est� conectado, mas n�o no destino correto
        return fio != null &&
               fio.GetEstaConectado() &&
               !fio.EstaNoDestinoCorreto();
    }

    IEnumerator FazerTransicao()
    {
        telaFade.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn());

        // Move player e ativa controles
        player.position = novoSpawnPoint.position;
        player.gameObject.SetActive(true);
        if (cameraManager != null) cameraManager.SetScenarioBounds(novoCenarioIndex);
        if (joystick != null) joystick.SetActive(true);
        if (HUD != null) HUD.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(FadeOut());

        telaFade.gameObject.SetActive(false);

        Debug.Log("? Vit�ria invertida: jogador bagun�ou tudo corretamente!");
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        Color c = telaFade.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            telaFade.color = c;
            yield return null;
        }
        c.a = 1;
        telaFade.color = c;
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Color c = telaFade.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            telaFade.color = c;
            yield return null;
        }
        c.a = 0;
        telaFade.color = c;
    }
}
