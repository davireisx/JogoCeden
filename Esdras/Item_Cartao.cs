using System.Collections;                 // Necessário para IEnumerator
using UnityEngine;
using UnityEngine.UI;                    // Necessário para usar Button e Image

public class ItemInteraction : MonoBehaviour
{
    public GameObject player;            // Referência ao jogador
    public Button interactionButton;     // Botão de interação da UI
    public Image initialImage;           // Imagem que aparece 3s depois

    public CartãoInteração cartaoInteracao; // arraste o GameObject com esse script no Inspector


    private bool playerInRange = false;
    public float range = 3f;             // Range em forma de quadrado (ajustável)

    void Start()
    {
        interactionButton.gameObject.SetActive(false); // Esconde o botão no início
        initialImage.gameObject.SetActive(false);      // Esconde a imagem no início

        interactionButton.onClick.AddListener(OnInteractionButtonClicked); // Evento do botão

        StartCoroutine(ShowImageAfterDelay()); // Começa a contagem de 3s
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
        interactionButton.gameObject.SetActive(false); // Oculta o botão
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
