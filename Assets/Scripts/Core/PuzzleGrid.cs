using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleGrid : MonoBehaviour
{
    public static PuzzleGrid Instance { get; private set; }

    [Header("Grid Settings")]
    public Transform gridParent;
    public GameObject letterPrefab;
    public int gridWidth = 5;
    public int gridHeight = 4;

    private LetterTile[,] grid;
    private List<LetterTile> selectedTiles = new List<LetterTile>();
    private string currentWord = "";
    private LevelData currentLevelData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetupGrid(LevelData levelData)
    {
        currentLevelData = levelData;
        ClearGrid();
        CreateGrid(levelData.gridLetters);
        WordValidator.Instance?.SetLevelWords(levelData.targetWords);
    }

    private void CreateGrid(char[] letters)
    {
        grid = new LetterTile[gridWidth, gridHeight];
        int letterIndex = 0;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (letterIndex < letters.Length)
                {
                    GameObject tileObj = Instantiate(letterPrefab, gridParent);
                    LetterTile tile = tileObj.GetComponent<LetterTile>();

                    if (tile != null)
                    {
                        tile.Initialize(letters[letterIndex], x, y);
                        grid[x, y] = tile;
                    }

                    letterIndex++;
                }
            }
        }
    }

    public void OnLetterClicked(LetterTile tile)
    {
        if (selectedTiles.Contains(tile))
        {
            int index = selectedTiles.IndexOf(tile);
            for (int i = selectedTiles.Count - 1; i >= index; i--)
            {
                selectedTiles[i].Deselect();
                selectedTiles.RemoveAt(i);
            }
        }
        else
        {
            if (selectedTiles.Count == 0 || IsAdjacent(selectedTiles[selectedTiles.Count - 1], tile))
            {
                selectedTiles.Add(tile);
                tile.Select();
            }
        }

        UpdateCurrentWord();
        UIManager.Instance?.UpdateCurrentWord(currentWord);
    }

    private bool IsAdjacent(LetterTile a, LetterTile b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return (dx <= 1 && dy <= 1) && (dx + dy > 0);
    }

    private void UpdateCurrentWord()
    {
        currentWord = "";
        foreach (LetterTile tile in selectedTiles)
        {
            currentWord += tile.letter;
        }
    }

    public void SubmitWord()
    {
        if (string.IsNullOrEmpty(currentWord) || currentWord.Length < 3)
        {
            ClearSelection();
            return;
        }

        if (WordValidator.Instance.IsValidWord(currentWord))
        {
            if (WordValidator.Instance.IsTargetWord(currentWord))
            {
                LevelManager.Instance.WordFound(currentWord);
                HighlightFoundWord();
            }
            else
            {
                UIManager.Instance?.ShowMessage("Not a target word!");
            }
        }
        else
        {
            UIManager.Instance?.ShowMessage("Invalid word!");
        }

        ClearSelection();
    }

    private void HighlightFoundWord()
    {
        foreach (LetterTile tile in selectedTiles)
        {
            tile.HighlightFound();
        }
    }

    public void RevealWord(string word)
    {
        foreach (char letter in word)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null && grid[x, y].letter == char.ToUpper(letter) && !grid[x, y].isFound)
                    {
                        grid[x, y].HighlightFound();
                        break;
                    }
                }
            }
        }
    }

    public void ClearSelection()
    {
        foreach (LetterTile tile in selectedTiles)
        {
            tile.Deselect();
        }
        selectedTiles.Clear();
        currentWord = "";
        UIManager.Instance?.UpdateCurrentWord("");
    }

    private void ClearGrid()
    {
        if (grid != null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        Destroy(grid[x, y].gameObject);
                    }
                }
            }
        }

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShuffleGrid()
    {
        List<char> letters = new List<char>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    letters.Add(grid[x, y].letter);
                }
            }
        }

        for (int i = letters.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (letters[i], letters[j]) = (letters[j], letters[i]);
        }

        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null && index < letters.Count)
                {
                    grid[x, y].UpdateLetter(letters[index]);
                    index++;
                }
            }
        }
    }
}
