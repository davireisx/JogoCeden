using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class Dialogo : MonoBehaviour
{
    [Header("Refer�ncias")]
    public Transform player;
    public GameObject joystick;
    public GameObject hud;

    [Header("Caixas de Di�logo")]
    public GameObject caixaPersonagem1;
    public GameObject caixaPersonagem2;
    public Button botaoAvancar1;
    public Button botaoAvancar2;
    public AudioSource next;

    [Header("Falas")]
    public GameObject[] falas;
    public bool[] falasDoPersonagem1; // <-- Define quem fala em cada �ndice

    [Header("Configura��o do Di�logo Autom�tico")]
    public float tempoAntesDeIniciar = 1f;

    private int falaAtual = 0;
    private bool dialogoAtivo = false;

    void Start()
    {
        DesativarFalas();

        caixaPersonagem1?.SetActive(false);
        caixaPersonagem2?.SetActive(false);


        StartCoroutine(IniciarDialogoAutomaticamente());
    }

    IEnumerator IniciarDialogoAutomaticamente()
    {
        yield return new WaitForSeconds(tempoAntesDeIniciar);
        IniciarDialogo();
    }

    public void IniciarDialogo()
    {
        falaAtual = 0;
        dialogoAtivo = true;

        hud?.SetActive(false);
        joystick?.SetActive(false);

        AtualizarCaixaDialogo();
        MostrarFalaAtual();
    }

    public void AvancarFala()
    {
        next.Play();
        falaAtual++;

        if (falaAtual < falas.Length)
        {
            AtualizarCaixaDialogo();
            MostrarFalaAtual();
        }
        else
        {
            DesativarFalas();
            caixaPersonagem1?.SetActive(false);
            caixaPersonagem2?.SetActive(false);
            dialogoAtivo = false;

            hud?.SetActive(true);
            joystick?.SetActive(true);
        }
    }

    void AtualizarCaixaDialogo()
    {
        if (falaAtual >= falasDoPersonagem1.Length) return;

        bool personagem1Falando = falasDoPersonagem1[falaAtual];

        caixaPersonagem1?.SetActive(personagem1Falando);
        caixaPersonagem2?.SetActive(!personagem1Falando);

        botaoAvancar1?.gameObject.SetActive(personagem1Falando);
        botaoAvancar2?.gameObject.SetActive(!personagem1Falando);//
    }

    void MostrarFalaAtual()
    {
        for (int i = 0; i < falas.Length; i++)
        {
            if (falas[i] != null)
                falas[i].SetActive(i == falaAtual);
        }
    }

    void DesativarFalas()
    {
        foreach (var fala in falas)
            if (fala != null) fala.SetActive(false);
    }
}
