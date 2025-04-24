using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class countdownTimer : MonoBehaviour
{
    [Header("Configurações do Contador")]
    [SerializeField] private float tempoTotal = 30f; // 30 segundos
    [SerializeField] private Text textoContador;
    [SerializeField] private bool comecarAutomaticamente = true;
    [SerializeField] private bool contarDecrescente = true;

    private float tempoRestante;
    private bool contadorAtivo = false;
    private Coroutine contadorCoroutine;

    // Evento para quando o tempo acabar
    public event Action OnTimerComplete;

    private void Start()
    {
        if (comecarAutomaticamente)
        {
            IniciarContador();
        }
    }

    public void IniciarContador()
    {
        if (contadorCoroutine != null)
        {
            StopCoroutine(contadorCoroutine);
        }

        tempoRestante = contarDecrescente ? tempoTotal : 0f;
        contadorAtivo = true;
        contadorCoroutine = StartCoroutine(AtualizarContador());
    }

    public void PararContador()
    {
        contadorAtivo = false;
        if (contadorCoroutine != null)
        {
            StopCoroutine(contadorCoroutine);
        }
    }

    public void ResetarContador()
    {
        tempoRestante = contarDecrescente ? tempoTotal : 0f;
        AtualizarTexto();
    }

    private IEnumerator AtualizarContador()
    {
        while (contadorAtivo)
        {
            if (contarDecrescente)
            {
                tempoRestante -= Time.deltaTime;
                if (tempoRestante <= 0f)
                {
                    tempoRestante = 0f;
                    contadorAtivo = false;
                    OnTimerComplete?.Invoke();
                }
            }
            else
            {
                tempoRestante += Time.deltaTime;
                if (tempoRestante >= tempoTotal)
                {
                    tempoRestante = tempoTotal;
                    contadorAtivo = false;
                    OnTimerComplete?.Invoke();
                }
            }

            AtualizarTexto();
            yield return null;
        }
    }

    private void AtualizarTexto()
    {
        if (textoContador != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(tempoRestante);
            textoContador.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    // Métodos para controle externo
    public void AdicionarTempo(float segundos)
    {
        tempoRestante = Mathf.Clamp(tempoRestante + segundos, 0f, tempoTotal);
        AtualizarTexto();
    }

    public float GetTempoRestante()
    {
        return tempoRestante;
    }
}