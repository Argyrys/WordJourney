using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    public int levelNumber = 1;
    public int targetScore = 1000;
    public int timeLimit = 120;
    public int wordsToFind = 5;

    private LevelData currentLevelData;
    private List<string> foundWords = new List<string>();
    private int currentScore = 0;
    private float timeRemaining;
    private bool isLevelActive = false;

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

    private void Start()
    {
        Invoke(nameof(StartLevel), 0.1f);
    }

    private void StartLevel()
    {
        Debug.Log("StartLevel: PuzzleGrid.Instance=" + (PuzzleGrid.Instance != null) + " WordDatabase.Instance=" + (WordDatabase.Instance != null));
        LoadLevel(levelNumber);
    }

    private void Update()
    {
        if (isLevelActive && timeLimit > 0)
        {
            timeRemaining -= Time.deltaTime;
            UIManager.Instance?.UpdateTimer(timeRemaining);

            if (timeRemaining <= 0)
            {
                EndLevel(false);
            }
        }
    }

    public void LoadLevel(int level)
    {
        levelNumber = level;
        currentLevelData = GenerateLevelData(level);
        foundWords.Clear();
        currentScore = 0;
        timeRemaining = timeLimit;
        isLevelActive = true;

        UIManager.Instance?.UpdateLevelInfo(levelNumber, currentLevelData.targetWords.Count);
        PuzzleGrid.Instance?.SetupGrid(currentLevelData);
    }

    public void WordFound(string word)
    {
        if (!foundWords.Contains(word) && currentLevelData.targetWords.Contains(word))
        {
            foundWords.Add(word);
            int wordScore = CalculateWordScore(word);
            currentScore += wordScore;

            UIManager.Instance?.WordFound(word, wordScore);
            UIManager.Instance?.UpdateProgress(foundWords.Count, currentLevelData.targetWords.Count);

            if (foundWords.Count >= currentLevelData.targetWords.Count)
            {
                EndLevel(true);
            }
        }
    }

    public void UseHintForWord()
    {
        if (GameManager.Instance.hints > 0)
        {
            foreach (string word in currentLevelData.targetWords)
            {
                if (!foundWords.Contains(word))
                {
                    GameManager.Instance.UseHint();
                    WordFound(word);
                    PuzzleGrid.Instance?.RevealWord(word);
                    break;
                }
            }
        }
    }

    private int CalculateWordScore(string word)
    {
        int baseScore = word.Length * 10;
        int lengthBonus = word.Length > 4 ? (word.Length - 4) * 15 : 0;
        return baseScore + lengthBonus;
    }

    private void EndLevel(bool success)
    {
        isLevelActive = false;

        int stars = CalculateStars();
        GameManager.Instance.CompleteLevel(stars, currentScore);
    }

    private int CalculateStars()
    {
        if (foundWords.Count >= currentLevelData.targetWords.Count)
        {
            float timeBonus = timeRemaining / timeLimit;
            if (timeBonus > 0.5f) return 3;
            if (timeBonus > 0.25f) return 2;
            return 1;
        }
        return 0;
    }

    private LevelData GenerateLevelData(int level)
    {
        LevelData data = new LevelData();
        data.levelNumber = level;

        int worldIndex = Mathf.CeilToInt(level / 10f);
        data.worldTheme = GetWorldTheme(worldIndex);

        List<string> availableWords = WordDatabase.Instance.GetWordsForLevel(level);
        data.targetWords = availableWords;

        data.gridLetters = GenerateGridLetters(availableWords);

        return data;
    }

    private string GetWorldTheme(int world)
    {
        string[] themes = { "Paris", "Tokyo", "New York", "London", "Rome", "Sydney", "Dubai", "Singapore" };
        return themes[(world - 1) % themes.Length];
    }

    private char[] GenerateGridLetters(List<string> words)
    {
        HashSet<char> letters = new HashSet<char>();
        foreach (string word in words)
        {
            foreach (char c in word)
            {
                letters.Add(char.ToUpper(c));
            }
        }

        while (letters.Count < 20)
        {
            letters.Add((char)Random.Range('A', 'Z' + 1));
        }

        List<char> letterList = new List<char>(letters);
        for (int i = letterList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (letterList[i], letterList[j]) = (letterList[j], letterList[i]);
        }

        return letterList.ToArray();
    }
}

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public string worldTheme;
    public List<string> targetWords;
    public char[] gridLetters;
    public int timeLimit = 120;
    public int targetScore = 1000;
}
