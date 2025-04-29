using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Catraca))]
public class AproximarCatraca : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private Button botaoInteracao;
    [SerializeField] private Transform jogador;
    [SerializeField] private float raioInteracao = 2.5f;
    [SerializeField] private GameObject painelMinigame;
    [SerializeField] private DesafioDasCores desafioDasCores;

    private Camera mainCam;
    private RectTransform botaoRect;
    private Catraca catraca;

    void Start()
    {
        mainCam = Camera.main;
        botaoRect = botaoInteracao.GetComponent<RectTransform>();
        catraca = GetComponent<Catraca>();

        if (botaoInteracao == null || painelMinigame == null)
        {
            Debug.LogError("AproximarCatraca: referência perdida!");
            enabled = false;
            return;
        }

        if (desafioDasCores == null)
            desafioDasCores = painelMinigame.GetComponent<DesafioDasCores>();

        botaoInteracao.gameObject.SetActive(false);

        // adiciona listener ao clique
        botaoInteracao.onClick.AddListener(OnBotaoClicked);
    }

    void Update()
    {
        // checa distância
        float dist = Vector3.Distance(jogador.position, transform.position);
        if (dist > raioInteracao)
        {
            botaoInteracao.gameObject.SetActive(false);
            return;
        }

        // checa se deve ativar o botão baseado na cor
        if (!catraca.DeveAtivarBotao())
        {
            botaoInteracao.gameObject.SetActive(false);
            return;
        }

        // posiciona + ativa
        Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position);
        botaoRect.position = screenPos + new Vector3(0, 50f, 0);

        if (!botaoInteracao.gameObject.activeSelf)
            botaoInteracao.gameObject.SetActive(true);
    }

    public void OnBotaoClicked()
    {
        botaoInteracao.gameObject.SetActive(false);
        painelMinigame.SetActive(true);
        Catraca catracaAtual = GetComponent<Catraca>();

        catracaAtual.Consertar();
        desafioDasCores.IniciarMinigame(catracaAtual);

        Debug.Log("[AproximarCatraca] Minigame iniciado!");
    }
}
