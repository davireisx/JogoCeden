using UnityEngine;

public class SistemaArmaMiriam : MonoBehaviour
{
    private SpriteRenderer weaponSprite; // SpriteRenderer da arma
    private AudioSource audioSource; // �udio para disparos

    public GameObject bullet; // Prefab do projetil
    public Transform spawnBullet; // Local onde o projetil ser� instanciado

    // Amplitude maior para a varia��o de �ngulo
    [SerializeField] private float minAngle; // �ngulo m�nimo
    [SerializeField] private float maxAngle;  // �ngulo m�ximo

    private PlayerShooter playerController; // Controle do jogador

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        weaponSprite = GetComponent<SpriteRenderer>();
        playerController = transform.parent.GetComponent<PlayerShooter>();
    }

    void Update()
    {
        AimWeapon(); // Mira a arma com base na posi��o do cursor
        Shoot();     // Executa o disparo quando o bot�o for pressionado
    }

    private void AimWeapon()
    {
        if (playerController == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Calcula a dire��o relativa ao personagem
        float relativeX = mousePos.x - playerScreenPos.x;
        float relativeY = mousePos.y - playerScreenPos.y;

        // Inverte o X quando estiver virado para a esquerda
        if (playerController.IsFacingLeft())
        {
            relativeX = -relativeX;
        }

        float angle = Mathf.Atan2(relativeY, relativeX) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle); // Varia��o maior no �ngulo

        if (playerController.IsFacingLeft())
        {
            angle = 180f - angle; // Ajusta o �ngulo para a dire��o esquerda
            weaponSprite.flipY = true; // Flipa o sprite para o lado esquerdo
        }
        else
        {
            weaponSprite.flipY = false; // Mant�m o sprite normal no lado direito
        }

        transform.localRotation = Quaternion.Euler(0, 0, angle); // Aplica a rota��o calculada
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
