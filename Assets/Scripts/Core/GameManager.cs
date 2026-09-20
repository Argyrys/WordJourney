using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLevel = 1;
    public int currentWorld = 1;
    public int coins = 0;
    public int hints = 3;
    public int lives = 5;
    public int maxLives = 5;
    public int streak = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLevel(int level)
    {
        currentLevel = level;
        SceneManager.LoadScene("GameScene");
    }

    public void CompleteLevel(int starsEarned, int score)
    {
        int coinsEarned = CalculateCoins(starsEarned, score);
        coins += coinsEarned;

        if (starsEarned >= 2)
        {
            streak++;
        }
        else
        {
            streak = 0;
        }

        LevelProgress progress = new LevelProgress
        {
            level = currentLevel,
            stars = starsEarned,
            score = score,
            completed = true
        };

        SaveLevelProgress(currentLevel, progress);
        SaveGameData();

        UIManager.Instance?.ShowLevelComplete(starsEarned, coinsEarned);
    }

    public void UseHint()
    {
        if (hints > 0)
        {
            hints--;
            SaveGameData();
        }
    }

    public void AddHints(int amount)
    {
        hints += amount;
        SaveGameData();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveGameData();
    }

    public bool LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            SaveGameData();
            return true;
        }
        return false;
    }

    public void RestoreLife()
    {
        if (lives < maxLives)
        {
            lives++;
            SaveGameData();
        }
    }

    private int CalculateCoins(int stars, int score)
    {
        int baseCoins = 10;
        int starBonus = stars * 5;
        int scoreBonus = score / 100;
        return baseCoins + starBonus + scoreBonus;
    }

    private void SaveLevelProgress(int level, LevelProgress progress)
    {
        string key = $"Level_{level}";
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.SetInt("MaxLevel", Mathf.Max(PlayerPrefs.GetInt("MaxLevel", 1), level + 1));
        PlayerPrefs.Save();
    }

    public LevelProgress GetLevelProgress(int level)
    {
        string key = $"Level_{level}";
        string json = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(json))
        {
            return new LevelProgress { level = level, stars = 0, score = 0, completed = false };
        }
        return JsonUtility.FromJson<LevelProgress>(json);
    }

    private void SaveGameData()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Hints", hints);
        PlayerPrefs.SetInt("Lives", lives);
        PlayerPrefs.SetInt("Streak", streak);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("CurrentWorld", currentWorld);
        PlayerPrefs.Save();
    }

    private void LoadGameData()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        hints = PlayerPrefs.GetInt("Hints", 3);
        lives = PlayerPrefs.GetInt("Lives", 5);
        streak = PlayerPrefs.GetInt("Streak", 0);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        currentWorld = PlayerPrefs.GetInt("CurrentWorld", 1);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public int GetMaxLevel()
    {
        return PlayerPrefs.GetInt("MaxLevel", 1);
    }
}

[System.Serializable]
public class LevelProgress
{
    public int level;
    public int stars;
    public int score;
    public bool completed;
}
