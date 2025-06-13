using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorJogo : MonoBehaviour
{
    public void ReiniciarCena()
    {
        // Limpa manualmente todos os alunos persistentes
        AlunoSurgindo.LimparTodosAlunos();

        // Recarrega a cena
        Scene cenaAtual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cenaAtual.name);
    }
}
