using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public Transform destinoTeleporte;
    public Transform[] novosWaypoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entrou em contato com: " + other.name + " - Tag: " + other.tag);

        if (!other.CompareTag("Enemy"))
        {
            Debug.Log("Objeto não tem a tag 'Enemy'");
            return;
        }

        Debug.Log("Aluno válido entrou no trigger: " + other.name);

        // Teleporta o aluno
        other.transform.position = destinoTeleporte.position + Vector3.up * 0.1f;

        // Aplica novos waypoints
        AlunoMovimento movimento = other.GetComponent<AlunoMovimento>();
        if (movimento != null && novosWaypoints != null && novosWaypoints.Length > 0)
        {
            Debug.Log("Aplicando novos waypoints para " + other.name);
            movimento.DefinirWaypoints(novosWaypoints);
        }
        else
        {
            Debug.LogWarning("Falha ao aplicar novos waypoints - Movimento: " + (movimento != null) +
                            ", Waypoints: " + (novosWaypoints != null ? novosWaypoints.Length : 0));
        }
    }
}
