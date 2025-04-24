using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChargeBarController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configurações")]
    [SerializeField] private Animator barraAnimator;
    [SerializeField] private float tempoMaximoCharge = 2f;

    [Header("Parâmetros do Animator")]
    [SerializeField] private string parametroCharge = "ChargeLevel";
    [SerializeField] private string parametroCharging = "IsCharging";

    private bool isCharging = false;
    private float currentCharge = 0f;

    private void Update()
    {
        if (!isCharging) return;

        AtualizarCharge();
        AtualizarAnimator();
    }

    private void AtualizarCharge()
    {
        currentCharge += Time.deltaTime / tempoMaximoCharge;
        currentCharge = Mathf.Clamp01(currentCharge);
    }

    private void AtualizarAnimator()
    {
        barraAnimator.SetFloat(parametroCharge, currentCharge);
    }

    public void BotaoSegurado(BaseEventData eventData)
    {
        OnPointerDown((PointerEventData)eventData);
    }

    public void BotaoSolto(BaseEventData eventData)
    {
        OnPointerUp((PointerEventData)eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        IniciarCharge();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        FinalizarCharge();
    }

    private void IniciarCharge()
    {
        isCharging = true;
        currentCharge = 0f;
        barraAnimator.SetBool(parametroCharging, true);
        barraAnimator.SetFloat(parametroCharge, 0f);
    }

    private void FinalizarCharge()
    {
        isCharging = false;
        barraAnimator.SetBool(parametroCharging, false);

        int faseAtual = DeterminarFase();
        Debug.Log($"Tiro disparado - Fase {faseAtual + 1}");

        // Exemplo de chamada de disparo:
        // ShootProjectile(faseAtual);
    }

    private int DeterminarFase()
    {
        if (currentCharge >= 0.75f) return 3;
        if (currentCharge >= 0.5f) return 2;
        if (currentCharge >= 0.25f) return 1;
        return 0;
    }
}
