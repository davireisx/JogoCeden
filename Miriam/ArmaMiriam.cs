using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ArmaMiriam : MonoBehaviour
{
    [Header("Configuração da Arma")]
    [SerializeField] private GameObject bullet; // Prefab do projetil
    [SerializeField] private Transform spawnBullet; // Ponto de spawn do projetil
    [SerializeField] private float fireRate; // Taxa de disparo (segundos por tiro)

    [Header("Detecção de Inimigos")]
    [SerializeField] private float detectionRadius = 10f; // Raio de detecção dos inimigos
    [SerializeField] private LayerMask enemyLayer; // Layer para identificar inimigos
    private Transform currentTarget; // Referência ao inimigo mais próximo

    [Header("Joystick e Mira")]
    [SerializeField] private Joystick aimJoystick; // Joystick usado para mirar
    private Vector2 aimInput; // Input do joystick de mira

    [Header("Player")]
    [SerializeField] private Elias playerController; // Referência ao controle do jogador

    [Header("Botão de Disparo")]
    [SerializeField] private Button shootButton; // Botão para disparar o tiro

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = transform.parent.GetComponent<Elias>();

        if (shootButton != null)
        {
            // Vincula o método de disparo ao botão
            shootButton.onClick.AddListener(Shoot);
        }
        else
        {
            Debug.LogError("Botão de disparo não configurado! Arraste o botão para o campo no Inspector.");
        }
    }

    private void Update()
    {
        DetectNearestEnemy(); // Detecta inimigos no raio
        AimWeapon(); // Mira a arma com suavização
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

        // Define o inimigo mais próximo como o alvo atual
        currentTarget = nearestEnemy;
    }

    private void AimWeapon()
    {
        if (currentTarget != null)
        {
            // Mira automaticamente no inimigo mais próximo
            Vector2 directionToEnemy = (currentTarget.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;

            // Suaviza a rotação da arma
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
        else if (aimJoystick != null && aimJoystick.Horizontal != 0 || aimJoystick.Vertical != 0)
        {
            // Usa a mira manual se nenhum inimigo estiver próximo
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
            Debug.LogError("Configuração da bala ou ponto de spawn está faltando!");
            return;
        }

        Instantiate(bullet, spawnBullet.position, transform.rotation);
        audioSource.Play();
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha o raio de detecção no Editor para debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
