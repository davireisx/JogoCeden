using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string nomeCena1;
    public string nomeCena2;

    public Button cena1; 
    public Button cena2;
    //public Button sair;

    private void Start()
    {
        // Vincula os botões aos métodos
        cena1.onClick.AddListener(OnBotao1Click);
        cena2.onClick.AddListener(OnBotao2Click);
    }

    public void OnBotao1Click()
    {
        SceneManager.LoadScene(nomeCena1);
    }

    public void OnBotao2Click()
    {
        SceneManager.LoadScene(nomeCena2);
    }

    /*public void OnBotao3Click()
    {
        Application.Quit(); // Encerra o jogo (funciona apenas em builds)
        Debug.Log("Sair do jogo."); // Para testes no editor
    } */
}
