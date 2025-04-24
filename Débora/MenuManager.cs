using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Nomes das cenas que cada bot�o deve carregar
    public Button cenaBotao1;
    public Button cenaBotao2;
    public Button cenaBotao3;
    public Button cenaBotao4;

    // Fun��es que ser�o chamadas pelos bot�es
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