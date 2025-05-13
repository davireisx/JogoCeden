using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public CameraSeguirEsdras cameraScript;

    [Header("Configurações de Spawn")]
    public float delayAtivacaoSpawn = 1.5f;

    [Header("Catraca1")]
    public float minX1;
    public float maxX1;
    public float minY1;
    public float maxY1;
    public AlunoSurgindo AlunosSpawns1;

    [Header("Catraca2")]
    public float minX2;
    public float maxX2;
    public float minY2;
    public float maxY2;
    // public AlunoSurgindo AlunosSpawns2;

    [Header("Catraca3")]
    public float minX3;
    public float maxX3;
    public float minY3;
    public float maxY3;
    // public AlunoSurgindo AlunosSpawns3;

    [Header("Catraca4")]
    public float minX4;
    public float maxX4;
    public float minY4;
    public float maxY4;
    public AlunoSurgindo AlunosSpawns4;

    [Header("Corredor1")]
    public float minX5;
    public float maxX5;
    public float minY5;
    public float maxY5;
    public AlunoSurgindo AlunosSpawns5;

    [Header("Corredor2")]
    public float minX6;
    public float maxX6;
    public float minY6;
    public float maxY6;
    // public AlunoSurgindo AlunosSpawns6;

    private void Start()
    {
        SetScenarioBounds(1);
    }

    public void SetScenarioBounds(int index)
    {
        if (cameraScript == null)
            cameraScript = Camera.main.GetComponent<CameraSeguirEsdras>();

        DesativarTodosSpawns();

        if (index == 1)
        {
            cameraScript.globalMinX = minX1;
            cameraScript.globalMaxX = maxX1;
            cameraScript.globalMinY = minY1;
            cameraScript.globalMaxY = maxY1;
            StartCoroutine(AtivarSpawnComDelay(AlunosSpawns1));
        }
        else if (index == 2)
        {
            cameraScript.globalMinX = minX2;
            cameraScript.globalMaxX = maxX2;
            cameraScript.globalMinY = minY2;
            cameraScript.globalMaxY = maxY2;
        }
        else if (index == 3)
        {
            cameraScript.globalMinX = minX3;
            cameraScript.globalMaxX = maxX3;
            cameraScript.globalMinY = minY3;
            cameraScript.globalMaxY = maxY3;
        }
        else if (index == 4)
        {
            cameraScript.globalMinX = minX4;
            cameraScript.globalMaxX = maxX4;
            cameraScript.globalMinY = minY4;
            cameraScript.globalMaxY = maxY4;
            StartCoroutine(AtivarSpawnComDelay(AlunosSpawns4));
        }
        else if (index == 5)
        {
            cameraScript.globalMinX = minX5;
            cameraScript.globalMaxX = maxX5;
            cameraScript.globalMinY = minY5;
            cameraScript.globalMaxY = maxY5;
            StartCoroutine(AtivarSpawnComDelay(AlunosSpawns5));
        }
        else if (index == 6)
        {
            cameraScript.globalMinX = minX6;
            cameraScript.globalMaxX = maxX6;
            cameraScript.globalMinY = minY6;
            cameraScript.globalMaxY = maxY6;
        }
    }

    private IEnumerator AtivarSpawnComDelay(AlunoSurgindo spawn)
    {
        yield return new WaitForSeconds(delayAtivacaoSpawn);
        spawn.gameObject.SetActive(true);
        spawn.enabled = true;
    }

    private void DesativarTodosSpawns()
    {
        AlunosSpawns1.enabled = false;
        AlunosSpawns4.enabled = false;
        AlunosSpawns5.enabled = false;

    }
}
