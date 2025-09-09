using UnityEngine;
using System.Collections;

public class TeleportFinal : MonoBehaviour
{
    public static bool teleporting = false;
    public static int targetSpawnIndex = 0;

    public static Vector2 savedJoystickInput = Vector2.zero;

    [SerializeField] private int mySpawn; // Defina no inspetor para cada portal
    [SerializeField] private int scenarioNumber; // Novo: 1 = cen�rio 1, 2 = cen�rio 2
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Transform[] spawnPoints; // Pontos de destino no mapa, posicione no Inspector

    private bool playerInside = false;
    private float timeInside = 0f;
    [SerializeField] private float timeToTeleport = 0.3f;

    private GameObject playerObj;

    // Adicionamos uma refer�ncia ao CameraManager para ser configurada no Inspector
    [SerializeField] private CameraManagerEsdras cameraManager;

    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (playerObj == null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerObj != null && !teleporting)
        {
            Vector2 playerPos = playerObj.transform.position;

            if (myCollider.OverlapPoint(playerPos))
            {
                // Teleportar IMEDIATAMENTE ao detectar que o player est� dentro
                AlunoSegundoAno aluno = playerObj.GetComponent<AlunoSegundoAno>();
                if (aluno != null)
                {
                    teleporting = true;
                    targetSpawnIndex = mySpawn;
                    savedJoystickInput = aluno.GetCurrentInput();
                    StartCoroutine(Transition(playerObj));
                }
            }
        }
    }




    private void Start()
    {
        if (teleporting)
        {
            myCollider.enabled = false;
            StartCoroutine(EnableColliderAfterDelay(1f));
        }
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        myCollider.enabled = true;
        teleporting = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!myCollider.enabled || teleporting) return;

        if (other.CompareTag("Player"))
        {
            playerInside = true;
            timeInside = 0f;
            playerObj = other.gameObject;

            AlunoSegundoAno aluno = other.GetComponent<AlunoSegundoAno>();

            // Opcional: teleportar imediatamente se o jogador estiver se movendo
            if (aluno != null) //&& (aluno.IsMoving() || aluno.IsInputLocked())//)
            {
                teleporting = true;
                targetSpawnIndex = mySpawn;
                savedJoystickInput = aluno.GetCurrentInput();
                StartCoroutine(Transition(other.gameObject));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timeInside = 0f;
            playerObj = null;
        }
    }


    private IEnumerator Transition(GameObject player)
    {
        // FADE OUT
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = 1f; // Garante 100% opaco

        yield return new WaitForSeconds(0.1f); // pequena pausa opcional

        // MOVE o jogador
        if (targetSpawnIndex >= 0 && targetSpawnIndex < spawnPoints.Length)
        {
            player.transform.position = spawnPoints[targetSpawnIndex].position;

            Esdras esdras = player.GetComponent<Esdras>();
            esdras.SetInput(savedJoystickInput);
        }

        // ATUALIZA a c�mera
        if (cameraManager != null)
        {
            cameraManager.SetScenarioBounds(scenarioNumber);
        }

        // FADE IN
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = 0f; // Garante 100% transparente

        teleporting = false;
    }

}