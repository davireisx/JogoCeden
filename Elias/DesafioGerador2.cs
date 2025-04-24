using UnityEngine;
using System.Collections;

public class DesafioGerador2 : MonoBehaviour
{
    public GameObject Gerador2Object;
    public GameObject fase1Object;
    public GameObject fase2Object;
    public GameObject fase3Object;
    public GameObject fase4Object;
    public GameObject particlesPrefab;
    public float tempoPorFase = 5f;

    public void IniciarDesafio()
    {
        StartCoroutine(ExecutarFases());
    }

    private IEnumerator ExecutarFases()
    {
        Gerador2Object.SetActive(true);

        yield return ExecutarFase(fase1Object, "Desafio 2 - Fase 1 iniciada!", "Desafio 2 - Fase 1 concluída!");
        yield return ExecutarFase(fase2Object, "Desafio 2 - Fase 2 iniciada!", "Desafio 2 - Fase 2 concluída!");
        yield return ExecutarFase(fase3Object, "Desafio 2 - Fase 3 iniciada!", "Desafio 2 - Fase 3 concluída!");

        fase4Object.SetActive(true);
        Debug.Log("Desafio 2 - Fase 4 iniciada!");
        yield return new WaitForSeconds(tempoPorFase);
        Debug.Log("Desafio 2 - Fase 4 concluída!");
    }

    private IEnumerator ExecutarFase(GameObject faseObject, string mensagemInicio, string mensagemFim)
    {
        faseObject.SetActive(true);
        GameObject particlesInstance = Instantiate(particlesPrefab, faseObject.transform.position, Quaternion.identity);
        particlesInstance.GetComponent<ParticleSystem>().Play();

        Debug.Log(mensagemInicio);
        yield return new WaitForSeconds(tempoPorFase);

        faseObject.SetActive(false);
        Destroy(particlesInstance);
        Debug.Log(mensagemFim);
    }
}
