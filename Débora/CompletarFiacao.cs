using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem.HID;

public class CompletarFiacao : MonoBehaviour
{
    [Header("Objetos a verificar")]
    public GameObject[] objetosParaVerificar; // Precisa ter exatamente 4 no inspector

    [Header("Refer�ncias para troca")]
    public Transform player;
    public Transform novoSpawnPoint;
    public GameObject joystick;
    public CameraManagerEsdras cameraManager;
    public Image telaFade;

    [Header("Desativar Script Ap�s Troca")]
    public GameObject objetoComInteragir; // GameObject com o InteragirTrocaCenario
    public GameObject HUD;

    [Header("Vag�o para pintar")]
    public GameObject vagao;

    [Header("Configura��es")]
    public float fadeDuration = 1f;
    public int novoCenarioIndex = 2;

    private bool trocaFeita = false;

    void Start()
    {
        if (objetosParaVerificar.Length != 4)
            Debug.LogWarning("Voc� precisa atribuir exatamente 4 objetos para verifica��o.");

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

        bool todosAtivos = true;
        foreach (GameObject obj in objetosParaVerificar)
        {
            if (obj == null || !obj.activeSelf)
            {
                todosAtivos = false;
                break;
            }
        }

        if (todosAtivos)
        {
            StartCoroutine(FazerTransicao());
            trocaFeita = true;
        }
    }

    IEnumerator FazerTransicao()
    {
     
        telaFade.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn());

        // Teleporta e reativa
        player.position = novoSpawnPoint.position;
        if (cameraManager != null) cameraManager.SetScenarioBounds(novoCenarioIndex);

        player.gameObject.SetActive(true);
        if (joystick != null) joystick.SetActive(true);

        // Desativa o script InteragirTrocaCenario
        if (objetoComInteragir != null)
        {
            InteragirTrocaCenario interagir = objetoComInteragir.GetComponent<InteragirTrocaCenario>();
            if (interagir != null)
            {
                interagir.enabled = false;
                Debug.Log("Script InteragirTrocaCenario desativado.");
            }
            else
            {
                Debug.LogWarning("O GameObject n�o tem o script InteragirTrocaCenario.");
            }
        }

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(FadeOut());

        telaFade.gameObject.SetActive(false);
        HUD.gameObject.SetActive(true);
        joystick.gameObject.SetActive(true);


        Debug.Log("Troca de cen�rio conclu�da.");

        // Pinta o vag�o de branco
        if (vagao != null)
        {
            SpriteRenderer sr = vagao.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.white;
                Debug.Log("Vag�o ficou branco!");
            }
            else
            {
                Debug.LogWarning("O GameObject 'vag�o' n�o tem um SpriteRenderer.");
            }
        }
        else
        {
            Debug.LogWarning("O campo 'vagao' n�o foi atribu�do no Inspector.");
        }

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
