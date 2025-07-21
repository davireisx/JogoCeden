using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

using static Catraca;

public class CatracaManager : MonoBehaviour
{
    [Header("Contadores de HUD")]
    public Text textoFacil;
    public Text textoMedio;
    public Text textoDificil;

    [Header("Checks de confirmação")]
    public GameObject checkFacil;
    public GameObject checkMedio;
    public GameObject checkDificil;

    [Header("Check geral de vitória")]
    public GameObject checkGeral;


    [Header("Objetos finais a desativar")]
    public GameObject trapCatraca;
    public GameObject arquiteto;

    [Header("Cenas")]
    public string nomeCenaVitoria;
    public string nomeCenaDerrota;

    private Dictionary<Dificuldade, int> resolvidas = new Dictionary<Dificuldade, int>();
    private Dictionary<Dificuldade, int> falhas = new Dictionary<Dificuldade, int>();

    private Color corVitoria = new Color(230f / 255f, 240f / 255f, 255f / 255f); // E6F0FF

    private void Start()
    {
        resolvidas[Dificuldade.Facil] = 0;
        resolvidas[Dificuldade.Medio] = 0;
        resolvidas[Dificuldade.Dificil] = 0;

        falhas[Dificuldade.Facil] = 0;
        falhas[Dificuldade.Medio] = 0;
        falhas[Dificuldade.Dificil] = 0;

        AtualizarHUD();
    }

    public void CatracaResolvida(Dificuldade dificuldade)
    {
        if (resolvidas[dificuldade] == 0)
        {
            resolvidas[dificuldade]++;
            AtualizarHUD();
            VerificarVitoria();

            // Ativa o check de confirmação correspondente
            switch (dificuldade)
            {
                case Dificuldade.Facil:
                    if (checkFacil != null) checkFacil.SetActive(true);
                    break;
                case Dificuldade.Medio:
                    if (checkMedio != null) checkMedio.SetActive(true);
                    break;
                case Dificuldade.Dificil:
                    if (checkDificil != null) checkDificil.SetActive(true);
                    break;
            }
        }
    }


    public void CatracaFalhou(Dificuldade dificuldade)
    {
        falhas[dificuldade]++;
        if (resolvidas[dificuldade] == 0 && falhas[dificuldade] >= 2)
        {
            StartCoroutine(DerrotaFinal());
        }
    }

    IEnumerator DerrotaFinal()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nomeCenaDerrota);
    }



    void AtualizarHUD()
    {
        if (textoFacil) textoFacil.text = $"Fácil:   {resolvidas[Dificuldade.Facil]}  /  1";
        if (textoMedio) textoMedio.text = $"Médio:   {resolvidas[Dificuldade.Medio]}  /  1";
        if (textoDificil) textoDificil.text = $"Difícil:   {resolvidas[Dificuldade.Dificil]}  /  1";
    }

    void VerificarVitoria()
    {
        if (resolvidas[Dificuldade.Facil] >= 1 &&
            resolvidas[Dificuldade.Medio] >= 1 &&
            resolvidas[Dificuldade.Dificil] >= 1)
        {
            if (trapCatraca) trapCatraca.SetActive(false);
            if (arquiteto) arquiteto.SetActive(false);

            // Ativa o check geral
            if (checkGeral != null) checkGeral.SetActive(true);

            // Reinicia a cor de TODAS as catracas para cor normal
            Catraca[] catracas = FindObjectsOfType<Catraca>();
            foreach (Catraca c in catracas)
            {
                c.SetColor(corVitoria);
            }

            // Espera 2 segundos e carrega a cena de vitória
            StartCoroutine(VitoriaFinal());
        }
    }


    IEnumerator VitoriaFinal()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(nomeCenaVitoria);
    }

    public bool TodasEstaoCorrompidas()
    {
        Catraca[] catracas = FindObjectsOfType<Catraca>();
        foreach (Catraca c in catracas)
        {
            if (!c.DeveAtivarBotao())
                return false;
        }
        return true;
    }


}
