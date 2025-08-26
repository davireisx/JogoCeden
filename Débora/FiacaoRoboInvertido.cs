using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiacaoRoboInvertido : MonoBehaviour
{
    [System.Serializable]
    public class FioAnimado
    {
        public WireDragComLimite fio;
        public Transform destinoCorreto;
    }

    [Header("Lista de fios e seus destinos")]
    public List<FioAnimado> fiosSequenciais = new List<FioAnimado>();

    [Header("Configuração de animação")]
    public float tempoEntreFios = 0.5f;
    public float duracaoAnimacao = 0.5f;

    [Header("Referência da luz")]
    public GameObject lightObject; // Arraste sua luz aqui no Inspector

    private void Start()
    {
        // Luz começa sempre ligada
        if (lightObject != null)
            lightObject.SetActive(true);

        StartCoroutine(AnimarFiosSequencialmente());
    }

    private void Update()
    {
        if (lightObject == null)
            return;

        // Se algum fio estiver fora do destino, desativa a luz
        if (!TodosFiosNoDestino() && lightObject.activeSelf)
        {
            Debug.Log("aquii");
            lightObject.SetActive(false);
        }
    }


    private bool TodosFiosNoDestino()
    {
        foreach (var fioData in fiosSequenciais)
        {
            if (fioData.fio == null) continue;

            // Usa o estado de conexão, não só a posição
            if (!fioData.fio.EstaNoDestinoCorreto())
                return false;
        }
        return true;
    }


    private IEnumerator AnimarFiosSequencialmente()
    {
        foreach (var fioData in fiosSequenciais)
        {
            yield return StartCoroutine(EsticarFio(fioData.fio, fioData.destinoCorreto));
            yield return new WaitForSeconds(tempoEntreFios);
        }
    }

    private IEnumerator EsticarFio(WireDragComLimite fio, Transform destino)
    {
        if (!ValidarComponentes(fio, destino)) yield break;

        var (pontoFixo, holderVisual, parteVisual, pontaFinal) = GetComponentes(fio);

        SetupInicial(fio, destino, pontoFixo, holderVisual);
        yield return AnimacaoPrincipal(fio, destino, pontoFixo, holderVisual, parteVisual, pontaFinal);
        FinalizarAnimacao(fio, destino, pontoFixo, holderVisual, parteVisual, pontaFinal);
    }

    private bool ValidarComponentes(WireDragComLimite fio, Transform destino)
    {
        if (fio == null || destino == null || fio.pontoFixo == null ||
            fio.parteVisual == null || fio.holderVisual == null)
        {
            Debug.LogError("Componentes essenciais não configurados!");
            return false;
        }
        return true;
    }

    private (Transform, Transform, SpriteRenderer, Transform) GetComponentes(WireDragComLimite fio)
    {
        return (fio.pontoFixo, fio.holderVisual, fio.parteVisual, fio.pontaFinal);
    }

    private void SetupInicial(WireDragComLimite fio, Transform destino, Transform pontoFixo, Transform holderVisual)
    {
        Vector3 direcaoGlobal = destino.position - pontoFixo.position;
        float anguloZ = Mathf.Atan2(direcaoGlobal.y, direcaoGlobal.x) * Mathf.Rad2Deg;
        holderVisual.rotation = Quaternion.Euler(0f, 0f, anguloZ);

        Debug.DrawLine(pontoFixo.position, destino.position, Color.cyan, 5f);
    }

    private IEnumerator AnimacaoPrincipal(WireDragComLimite fio, Transform destino,
        Transform pontoFixo, Transform holderVisual, SpriteRenderer parteVisual, Transform pontaFinal)
    {
        float distanciaFinal = Vector3.Distance(pontoFixo.position, destino.position);
        float distanciaInicial = parteVisual.size.x;
        float larguraVisual = parteVisual.size.y;
        float tempo = 0f;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / duracaoAnimacao);

            AtualizarComponentes(progresso, distanciaInicial, distanciaFinal, larguraVisual,
                               pontoFixo, holderVisual, parteVisual, pontaFinal, destino);

            yield return null;
        }
    }

    private void AtualizarComponentes(float progresso, float distanciaInicial, float distanciaFinal,
       float larguraVisual, Transform pontoFixo, Transform holderVisual,
       SpriteRenderer parteVisual, Transform pontaFinal, Transform destino)
    {
        float comprimentoMundo = Mathf.Lerp(distanciaInicial, distanciaFinal, progresso);
        Vector3 direcaoMundo = (destino.position - pontoFixo.position).normalized;
        Vector3 posicaoFinalLocal = holderVisual.InverseTransformPoint(pontoFixo.position + direcaoMundo * comprimentoMundo);
        float comprimentoLocal = Mathf.Max(posicaoFinalLocal.x, 0.1f);

        parteVisual.size = new Vector2(comprimentoLocal, larguraVisual);

        if (pontaFinal != null)
            pontaFinal.localPosition = new Vector3(comprimentoLocal, 0f, 0f);

        Debug.DrawLine(pontoFixo.position, pontoFixo.position + direcaoMundo * comprimentoMundo, Color.Lerp(Color.yellow, Color.green, progresso), 0.1f);
    }

    private void FinalizarAnimacao(WireDragComLimite fio, Transform destino,
        Transform pontoFixo, Transform holderVisual, SpriteRenderer parteVisual, Transform pontaFinal)
    {
        if (pontaFinal != null)
            pontaFinal.position = destino.position;

        fio.SincronizarVisualComPontaFinal();
        fio.ConectarAutomaticamente(destino);

        // ?? Verifica se todos estão no destino correto após conectar
        if (TodosFiosNoDestino() && lightObject != null)
            lightObject.SetActive(true);
    }

}
