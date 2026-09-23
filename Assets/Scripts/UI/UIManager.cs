using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Top Bar")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI currentWordText;
    public TextMeshProUGUI heartsText;
    public TextMeshProUGUI coinDisplayText;

    [Header("Progress")]
    public Image progressBar;
    public TextMeshProUGUI progressText;

    [Header("Buttons")]
    public Button submitButton;
    public Button hintButton;
    public Button shuffleButton;
    public Button bonusButton;
    public Button backButton;
    public Button plusButton;
    public Button continueButton;

    [Header("Level Complete")]
    public TextMeshProUGUI completeScoreText;
    public TextMeshProUGUI completeCoinsText;
    public TextMeshProUGUI starsText;

    [Header("Panels")]
    public GameObject levelCompletePanel;
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        submitButton?.onClick.AddListener(() => {});
        hintButton?.onClick.AddListener(() => LevelManager.Instance?.UseHintForWord());
        shuffleButton?.onClick.AddListener(() => LevelManager.Instance?.ShuffleLetters());
        bonusButton?.onClick.AddListener(() => LevelManager.Instance?.RevealBonusWord());
        backButton?.onClick.AddListener(() => GameManager.Instance?.ReturnToMenu());
        plusButton?.onClick.AddListener(() => GameManager.Instance?.ReturnToMenu());
        continueButton?.onClick.AddListener(() => GameManager.Instance?.ContinueToNextLevel());
        UpdateCurrencyUI();
    }

    public void UpdateLevelInfo(int level, int totalWords)
    {
        levelText.text = $"Level {level}";
        scoreText.text = "0";
        UpdateProgress(0f);
    }

    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";

        if (time <= 10)
            timerText.color = new Color(1f, 0.3f, 0.3f);
        else if (time <= 30)
            timerText.color = new Color(1f, 0.7f, 0.2f);
        else
            timerText.color = Color.white;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
            progressBar.fillAmount = progress;
        if (progressText != null)
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    public void UpdateCurrentWord(string word)
    {
        if (currentWordText != null)
        {
            currentWordText.text = word;
            if (!string.IsNullOrEmpty(word))
            {
                StopAllCoroutines();
                StartCoroutine(PulseWord());
            }
        }
    }

    private IEnumerator PulseWord()
    {
        currentWordText.transform.localScale = Vector3.one * 1.12f;
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            currentWordText.transform.localScale = Vector3.Lerp(Vector3.one * 1.12f, Vector3.one, elapsed / 0.15f);
            yield return null;
        }
        currentWordText.transform.localScale = Vector3.one;
    }

    public void UpdateCurrencyUI()
    {
        if (GameManager.Instance != null)
        {
            if (coinsText != null)
                coinsText.text = $"\u25C6 {GameManager.Instance.coins}";
            if (coinDisplayText != null)
                coinDisplayText.text = $"{GameManager.Instance.coins}";
            UpdateHearts();
        }
    }

    public void UpdateHearts()
    {
        if (heartsText != null && GameManager.Instance != null)
        {
            string hearts = "";
            for (int i = 0; i < GameManager.Instance.maxLives; i++)
            {
                hearts += i < GameManager.Instance.lives ? "\u2665" : "\u2661";
            }
            heartsText.text = hearts;
        }
    }

    public void WordFound(string word, int score)
    {
        ShowMessage($"+{score} - {word}!");
        UpdateScore(score);
        AudioManager.Instance?.PlayWordFound();
    }

    public void ShowLevelComplete(int stars, int coinsEarned, int score)
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (completeScoreText != null)
            completeScoreText.text = score.ToString();
        if (completeCoinsText != null)
            completeCoinsText.text = $"+{coinsEarned}";
        if (starsText != null)
        {
            string starString = "";
            for (int i = 0; i < 3; i++)
                starString += i < stars ? "\u2605" : "\u2606";
            starsText.text = starString;
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
            StartCoroutine(HideMessage());
        }
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(1.5f);
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }
}
