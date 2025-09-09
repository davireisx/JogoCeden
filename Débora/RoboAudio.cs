using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RoboAudio : MonoBehaviour
{
    [Header("Configura��es")]
    public AudioSource audioSource;
    public GameObject robo; // refer�ncia ao GameObject do Robo
    public Transform[] waypoints; // waypoints que queremos monitorar
    public float alcance = 0.1f; // dist�ncia m�nima para considerar que chegou no waypoint

    private bool somAtivo = false; // controla se o som est� tocando
    private int waypointAtual = -1;

    private void Awake()
    {
        // Cria um AudioSource se n�o existir
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true; // loop para continuar tocando enquanto estiver no waypoint
    }

    private void Update()
    {
        if (robo == null || waypoints == null || waypoints.Length == 0)
            return;

        bool estaEmWaypoint = false;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            float distancia = Vector3.Distance(robo.transform.position, waypoints[i].position);

            if (distancia <= alcance)
            {
                estaEmWaypoint = true;

                if (waypointAtual != i)
                {
                    waypointAtual = i;
                    IniciarSom();
                }

                break; // j� encontrou um waypoint
            }
        }

        // Se n�o est� em nenhum waypoint, para o som
        if (!estaEmWaypoint && somAtivo)
        {
            PararSom();
            waypointAtual = -1;
        }
    }

    private void IniciarSom()
    {
        if (!somAtivo && audioSource != null)
        {
            audioSource.Play();
            somAtivo = true;
            Debug.Log("Som iniciado no waypoint " + waypointAtual);
        }
    }

    private void PararSom()
    {
        if (somAtivo && audioSource != null)
        {
            audioSource.Stop();
            somAtivo = false;
            Debug.Log("Som parado");
        }
    }
}
