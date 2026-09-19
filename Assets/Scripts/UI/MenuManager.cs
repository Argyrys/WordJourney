using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;

    [Header("Main Menu")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI highLevelText;

    [Header("Buttons")]
    public Button playButton;
    public Button levelsButton;
    public Button shopButton;
    public Button settingsButton;

    [Header("Level Select")]
    public Transform levelButtonParent;
    public GameObject levelButtonPrefab;
    public int levelsPerPage = 20;
    private int currentPage = 0;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);

        SetupButtons();
        UpdateMainMenuUI();
    }

    private void SetupButtons()
    {
        playButton?.onClick.AddListener(OnPlayClicked);
        levelsButton?.onClick.AddListener(OnLevelsClicked);
        shopButton?.onClick.AddListener(OnShopClicked);
        settingsButton?.onClick.AddListener(OnSettingsClicked);
    }

    private void UpdateMainMenuUI()
    {
        if (GameManager.Instance != null)
        {
            coinsText.text = GameManager.Instance.coins.ToString();
            streakText.text = $"Streak: {GameManager.Instance.streak}";
            highLevelText.text = $"Level {GameManager.Instance.GetMaxLevel() - 1}";
        }
    }

    private void OnPlayClicked()
    {
        int nextLevel = GameManager.Instance.GetMaxLevel();
        GameManager.Instance.StartLevel(nextLevel);
    }

    private void OnLevelsClicked()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        PopulateLevelSelect();
    }

    private void OnShopClicked()
    {
        mainMenuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    private void OnSettingsClicked()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        UpdateMainMenuUI();
    }

    private void PopulateLevelSelect()
    {
        foreach (Transform child in levelButtonParent)
        {
            Destroy(child.gameObject);
        }

        int maxLevel = GameManager.Instance.GetMaxLevel();
        int startLevel = currentPage * levelsPerPage + 1;
        int endLevel = Mathf.Min(startLevel + levelsPerPage, 100);

        for (int i = startLevel; i <= endLevel; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonParent);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();

            if (levelButton != null)
            {
                LevelProgress progress = GameManager.Instance.GetLevelProgress(i);
                levelButton.Setup(i, progress.stars, i < maxLevel);
            }
        }
    }

    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt(100f / levelsPerPage) - 1;
        if (currentPage < maxPage)
        {
            currentPage++;
            PopulateLevelSelect();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            PopulateLevelSelect();
        }
    }
}
