using UnityEngine;

public class SistemaMover : MonoBehaviour
{
    public MoveSystem[] pieces; // Todas as pe�as do jogo

    public void CheckAllPieces()
    {
        Debug.Log("?? Verificando todas as pe�as..."); // Mensagem inicial para ver se o GameManager foi chamado

        bool allCorrect = true;

        foreach (MoveSystem piece in pieces)
        {
            if (!piece.IsCorrect()) // Se alguma pe�a estiver errada...
            {
                allCorrect = false;
                Debug.Log($"? Pe�a {piece.gameObject.name} est� errada!");
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("? Todas as pe�as foram colocadas corretamente! Parab�ns! ??");
        }
        else
        {
            Debug.Log("? Algumas pe�as est�o no lugar errado! Reiniciando...");
            ResetAllPieces();
        }
    }

    private void ResetAllPieces()
    {
        Debug.Log("?? Resetando todas as pe�as...");

        foreach (MoveSystem piece in pieces)
        {
            piece.ResetPosition();
        }
    }
}
