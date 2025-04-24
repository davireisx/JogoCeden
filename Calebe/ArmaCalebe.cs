using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ArmaCalebe : MonoBehaviour
{
    [Header("Configuração da Arma")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject weakBullet;
    [SerializeField] private GameObject mediumBullet;
    [SerializeField] private GameObject strongBullet;
    [SerializeField] private GameObject veryStrongBullet;
    [SerializeField] private Transform spawnBullet;
    [SerializeField] private SpriteRenderer comunicadorSpriteRenderer;
    [SerializeField] private GameObject barraCarregamento; // Barra de carregamento
    [SerializeField] private GameObject mira; // Mira no Canva
    [SerializeField] private float fireRate = 3f;

    [Header("Joystick e Mira")]
    [SerializeField] private Joystick aimJoystick;
    private Vector2 aimInput;

    [Header("Personagens")]
    [SerializeField] private Calebe playerController;
    [SerializeField] private ArmaCalebe arma;
    [SerializeField] private QueroQuero queroQuero;

    [Header("Configurações de Disparo")]
    [SerializeField] private float tempoMinimoPressionado = 0.1f;
    private bool podeDisparar = true;

    [Header("Barra de Carregamento")]
    [SerializeField] private Animator barraDeCarregamentoAnimator;
    [SerializeField] private float tempoParaCarregar = 5f;
    [SerializeField] private float tempoDeRetrocesso = 0f;
    [SerializeField] private float tempoDePausaAoSoltar = 1.5f;


    private AudioSource audioSource;
    private bool estaCarregando = false;
    private bool pausando = false;
    private bool retrocedendo = false;
    private bool podeAtacarQueroQuero = true; // Já existente para controlar o ataque do Quero-Quero
    private float tempoPressionado = 0f;
    private float tempoUltimoDisparo = 0f;
    private int faseAtual = 0;
    private IEnumerator retrocessoCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = transform.parent.GetComponent<Calebe>();

        if (aimJoystick != null)
        {
            EventTrigger joystickTrigger = aimJoystick.GetComponent<EventTrigger>();
            if (joystickTrigger == null)
                joystickTrigger = aimJoystick.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => OnShootButtonPressed());
            joystickTrigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => OnShootButtonReleased());
            joystickTrigger.triggers.Add(pointerUpEntry);
        }
    }

    private void Update()
    {
        AimWeapon();

        if (estaCarregando && podeDisparar)
        {
            tempoPressionado += Time.deltaTime;
            tempoPressionado = Mathf.Clamp(tempoPressionado, 0, tempoParaCarregar);

            if (tempoPressionado >= tempoMinimoPressionado)
            {
                int novaFase = Mathf.Clamp(Mathf.FloorToInt(tempoPressionado / (tempoParaCarregar / 17)), 0, 17);
                AtualizarFase(novaFase);
            }
        }
    }

    private void AtualizarFase(int novaFase)
    {
        if (faseAtual != novaFase)
        {
            faseAtual = novaFase;
            barraDeCarregamentoAnimator.SetInteger("Carregamento", faseAtual);
        }
    }

    public void OnShootButtonPressed()
    {
        if (!podeDisparar) return;

        if (retrocedendo && retrocessoCoroutine != null)
        {
            StopCoroutine(retrocessoCoroutine);
            retrocedendo = false;
        }

        estaCarregando = true;
        pausando = false;

        if (tempoPressionado < tempoMinimoPressionado)
        {
            tempoPressionado = 0f;
            AtualizarFase(0);
        }
    }

    public void OnShootButtonReleased()
    {
        if (!estaCarregando || !podeDisparar) return;

        estaCarregando = false;

        // Notificar o Quero-Quero que o jogador disparou
        if (queroQuero != null)
        {
            Debug.Log("Notificando o Quero-Quero que o jogador disparou novamente!");
            queroQuero.JogadorAtirou(); // Adicionado: Notifica o Quero-Quero
        }

        // Toque rápido - dispara bala fraca
        if (tempoPressionado < tempoMinimoPressionado)
        {
            ShootWeakBullet();
            ResetarBarra();

            podeDisparar = false;
            pausando = true;
            Invoke("IniciarRetrocesso", tempoDePausaAoSoltar);
            return;
        }

        // Disparo conforme a fase
        if (faseAtual >= 0 && faseAtual <= 3) ShootWeakBullet();
        else if (faseAtual >= 4 && faseAtual <= 7) ShootNormalBullet();
        else if (faseAtual >= 8 && faseAtual <= 11) ShootMediumBullet();
        else if (faseAtual >= 12 && faseAtual <= 15) ShootStrongBullet();
        else ShootVeryStrongBullet();

        podeDisparar = false;
        pausando = true;
        Invoke("IniciarRetrocesso", tempoDePausaAoSoltar);
    }

    public void Shoot()
    {
        OnShootButtonPressed();
        OnShootButtonReleased();
    }

    private void IniciarRetrocesso()
    {
        if (!estaCarregando)
        {
            retrocessoCoroutine = RetrocederFases();
            StartCoroutine(retrocessoCoroutine);
        }
    }

    private IEnumerator RetrocederFases()
    {
        retrocedendo = true;

        while (faseAtual > 0 && !estaCarregando)
        {
            yield return new WaitForSeconds(tempoDeRetrocesso);
            tempoPressionado = Mathf.Max(tempoPressionado - (tempoParaCarregar / 17), 0);
            int novaFase = Mathf.FloorToInt(tempoPressionado / (tempoParaCarregar / 17));
            AtualizarFase(novaFase);
        }

        retrocedendo = false;
        pausando = false;
        podeDisparar = true;
    }

    private void ResetarBarra()
    {
        tempoPressionado = 0f;
        AtualizarFase(0);
    }

    private void ShootWeakBullet()
    {
        DispararProjetil(bullet, "Fraco");
    }

    private void ShootNormalBullet()
    {
        DispararProjetil(weakBullet, "Normal");
    }

    private void ShootMediumBullet()
    {
        DispararProjetil(mediumBullet, "Médio");
    }

    private void ShootStrongBullet()
    {
        DispararProjetil(strongBullet, "Forte");
    }

    private void ShootVeryStrongBullet()
    {
        DispararProjetil(veryStrongBullet, "Muito Forte");
    }

    private void DispararProjetil(GameObject bulletPrefab, string tipo)
    {
        if (Time.time < tempoUltimoDisparo + fireRate || !podeDisparar)
        {
            Debug.Log($"Disparo {tipo} bloqueado! Aguarde o cooldown.");
            return;
        }

        if (bulletPrefab == null || spawnBullet == null) return;

        tempoUltimoDisparo = Time.time;
        Instantiate(bulletPrefab, spawnBullet.position, spawnBullet.rotation);

        if (audioSource != null) audioSource.Play();
    }

    private void AimWeapon()
    {
        if (aimJoystick != null && (aimJoystick.Horizontal != 0 || aimJoystick.Vertical != 0))
        {
            aimInput.x = aimJoystick.Horizontal;
            aimInput.y = aimJoystick.Vertical;

            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Atualizar a posição do indicador de mira
            Vector3 direction = new Vector3(aimInput.x, aimInput.y, 0).normalized;
            Vector3 indicatorPosition = transform.position + direction * 2f;

        }

    }


    public void DesativarEReativarElementos(float delay)
    {
        if (!podeAtacarQueroQuero) return; // Evita múltiplas chamadas se já está desativado

        podeAtacarQueroQuero = false;

        // Desativa os elementos
        if (comunicadorSpriteRenderer != null) comunicadorSpriteRenderer.enabled = false;
        if (barraCarregamento != null) barraCarregamento.SetActive(false);
        if (mira != null) mira.SetActive(false);

        // Reativa os elementos após o tempo
        StartCoroutine(ReativarElementos(delay));
    }

    private IEnumerator ReativarElementos(float delay)
    {
        Debug.Log("Esperando " + delay + " segundos para reativar os elementos...");
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log("Iniciando reativação dos elementos...");

        // Reativa os elementos
       comunicadorSpriteRenderer.enabled = true;
       barraCarregamento.SetActive(true);
        mira.SetActive(true);

        podeAtacarQueroQuero = true; // Permite que o jogador volte a ser atacado pelo Quero-Quero
        Debug.Log("Elementos reativados!");
    }


}