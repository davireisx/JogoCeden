using System.Collections;                 // Necess�rio para IEnumerator
using UnityEngine;
using UnityEngine.UI;                    // Necess�rio para usar Button e Image

public class ItemInteraction : MonoBehaviour
{
    public GameObject player;            // Refer�ncia ao jogador
    public Button interactionButton;     // Bot�o de intera��o da UI
    public Image initialImage;           // Imagem que aparece 3s depois

    public Cart�oIntera��o cartaoInteracao; // arraste o GameObject com esse script no Inspector


    private bool playerInRange = false;
    public float range = 3f;             // Range em forma de quadrado (ajust�vel)

    void Start()
    {
        interactionButton.gameObject.SetActive(false); // Esconde o bot�o no in�cio
        initialImage.gameObject.SetActive(false);      // Esconde a imagem no in�cio

        interactionButton.onClick.AddListener(OnInteractionButtonClicked); // Evento do bot�o

        StartCoroutine(ShowImageAfterDelay()); // Come�a a contagem de 3s
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 playerPos = player.transform.position;
            Vector2 itemPos = transform.position;

            bool isInX = Mathf.Abs(playerPos.x - itemPos.x) <= range;
            bool isInY = Mathf.Abs(playerPos.y - itemPos.y) <= range;

            if (isInX && isInY)
            {
                if (!playerInRange)
                {
                    playerInRange = true;
                    interactionButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (playerInRange)
                {
                    playerInRange = false;
                    interactionButton.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator ShowImageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        initialImage.gameObject.SetActive(true);
    }

    void OnInteractionButtonClicked()
    {
        Destroy(gameObject);                         // Destroi o item
        interactionButton.gameObject.SetActive(false); // Oculta o bot�o
        initialImage.gameObject.SetActive(false);      // Oculta a imagem
        cartaoInteracao.interactionRange = 7f;
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(range * 2, range * 2, 0));
    }
}
