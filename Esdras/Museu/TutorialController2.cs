using UnityEngine;
using UnityEngine.UI;

public class TutorialController2 : MonoBehaviour
{
    [Header("Referências de tela")]
    public GameObject telaAtual;
    public GameObject telaSucessora;

    [Header("Botões")]
    public Button botaoAvancar;

    void Start()
    {
        telaAtual.SetActive(true);
        telaSucessora.SetActive(false);
        botaoAvancar.onClick.AddListener(Avancar);

    }

    public void Avancar()
    {

       telaAtual.SetActive(false);
        telaSucessora.SetActive(true);

    }
}
