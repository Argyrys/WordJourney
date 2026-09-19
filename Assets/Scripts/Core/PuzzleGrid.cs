using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PuzzleGrid : MonoBehaviour
{
    public static PuzzleGrid Instance { get; private set; }

    [Header("Grid Settings")]
    public Transform gridParent;
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

        if (gridParent == null)
        {
            gridParent = transform;
        }

        Debug.Log("SetupGrid called with " + levelData.gridLetters.Length + " letters, gridParent: " + gridParent.name);
        CreateGrid(levelData.gridLetters);
        WordValidator.Instance?.SetLevelWords(levelData.targetWords);
    }

    private void CreateGrid(char[] letters)
    {
        grid = new LetterTile[gridWidth, gridHeight];
        int letterIndex = 0;

        float tileSize = 80f;
        float spacing = 10f;
        float totalWidth = gridWidth * (tileSize + spacing) - spacing;
        float totalHeight = gridHeight * (tileSize + spacing) - spacing;
        float startX = -totalWidth / 2f + tileSize / 2f;
        float startY = totalHeight / 2f - tileSize / 2f;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (letterIndex < letters.Length)
                {
                    GameObject tileObj = CreateTile(letters[letterIndex], x, y);
                    tileObj.transform.SetParent(gridParent, false);

                    LetterTile tile = tileObj.GetComponent<LetterTile>();
                    if (tile != null)
                    {
                        tile.Initialize(letters[letterIndex], x, y);
                        grid[x, y] = tile;
                    }

                    RectTransform rect = tileObj.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.anchorMin = new Vector2(0.5f, 0.5f);
                        rect.anchorMax = new Vector2(0.5f, 0.5f);
                        rect.anchoredPosition = new Vector2(
                            startX + x * (tileSize + spacing),
                            startY - y * (tileSize + spacing)
                        );
                        rect.sizeDelta = new Vector2(tileSize, tileSize);
                    }

                    letterIndex++;
                }
            }
        }
    }

    private GameObject CreateTile(char letter, int x, int y)
    {
        GameObject tileObj = new GameObject($"Tile_{x}_{y}");
        RectTransform rect = tileObj.AddComponent<RectTransform>();
        Image img = tileObj.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.95f);

        GameObject textObj = new GameObject("LetterText");
        textObj.transform.SetParent(tileObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = letter.ToString();
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;

        LetterTile tile = tileObj.AddComponent<LetterTile>();
        tile.letterText = tmp;
        tile.backgroundImage = img;

        return tileObj;
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

        if (WordValidator.Instance != null && WordValidator.Instance.IsValidWord(currentWord))
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

        if (gridParent != null)
        {
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }
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
