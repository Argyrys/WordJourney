using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WordValidator : MonoBehaviour
{
    public static WordValidator Instance { get; private set; }

    private HashSet<string> validWords = new HashSet<string>();
    private HashSet<string> currentLevelWords = new HashSet<string>();
    private HashSet<string> bonusWords = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadWordDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLevelWords(List<string> words)
    {
        currentLevelWords.Clear();
        foreach (string word in words)
        {
            currentLevelWords.Add(word.ToUpper());
        }
    }

    public bool IsValidWord(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length < 3)
            return false;

        string upperWord = word.ToUpper();
        return validWords.Contains(upperWord);
    }

    public bool IsTargetWord(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        string upperWord = word.ToUpper();
        return currentLevelWords.Contains(upperWord);
    }

    public List<string> GetValidWordsFromLetters(char[] letters)
    {
        List<string> foundWords = new List<string>();
        string letterString = new string(letters.Select(char.ToUpper).ToArray());

        foreach (string word in currentLevelWords)
        {
            if (CanFormWord(word, letterString))
            {
                foundWords.Add(word);
            }
        }

        return foundWords;
    }

    public void SetBonusWords(List<string> words)
    {
        bonusWords.Clear();
        foreach (string word in words)
        {
            bonusWords.Add(word.ToUpper());
        }
    }

    public bool IsBonusWord(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;
        return bonusWords.Contains(word.ToUpper());
    }

    public List<string> FindBonusWords(char[] letters, List<string> excludeWords, int count)
    {
        List<string> found = new List<string>();
        string letterString = new string(letters.Select(char.ToUpper).ToArray());
        HashSet<string> excluded = new HashSet<string>();

        foreach (string word in excludeWords)
        {
            excluded.Add(word.ToUpper());
        }

        foreach (string word in validWords)
        {
            if (found.Count >= count) break;
            if (excluded.Contains(word) || bonusWords.Contains(word)) continue;
            if (CanFormWord(word, letterString))
            {
                found.Add(word);
            }
        }

        return found;
    }

    private bool CanFormWord(string word, string availableLetters)
    {
        Dictionary<char, int> letterCount = new Dictionary<char, int>();
        foreach (char c in availableLetters)
        {
            if (letterCount.ContainsKey(c))
                letterCount[c]++;
            else
                letterCount[c] = 1;
        }

        foreach (char c in word)
        {
            if (!letterCount.ContainsKey(c) || letterCount[c] <= 0)
                return false;
            letterCount[c]--;
        }

        return true;
    }

    public List<string> GetHintsForWord(string word, int maxHints = 3)
    {
        List<string> hints = new List<string>();

        if (word.Length >= 3)
        {
            hints.Add($"Starts with '{word[0]}'");
        }
        if (word.Length >= 4)
        {
            hints.Add($"Length: {word.Length} letters");
        }
        if (word.Length >= 5)
        {
            hints.Add($"Contains '{word[word.Length / 2]}'");
        }

        return hints.Take(maxHints).ToList();
    }

    private void LoadWordDatabase()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("WordDatabase");
        if (wordFile != null)
        {
            string[] words = wordFile.text.Split('\n');
            foreach (string word in words)
            {
                string trimmedWord = word.Trim().ToUpper();
                if (!string.IsNullOrEmpty(trimmedWord) && trimmedWord.Length >= 3)
                {
                    validWords.Add(trimmedWord);
                }
            }
        }
        else
        {
            LoadDefaultWords();
        }
    }

    private void LoadDefaultWords()
    {
        string[] defaultWords = {
            "CAT", "DOG", "BIRD", "FISH", "LION", "TIGER", "BEAR", "WOLF", "FOX", "DEER",
            "APPLE", "BANANA", "CHERRY", "GRAPE", "MELON", "LEMON", "ORANGE", "PEACH", "PLUM",
            "HOUSE", "WATER", "EARTH", "FIRE", "WIND", "RAIN", "SNOW", "STAR", "MOON", "SUN",
            "BOOK", "PEN", "PAPER", "TABLE", "CHAIR", "DOOR", "WINDOW", "FLOOR", "WALL",
            "HAPPY", "SAD", "ANGRY", "CALM", "BRAVE", "SMART", "KIND", "FUNNY", "COOL",
            "WORLD", "CITY", "VILLAGE", "MOUNTAIN", "RIVER", "OCEAN", "FOREST", "DESERT",
            "BREAD", "MILK", "CHEESE", "BUTTER", "SUGAR", "SALT", "PEPPER", "RICE",
            "CLOTHES", "SHIRT", "PANTS", "SHOES", "HAT", "JACKET", "DRESS", "SOCKS",
            "COLORS", "RED", "BLUE", "GREEN", "YELLOW", "PURPLE", "ORANGE", "PINK", "BLACK",
            "NUMBERS", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT"
        };

        foreach (string word in defaultWords)
        {
            validWords.Add(word.ToUpper());
        }
    }
}
