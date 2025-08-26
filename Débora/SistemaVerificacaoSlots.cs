using UnityEngine;
using System.Collections;

public class SistemaVerificacaoWaves : MonoBehaviour
{

    [System.Serializable]
    public class GrupoDeWaves
    {
        public string nomeGrupo;
        public SistemaMoverManager[] sistemas; // Waves que fazem parte desse grupo

        public bool EstaFinalizado()
        {
            if (sistemas == null || sistemas.Length == 0)
                return false;

            foreach (var sistema in sistemas)
            {
                if (sistema == null || !sistema.sistemaFinalizado)
                    return false;
            }
            return true;
        }
    }

    [Header("Grupos de waves a verificar")]
    public GrupoDeWaves[] grupos;

    [Header("Refer�ncias p�s-verifica��o")]
    public GameObject check;
    public GameObject objetivo1;
    public GameObject objetivo2;

    [Header("Refer�ncia do rob�")]
    public RoboController robo; // Script do seu rob�

    private bool jaFinalizou = false;

    void Update()
    {
        if (!jaFinalizou && TodosOsGruposFinalizados())
        {
            StartCoroutine(FinalizarDesafio());
        }
    }

    private bool TodosOsGruposFinalizados()
    {
        if (grupos == null || grupos.Length == 0)
            return false;

        foreach (var grupo in grupos)
        {
            if (!grupo.EstaFinalizado())
                return false;
        }
        return true;
    }

    IEnumerator FinalizarDesafio()
    {
        jaFinalizou = true;

        if (check != null)
            check.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // Rob� leva nocaute antes de desligar o check

        if (check != null)
            check.SetActive(false);

        if (objetivo1 != null)
            objetivo1.SetActive(false);

        if (objetivo2 != null)
            objetivo2.SetActive(true);

        Debug.Log("? Todos os grupos de waves conclu�dos e rob� nocauteado!");
    }
}
