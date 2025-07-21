using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("Elementos iniciais")]
    public GameObject imagemFundo;
    public GameObject blurTitulo;
    public GameObject titulo;
    public GameObject curso;
    public GameObject personagem;
    public GameObject fundoTotal;

    [Header("Elementos intermedi�rios")]
    public GameObject blur; // blur de fundo durante os textos
    public GameObject[] textos; // textos mostrados um a um

    [Header("Tempos")]
    public float tempoInicial = 2f;
    public float tempoEntreTextos = 2f;

    void Start()
    {
        StartCoroutine(SequenciaIntro());
    }

    IEnumerator SequenciaIntro()
    {
        // Ativa os elementos iniciais
        fundoTotal.SetActive(true);
        imagemFundo.SetActive(true);
        blurTitulo.SetActive(true);
        titulo.SetActive(true);
        curso.SetActive(true);
        personagem.SetActive(true);
        blur.SetActive(false);

        yield return new WaitForSeconds(tempoInicial);

        // Desativa os elementos iniciais (exceto imagem de fundo)
        blurTitulo.SetActive(false);
        titulo.SetActive(false);
        curso.SetActive(false);
        personagem.SetActive(false);
        blur.SetActive(true);

        // Mostra os textos em sequ�ncia
        foreach (GameObject texto in textos)
        {
            texto.SetActive(true);
            yield return new WaitForSeconds(tempoEntreTextos);
            texto.SetActive(false);
        }

        // Fim da introdu��o
        imagemFundo.SetActive(false);
        blur.SetActive(false);
        fundoTotal.SetActive(false);

        // Aqui voc� pode iniciar o jogo (ativar HUD, personagem principal, etc)
        Debug.Log("Jogo come�ou!");
    }
}
