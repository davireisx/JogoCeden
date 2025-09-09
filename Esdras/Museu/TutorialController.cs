using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Refer�ncias de tela")]
    public GameObject telaAnterior;
    public GameObject telaAtual;
    public GameObject telaSucessora;

    [Header("Bot�es")]
    public Button botaoAvancar;
    public Button botaoVoltar;


    [Header("Audio")]
    public AudioSource next;
    public AudioSource back;


    void Start()
    {
        // Garante que apenas a tela atual est� ativa
        if (telaAtual != null)
            telaAtual.SetActive(true);

        if (telaAnterior != null)
            telaAnterior.SetActive(false);

        if (telaSucessora != null)
            telaSucessora.SetActive(false);

        // Configura os bot�es
        if (botaoAvancar != null)
            botaoAvancar.onClick.AddListener(Avancar);

        if (botaoVoltar != null)
            botaoVoltar.onClick.AddListener(Voltar);
    }

    public void Avancar()
    {
        Debug.Log("Avan�ar clicado - tentando tocar som");
        if (next != null && next.clip != null)
            next.Play();
        else
            Debug.LogWarning("AudioSource 'next' ou o AudioClip n�o foi configurado!");

        if (telaAtual != null) telaAtual.SetActive(false);
        if (telaSucessora != null) telaSucessora.SetActive(true);
    }

    public void Voltar()
    {
        Debug.Log("Voltar clicado - tentando tocar som");
        if (back != null && back.clip != null)
            back.Play();
        else
            Debug.LogWarning("AudioSource 'back' ou o AudioClip n�o foi configurado!");

        if (telaAtual != null) telaAtual.SetActive(false);
        if (telaAnterior != null) telaAnterior.SetActive(true);
    }

}
