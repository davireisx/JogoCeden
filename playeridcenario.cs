using UnityEngine;

public class PlayerCenario : MonoBehaviour
{
    public int cenarioAtual = 1; // Cen�rio inicial do player

    // Esse m�todo pode ser chamado pelo CameraManager ou por triggers de cen�rio
    public void SetCenario(int numeroCenario)
    {
        cenarioAtual = numeroCenario;
        Debug.Log("Player agora est� no cen�rio: " + numeroCenario);
    }
}
