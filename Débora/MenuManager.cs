using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Nomes das cenas que cada botão deve carregar
    public Button cenaBotao1;
    public Button cenaBotao2;
    public Button cenaBotao3;
    public Button cenaBotao4;

    // Funções que serão chamadas pelos botões
    public void OnBotao1Click()
    {
            SceneManager.LoadScene("SlidePuzzle_3X3");
    }

    public void OnBotao2Click()
    {       
            SceneManager.LoadScene("SlidePuzzle_4X4");
    }

    public void OnBotao3Click()
    {
            SceneManager.LoadScene("SlidePuzzle_5X5");
    }

    public void OnBotao4Click()
    { 
            SceneManager.LoadScene("SlidePuzzle_6X6");
    }
}