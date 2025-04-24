using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SlidePuzzle : MonoBehaviour
{
    [SerializeField] private Button[] tiles = null; // Tiles agora s�o bot�es
    [SerializeField] private RectTransform emptySpace = null; // Espa�o vazio como UI element
    [SerializeField] private Text endPanelTimeText = null, bestTimeText = null;
    [SerializeField] private GameObject endPanel = null, newRecordText = null;
    [SerializeField] private int puzzleSize; // Tamanho do puzzle (ex.: 3 para 3x3, 4 para 4x4)

    private int emptySpaceIndex;
    private int maxIndex;
    private int maxIndexMinusOne;
    private bool isFinished;

    private void Start()
    {
        maxIndex = puzzleSize * puzzleSize - 1;
        maxIndexMinusOne = maxIndex - 1;
        emptySpaceIndex = maxIndex;

        ConfigureGrid(); // Posiciona os bot�es corretamente na grade
        Shuffle(); // Embaralha os tiles de forma resolv�vel

        // Adiciona evento OnClick a cada bot�o
        for (int i = 0; i < tiles.Length; i++)
        {
            int index = i; // Captura o �ndice corretamente
            tiles[i].onClick.AddListener(() => OnTileClick(index));
        }
    }

    private void ConfigureGrid()
    {
        float tileSize = tiles[0].GetComponent<RectTransform>().sizeDelta.x; // Assume que todos os bot�es t�m o mesmo tamanho
        float spacing = 10f; // Espa�o entre os bot�es
        Vector2 startPosition = new Vector2(-((puzzleSize - 1) * (tileSize + spacing)) / 2, ((puzzleSize - 1) * (tileSize + spacing)) / 2);

        // Posiciona os bot�es em uma grade
        for (int i = 0; i < tiles.Length; i++)
        {
            int row = i / puzzleSize;
            int col = i % puzzleSize;
            Vector2 tilePosition = startPosition + new Vector2(col * (tileSize + spacing), -row * (tileSize + spacing));
            tiles[i].GetComponent<RectTransform>().anchoredPosition = tilePosition;
        }

        // Configura o espa�o vazio na �ltima posi��o da grade
        emptySpace.anchoredPosition = startPosition + new Vector2((puzzleSize - 1) * (tileSize + spacing), -(puzzleSize - 1) * (tileSize + spacing));
    }

    public void OnTileClick(int clickedIndex)
    {
        // Verifica se o �ndice clicado est� dentro do intervalo v�lido
        if (clickedIndex < 0 || clickedIndex >= tiles.Length)
        {
            Debug.LogError($"�ndice inv�lido: {clickedIndex}");
            return;
        }

        int emptyRow = emptySpaceIndex / puzzleSize;
        int emptyCol = emptySpaceIndex % puzzleSize;
        int clickedRow = clickedIndex / puzzleSize;
        int clickedCol = clickedIndex % puzzleSize;

        // Verifica se o bot�o clicado est� na mesma linha ou coluna que o espa�o vazio
        if ((emptyRow == clickedRow || emptyCol == clickedCol) && tiles[clickedIndex] != null)
        {
            MoveTile(clickedIndex);
        }
        else
        {
            Debug.LogWarning($"Movimento inv�lido: Tile em {clickedRow}, {clickedCol} n�o pode se mover.");
        }
    }

    private void MoveTile(int clickedIndex)
    {
        Vector2 emptyPos = emptySpace.anchoredPosition;

        // Troca as posi��es entre o bot�o clicado e o espa�o vazio
        tiles[clickedIndex].GetComponent<RectTransform>().anchoredPosition = emptyPos;
        emptySpace.anchoredPosition = tiles[clickedIndex].GetComponent<RectTransform>().anchoredPosition;

        // Atualiza os �ndices
        tiles[emptySpaceIndex] = tiles[clickedIndex];
        tiles[clickedIndex] = null;
        emptySpaceIndex = clickedIndex;

        CheckVictory();
    }

    private void CheckVictory()
    {
        int correctTiles = 0;

        foreach (var tile in tiles)
        {
            if (tile != null && tile.GetComponent<TilesScript>().inRightPlace)
            {
                correctTiles++;
            }
        }

        if (correctTiles == tiles.Length - 1)
        {
            isFinished = true;
            endPanel.SetActive(true);
        }
    }

    public void Shuffle()
    {
        int inversions;
        do
        {
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                int randomIndex = Random.Range(0, tiles.Length - 1);
                SwapTiles(i, randomIndex);
            }
            inversions = GetInversions();
        } while (!IsSolvable(inversions));
    }

    private void SwapTiles(int i, int randomIndex)
    {
        if (i < 0 || i >= tiles.Length || randomIndex < 0 || randomIndex >= tiles.Length)
        {
            Debug.LogError($"Tentativa de acessar �ndices inv�lidos: i={i}, randomIndex={randomIndex}");
            return;
        }

        // Realiza a troca entre dois tiles
        var tempTile = tiles[i];
        tiles[i] = tiles[randomIndex];
        tiles[randomIndex] = tempTile;

        Vector2 tempPosition = tiles[i].GetComponent<RectTransform>().anchoredPosition;
        tiles[i].GetComponent<RectTransform>().anchoredPosition = tiles[randomIndex].GetComponent<RectTransform>().anchoredPosition;
        tiles[randomIndex].GetComponent<RectTransform>().anchoredPosition = tempPosition;
    }

    private int GetInversions()
    {
        int inversionsSum = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null) continue;

            for (int j = i + 1; j < tiles.Length; j++)
            {
                if (tiles[j] != null && tiles[i].GetComponent<TilesScript>().number > tiles[j].GetComponent<TilesScript>().number)
                {
                    inversionsSum++;
                }
            }
        }

        return inversionsSum;
    }

    private bool IsSolvable(int inversions)
    {
        if (puzzleSize % 2 == 1) // Tamanho �mpar
        {
            return inversions % 2 == 0;
        }
        else // Tamanho par
        {
            int emptyRow = puzzleSize - (emptySpaceIndex / puzzleSize);
            return (inversions + emptyRow) % 2 == 1;
        }
    }
}
