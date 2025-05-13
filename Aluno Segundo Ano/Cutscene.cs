using UnityEngine;
using UnityEngine.SceneManagement;

public class irCutscene2 : MonoBehaviour
{
    [SerializeField] private string cutsceneSceneName; // Nome da cena da cutscene

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(cutsceneSceneName);
        }
    }
}
