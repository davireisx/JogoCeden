using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject enemy;
    void Start()
    {
        InvokeRepeating("SpawnEnemies", 0.5f, 0.5f);
    }

   void SpawnEnemies()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Instantiate(enemy, spawnPoints[index].position ,Quaternion.identity);
    }
}
