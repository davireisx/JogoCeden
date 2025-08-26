using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevaNocaute : MonoBehaviour
{
    [Header("Objetos a serem monitorados")]
    public List<GameObject> objetosParaMonitorar;

    [Header("Refer�ncia ao Robo")]
    public RoboController robo; // refer�ncia ao seu script do rob�

    [Header("UI")]
    public GameObject check;
    public GameObject objetivos1;
    public GameObject objetivos2;

    private bool todosDestruidos = false; // garante que s� execute uma vez

    void Update()
    {
        if (todosDestruidos || objetosParaMonitorar == null || objetosParaMonitorar.Count == 0)
            return;

        // Remove todos os objetos destru�dos da lista
        objetosParaMonitorar.RemoveAll(obj => obj == null);

        // Se a lista estiver vazia, todos foram destru�dos
        if (objetosParaMonitorar.Count == 0)
        {
            todosDestruidos = true;

            if (robo != null)
                robo.LevarNocaute();

            StartCoroutine(ChamarUI());

            Debug.Log("Todos os objetos foram destru�dos! A��o executada.");
        }
    }

    IEnumerator ChamarUI()
    {
        check.SetActive(true);
        yield return new WaitForSeconds(1f);
        objetivos1.SetActive(false);
        objetivos2.SetActive(true);
        check.SetActive(false);
    }
}
