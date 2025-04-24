using UnityEngine;

public class SistemaArmaMiriam : MonoBehaviour
{
    private SpriteRenderer weaponSprite; // SpriteRenderer da arma
    private AudioSource audioSource; // Áudio para disparos

    public GameObject bullet; // Prefab do projetil
    public Transform spawnBullet; // Local onde o projetil será instanciado

    // Amplitude maior para a variação de ângulo
    [SerializeField] private float minAngle; // Ângulo mínimo
    [SerializeField] private float maxAngle;  // Ângulo máximo

    private PlayerShooter playerController; // Controle do jogador

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        weaponSprite = GetComponent<SpriteRenderer>();
        playerController = transform.parent.GetComponent<PlayerShooter>();
    }

    void Update()
    {
        AimWeapon(); // Mira a arma com base na posição do cursor
        Shoot();     // Executa o disparo quando o botão for pressionado
    }

    private void AimWeapon()
    {
        if (playerController == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Calcula a direção relativa ao personagem
        float relativeX = mousePos.x - playerScreenPos.x;
        float relativeY = mousePos.y - playerScreenPos.y;

        // Inverte o X quando estiver virado para a esquerda
        if (playerController.IsFacingLeft())
        {
            relativeX = -relativeX;
        }

        float angle = Mathf.Atan2(relativeY, relativeX) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle); // Variação maior no ângulo

        if (playerController.IsFacingLeft())
        {
            angle = 180f - angle; // Ajusta o ângulo para a direção esquerda
            weaponSprite.flipY = true; // Flipa o sprite para o lado esquerdo
        }
        else
        {
            weaponSprite.flipY = false; // Mantém o sprite normal no lado direito
        }

        transform.localRotation = Quaternion.Euler(0, 0, angle); // Aplica a rotação calculada
    }

    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(bullet, spawnBullet.position, transform.rotation); // Dispara o projetil
            audioSource.Play(); // Reproduz o som do disparo
        }
    }
}
