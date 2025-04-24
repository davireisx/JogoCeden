using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ArmaMiriam : MonoBehaviour
{
    [Header("Configura��o da Arma")]
    [SerializeField] private GameObject bullet; // Prefab do projetil
    [SerializeField] private Transform spawnBullet; // Ponto de spawn do projetil
    [SerializeField] private float fireRate; // Taxa de disparo (segundos por tiro)

    [Header("Detec��o de Inimigos")]
    [SerializeField] private float detectionRadius = 10f; // Raio de detec��o dos inimigos
    [SerializeField] private LayerMask enemyLayer; // Layer para identificar inimigos
    private Transform currentTarget; // Refer�ncia ao inimigo mais pr�ximo

    [Header("Joystick e Mira")]
    [SerializeField] private Joystick aimJoystick; // Joystick usado para mirar
    private Vector2 aimInput; // Input do joystick de mira

    [Header("Player")]
    [SerializeField] private Elias playerController; // Refer�ncia ao controle do jogador

    [Header("Bot�o de Disparo")]
    [SerializeField] private Button shootButton; // Bot�o para disparar o tiro

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = transform.parent.GetComponent<Elias>();

        if (shootButton != null)
        {
            // Vincula o m�todo de disparo ao bot�o
            shootButton.onClick.AddListener(Shoot);
        }
        else
        {
            Debug.LogError("Bot�o de disparo n�o configurado! Arraste o bot�o para o campo no Inspector.");
        }
    }

    private void Update()
    {
        DetectNearestEnemy(); // Detecta inimigos no raio
        AimWeapon(); // Mira a arma com suaviza��o
    }

    private void DetectNearestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider2D enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        // Define o inimigo mais pr�ximo como o alvo atual
        currentTarget = nearestEnemy;
    }

    private void AimWeapon()
    {
        if (currentTarget != null)
        {
            // Mira automaticamente no inimigo mais pr�ximo
            Vector2 directionToEnemy = (currentTarget.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;

            // Suaviza a rota��o da arma
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
        else if (aimJoystick != null && aimJoystick.Horizontal != 0 || aimJoystick.Vertical != 0)
        {
            // Usa a mira manual se nenhum inimigo estiver pr�ximo
            aimInput.x = aimJoystick.Horizontal;
            aimInput.y = aimJoystick.Vertical;

            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
    }

    public void Shoot()
    {
        if (bullet == null || spawnBullet == null)
        {
            Debug.LogError("Configura��o da bala ou ponto de spawn est� faltando!");
            return;
        }

        Instantiate(bullet, spawnBullet.position, transform.rotation);
        audioSource.Play();
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha o raio de detec��o no Editor para debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
