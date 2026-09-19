using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WordDatabase : MonoBehaviour
{
    public static WordDatabase Instance { get; private set; }

    private Dictionary<int, List<string>> levelWords = new Dictionary<int, List<string>>();
    private List<string> allWords = new List<string>();

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
                    allWords.Add(word);
                }
            }
        }
        else
        {
            LoadDefaultWords();
        }

        GenerateLevelWords();
    }

    public List<string> GetWordsForLevel(int level)
    {
        if (levelWords.ContainsKey(level))
        {
            return levelWords[level];
        }

        return GenerateRandomWords(level, GetWordCountForLevel(level));
    }

    private int GetWordCountForLevel(int level)
    {
        int baseCount = 3;
        int increment = level / 10;
        return Mathf.Min(baseCount + increment, 8);
    }

    private List<string> GenerateRandomWords(int level, int count)
    {
        int minLength = Mathf.Min(3 + level / 20, 5);
        int maxLength = Mathf.Min(4 + level / 15, 8);

        List<string> suitableWords = allWords
            .Where(w => w.Length >= minLength && w.Length <= maxLength)
            .ToList();

        suitableWords = suitableWords.OrderBy(x => Random.value).ToList();

        List<string> selectedWords = new List<string>();
        for (int i = 0; i < Mathf.Min(count, suitableWords.Count); i++)
        {
            selectedWords.Add(suitableWords[i]);
        }

        return selectedWords;
    }

    private void GenerateLevelWords()
    {
        for (int level = 1; level <= 100; level++)
        {
            int wordCount = GetWordCountForLevel(level);
            levelWords[level] = GenerateRandomWords(level, wordCount);
        }
    }

    public bool IsValidWord(string word)
    {
        return allWords.Contains(word.ToUpper());
    }

    public List<string> GetAllWords()
    {
        return new List<string>(allWords);
    }

    public List<string> GetWordsByLength(int length)
    {
        return allWords.Where(w => w.Length == length).ToList();
    }

    public List<string> GetWordsStartingWith(char letter)
    {
        return allWords.Where(w => w.StartsWith(letter.ToString())).ToList();
    }

    private void LoadDefaultWords()
    {
        string[] words = {
            "CAT", "DOG", "BIRD", "FISH", "LION", "TIGER", "BEAR", "WOLF", "FOX", "DEER",
            "APPLE", "BANANA", "CHERRY", "GRAPE", "MELON", "LEMON", "ORANGE", "PEACH", "PLUM",
            "HOUSE", "WATER", "EARTH", "FIRE", "WIND", "RAIN", "SNOW", "STAR", "MOON", "SUN",
            "BOOK", "PEN", "PAPER", "TABLE", "CHAIR", "DOOR", "WINDOW", "FLOOR", "WALL",
            "HAPPY", "SAD", "ANGRY", "CALM", "BRAVE", "SMART", "KIND", "FUNNY", "COOL",
            "WORLD", "CITY", "VILLAGE", "MOUNTAIN", "RIVER", "OCEAN", "FOREST", "DESERT",
            "BREAD", "MILK", "CHEESE", "BUTTER", "SUGAR", "SALT", "PEPPER", "RICE",
            "CLOTHES", "SHIRT", "PANTS", "SHOES", "HAT", "JACKET", "DRESS", "SOCKS",
            "COLORS", "RED", "BLUE", "GREEN", "YELLOW", "PURPLE", "ORANGE", "PINK", "BLACK",
            "NUMBERS", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT",
            "COUNTRY", "FRANCE", "ENGLAND", "JAPAN", "CHINA", "INDIA", "BRAZIL", "CANADA",
            "ANIMAL", "PLANT", "TREE", "FLOWER", "GRASS", "ROCK", "SAND", "ICE", "CLOUD",
            "FAMILY", "MOTHER", "FATHER", "SISTER", "BROTHER", "DAUGHTER", "SON", "BABY",
            "FOOD", "DRINK", "FRUIT", "VEGETABLE", "MEAT", "FISH", "BIRD", "WATER",
            "TIME", "DAY", "NIGHT", "MORNING", "EVENING", "WEEK", "MONTH", "YEAR",
            "SPORT", "BALL", "GAME", "PLAY", "RUN", "SWIM", "JUMP", "WALK", "THROW",
            "MUSIC", "SONG", "DANCE", "SING", "PLAY", "LISTEN", "HEAR", "SOUND",
            "COLOR", "PAINT", "DRAW", "WRITE", "READ", "LEARN", "TEACH", "STUDY",
            "WORK", "JOB", "SCHOOL", "CLASS", "TEACHER", "STUDENT", "BOOK", "PEN",
            "HEALTH", "HEART", "BRAIN", "HAND", "FOOT", "HEAD", "EYE", "EAR", "NOSE",
            "WEATHER", "SUN", "RAIN", "SNOW", "WIND", "CLOUD", "STORM", "HOT", "COLD",
            "TRAVEL", "CAR", "BUS", "TRAIN", "PLANE", "BOAT", "WALK", "RIDE", "FLY"
        };

        allWords = new List<string>(words);
    }
}
