using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WordDatabase : MonoBehaviour
{
    public static WordDatabase Instance { get; private set; }

    private List<string> allWords = new List<string>();
    private Dictionary<int, List<string>> levelCache = new Dictionary<int, List<string>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadDatabase()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("WordDatabase");
        if (wordFile != null)
        {
            string[] lines = wordFile.text.Split('\n');
            foreach (string line in lines)
            {
                string word = line.Trim().ToUpper();
                if (!string.IsNullOrEmpty(word) && word.Length >= 3 && word.Length <= 8)
                {
                    if (!allWords.Contains(word))
                        allWords.Add(word);
                }
            }
        }

        Debug.Log($"WordDatabase loaded {allWords.Count} words");
    }

    public List<string> GetWordsForLevel(int level)
    {
        if (levelCache.ContainsKey(level))
            return levelCache[level];

        int wordCount = GetWordCountForLevel(level);
        int minLength = 3;
        int maxLength = Mathf.Min(3 + level / 10, 6);

        if (level <= 5)
        {
            minLength = 3;
            maxLength = 4;
        }

        List<string> suitableWords = allWords
            .Where(w => w.Length >= minLength && w.Length <= maxLength)
            .OrderBy(x => Random.value)
            .ToList();

        List<string> selectedWords = new List<string>();
        HashSet<char> usedLetters = new HashSet<char>();

        foreach (string word in suitableWords)
        {
            if (selectedWords.Count >= wordCount)
                break;

            char[] wordLetters = word.ToCharArray();
            bool canForm = true;

            foreach (char c in wordLetters)
            {
                int needed = wordLetters.Count(x => x == c);
                int available = usedLetters.Count(x => x == c);
                int extraSlots = 20 - GetTotalLetterCount(usedLetters);

                if (needed > available && needed - available > extraSlots)
                {
                    canForm = false;
                    break;
                }
            }

            if (canForm || selectedWords.Count < 2)
            {
                selectedWords.Add(word);
                foreach (char c in word)
                    usedLetters.Add(c);
            }
        }

        while (selectedWords.Count < wordCount && selectedWords.Count < wordCount)
        {
            string fallback = suitableWords.FirstOrDefault(w => !selectedWords.Contains(w));
            if (fallback != null)
                selectedWords.Add(fallback);
            else
                break;
        }

        levelCache[level] = selectedWords;
        return selectedWords;
    }

    private int GetTotalLetterCount(HashSet<char> letters)
    {
        int total = 0;
        foreach (char c in letters)
            total++;
        return total;
    }

    private int GetWordCountForLevel(int level)
    {
        if (level <= 3) return 6;
        if (level <= 10) return 8;
        if (level <= 20) return 10;
        if (level <= 50) return 11;
        return Mathf.Min(12, 8 + level / 20);
    }

    public bool IsValidWord(string word)
    {
        return allWords.Contains(word.ToUpper());
    }

    public List<string> GetAllWords()
    {
        return new List<string>(allWords);
    }
}
