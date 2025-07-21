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

    private void Start()
    {
        StartCoroutine(AnimarFiosSequencialmente());
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
        // Validação reforçada
        if (!ValidarComponentes(fio, destino)) yield break;

        // Obter referências
        var (pontoFixo, holderVisual, parteVisual, pontaFinal) = GetComponentes(fio);

        // Configuração inicial
        SetupInicial(fio, destino, pontoFixo, holderVisual);

        // Animação principal
        yield return AnimacaoPrincipal(fio, destino, pontoFixo, holderVisual, parteVisual, pontaFinal);

        // Finalização precisa
        FinalizarAnimacao(fio, destino, pontoFixo, holderVisual, parteVisual, pontaFinal);
    }

    // Métodos auxiliares
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

            // Atualização sincronizada
            AtualizarComponentes(progresso, distanciaInicial, distanciaFinal, larguraVisual,
                               pontoFixo, holderVisual, parteVisual, pontaFinal, destino);

            yield return null;
        }
    }

    private void AtualizarComponentes(float progresso, float distanciaInicial, float distanciaFinal,
       float larguraVisual, Transform pontoFixo, Transform holderVisual,
       SpriteRenderer parteVisual, Transform pontaFinal, Transform destino)
    {
        // Calcula o comprimento atual em mundo (distância linear)
        float comprimentoMundo = Mathf.Lerp(distanciaInicial, distanciaFinal, progresso);

        // Calcula a direção do fio no mundo
        Vector3 direcaoMundo = (destino.position - pontoFixo.position).normalized;

        // Transforma a posição final para o espaço local do holderVisual
        Vector3 posicaoFinalLocal = holderVisual.InverseTransformPoint(pontoFixo.position + direcaoMundo * comprimentoMundo);

        // Usa o componente X do espaço local para ajustar o tamanho
        float comprimentoLocal = Mathf.Max(posicaoFinalLocal.x, 0.1f);

        // Atualiza o tamanho local do sprite
        parteVisual.size = new Vector2(comprimentoLocal, larguraVisual);

        // Atualiza a posição local da ponta para coincidir com o final do sprite
        if (pontaFinal != null)
            pontaFinal.localPosition = new Vector3(comprimentoLocal, 0f, 0f);

        // Opcional: debug visual para conferir no mundo
        Debug.DrawLine(pontoFixo.position, pontoFixo.position + direcaoMundo * comprimentoMundo, Color.Lerp(Color.yellow, Color.green, progresso), 0.1f);
    }



    private void FinalizarAnimacao(WireDragComLimite fio, Transform destino,
     Transform pontoFixo, Transform holderVisual, SpriteRenderer parteVisual, Transform pontaFinal)
    {
        if (pontaFinal != null)
            pontaFinal.position = destino.position;

        fio.SincronizarVisualComPontaFinal();
        fio.ConectarAutomaticamente(destino);

    }

}
