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
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

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
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
        PopulateLevelSelect();
    }

    private void OnShopClicked()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
    }

    private void OnSettingsClicked()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        UpdateMainMenuUI();
    }

    private void PopulateLevelSelect()
    {
        if (levelButtonParent == null) return;

        foreach (Transform child in levelButtonParent)
        {
            Destroy(child.gameObject);
        }

        int maxLevel = GameManager.Instance.GetMaxLevel();
        int startLevel = currentPage * levelsPerPage + 1;
        int endLevel = Mathf.Min(startLevel + levelsPerPage, 100);

        if (levelButtonPrefab == null) return;

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
