using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI wordsFoundText;
    public TextMeshProUGUI currentWordText;

    [Header("Currency UI")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI hintsText;

    [Header("Panels")]
    public GameObject gamePanel;
    public GameObject levelCompletePanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Level Complete")]
    public TextMeshProUGUI completeLevelText;
    public TextMeshProUGUI completeScoreText;
    public TextMeshProUGUI completeCoinsText;
    public Image[] starsImages;
    public Color starActiveColor = new Color(1f, 0.85f, 0f);
    public Color starInactiveColor = new Color(0.3f, 0.3f, 0.35f);

    [Header("Buttons")]
    public Button submitButton;
    public Button hintButton;
    public Button shuffleButton;
    public Button pauseButton;
    public Button nextLevelButton;
    public Button menuButton;

    [Header("Messages")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

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
        SetupButtons();
        UpdateCurrencyUI();
    }

    private void SetupButtons()
    {
        submitButton?.onClick.AddListener(OnSubmitClicked);
        hintButton?.onClick.AddListener(OnHintClicked);
        shuffleButton?.onClick.AddListener(OnShuffleClicked);
        pauseButton?.onClick.AddListener(OnPauseClicked);
        nextLevelButton?.onClick.AddListener(OnNextLevelClicked);
        menuButton?.onClick.AddListener(OnMenuClicked);
    }

    public void UpdateLevelInfo(int level, int totalWords)
    {
        levelText.text = $"Level {level}";
        wordsFoundText.text = $"0/{totalWords}";
        scoreText.text = "0";
    }

    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";

        if (time <= 10)
        {
            timerText.color = new Color(1f, 0.3f, 0.3f);
        }
        else if (time <= 30)
        {
            timerText.color = new Color(1f, 0.7f, 0.2f);
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateProgress(int found, int total)
    {
        wordsFoundText.text = $"{found}/{total}";
    }

    public void UpdateCurrentWord(string word)
    {
        if (currentWordText != null)
        {
            currentWordText.text = word;
            if (!string.IsNullOrEmpty(word))
            {
                currentWordText.transform.localScale = Vector3.one * 1.05f;
                StartCoroutine(ResetScale(currentWordText.transform, 0.1f));
            }
        }
    }

    private IEnumerator ResetScale(Transform t, float duration)
    {
        yield return new WaitForSeconds(duration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.one * 1.05f, Vector3.one, elapsed / duration);
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    public void UpdateCurrencyUI()
    {
        if (GameManager.Instance != null)
        {
            coinsText.text = GameManager.Instance.coins.ToString();
            hintsText.text = GameManager.Instance.hints.ToString();
        }
    }

    public void WordFound(string word, int score)
    {
        ShowMessage($"+{score} - {word}!");
        UpdateScore(score);
    }

    public void ShowLevelComplete(int stars, int coinsEarned)
    {
        levelCompletePanel.SetActive(true);
        completeLevelText.text = $"Level {GameManager.Instance.currentLevel}";
        completeScoreText.text = $"Score: {scoreText.text}";
        completeCoinsText.text = $"+{coinsEarned} Coins";

        for (int i = 0; i < starsImages.Length; i++)
        {
            starsImages[i].color = i < stars ? starActiveColor : starInactiveColor;
        }

        UpdateCurrencyUI();
        AudioManager.Instance?.PlayLevelComplete();
    }

    public void ShowMessage(string message)
    {
        if (messageText != null && messagePanel != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideMessageAfterDelay(1.5f));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePauseMenu()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnSubmitClicked()
    {
        PuzzleGrid.Instance?.SubmitWord();
    }

    private void OnHintClicked()
    {
        LevelManager.Instance?.UseHintForWord();
        UpdateCurrencyUI();
    }

    private void OnShuffleClicked()
    {
        PuzzleGrid.Instance?.ShuffleGrid();
    }

    private void OnPauseClicked()
    {
        ShowPauseMenu();
    }

    private void OnNextLevelClicked()
    {
        int nextLevel = GameManager.Instance.currentLevel + 1;
        GameManager.Instance.StartLevel(nextLevel);
    }

    private void OnMenuClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ReturnToMenu();
    }
}
