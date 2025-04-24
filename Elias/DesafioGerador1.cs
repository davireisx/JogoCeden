using UnityEngine;
using System.Collections;

public class DesafioGerador1 : MonoBehaviour
{
    [Header("Geradores e Fases")]
    public GameObject Gerador1Object; // Gerador respons�vel pelas fases
    public GameObject fase1Object;    // Objeto da fase 1
    public GameObject fase2Object;    // Objeto da fase 2
    public GameObject fase3Object;    // Objeto da fase 3
    public GameObject fase4Object;    // Objeto da fase 4

    [Header("Configura��o de Part�culas")]
    public GameObject particlesPrefab; // Prefab para part�culas visuais
    public float tempoPorFase = 7f;    // Tempo de dura��o de cada fase

    public Botao2 botao2; // Refer�ncia ao bot�o 2

    /// <summary>
    /// Inicia o desafio ao executar as fases do gerador.
    /// </summary>
    public void IniciarDesafio()
    {
        StartCoroutine(ExecutarFases());
    }

    /// <summary>
    /// Executa as fases do gerador em sequ�ncia com part�culas e controle de tempo.
    /// </summary>
    private IEnumerator ExecutarFases()
    {
        // Ativa o gerador principal
        Gerador1Object.SetActive(true);

        // Fase 1
        yield return ExecutarFase(fase1Object, "Fase 1 iniciada!", "Fase 1 conclu�da!");

        // Fase 2
        yield return ExecutarFase(fase2Object, "Fase 2 iniciada!", "Fase 2 conclu�da!");

        // Fase 3
        yield return ExecutarFase(fase3Object, "Fase 3 iniciada!", "Fase 3 conclu�da!");

        // Fase 4
        fase4Object.SetActive(true);
        Debug.Log("Fase 4 iniciada!");
        yield return new WaitForSeconds(tempoPorFase);
        Debug.Log("Fase 4 conclu�da!");

        // Ativa o bot�o 2 ap�s a conclus�o
        botao2.DesafioGerador1Concluido();
    }

    /// <summary>
    /// Executa uma fase espec�fica com part�culas e mensagens.
    /// </summary>
    /// <param name="faseObject">Objeto da fase atual.</param>
    /// <param name="mensagemInicio">Mensagem exibida ao iniciar a fase.</param>
    /// <param name="mensagemFim">Mensagem exibida ao finalizar a fase.</param>
    private IEnumerator ExecutarFase(GameObject faseObject, string mensagemInicio, string mensagemFim)
    {
        // Ativa o objeto da fase
        faseObject.SetActive(true);

        // Instancia e toca as part�culas
        GameObject particlesInstance = Instantiate(particlesPrefab, faseObject.transform.position, Quaternion.identity);
        particlesInstance.GetComponent<ParticleSystem>().Play();

        // Exibe mensagem de in�cio
        Debug.Log(mensagemInicio);
        yield return new WaitForSeconds(tempoPorFase);

        // Desativa o objeto da fase e destr�i as part�culas
        faseObject.SetActive(false);
        Destroy(particlesInstance);
        Debug.Log(mensagemFim);
    }
}
