#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ProjectSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup WordJourney Project")]
    public static void SetupProject()
    {
        CreateMenuScene();
        CreateGameScene();
        SetupBuildSettings();
        Debug.Log("WordJourney project setup complete!");
    }

    static void CreateMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject canvas = CreateCanvas("MainCanvas");

        GameObject managerObj = new GameObject("MenuManager");
        managerObj.AddComponent<MenuManager>();
        managerObj.AddComponent<AudioManager>();

        GameObject titleText = CreateText(canvas.transform, "TitleText", "WORD JOURNEY", 60, new Vector2(0, 200));
        GameObject coinsText = CreateText(canvas.transform, "CoinsText", "Coins: 0", 30, new Vector2(-350, 300));
        GameObject streakText = CreateText(canvas.transform, "StreakText", "Streak: 0", 30, new Vector2(0, 300));
        GameObject levelText = CreateText(canvas.transform, "LevelText", "Level 1", 30, new Vector2(350, 300));

        GameObject playButton = CreateButton(canvas.transform, "PlayButton", "PLAY", new Vector2(0, 50));
        GameObject levelsButton = CreateButton(canvas.transform, "LevelsButton", "LEVELS", new Vector2(0, -30));
        GameObject shopButton = CreateButton(canvas.transform, "ShopButton", "SHOP", new Vector2(0, -110));
        GameObject settingsButton = CreateButton(canvas.transform, "SettingsButton", "SETTINGS", new Vector2(0, -190));

        MenuManager menuManager = managerObj.GetComponent<MenuManager>();
        menuManager.coinsText = coinsText.GetComponent<TextMeshProUGUI>();
        menuManager.streakText = streakText.GetComponent<TextMeshProUGUI>();
        menuManager.highLevelText = levelText.GetComponent<TextMeshProUGUI>();
        menuManager.playButton = playButton.GetComponent<Button>();
        menuManager.levelsButton = levelsButton.GetComponent<Button>();
        menuManager.shopButton = shopButton.GetComponent<Button>();
        menuManager.settingsButton = settingsButton.GetComponent<Button>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MenuScene.unity");
    }

    static void CreateGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject canvas = CreateCanvas("GameCanvas");

        GameObject managerObj = new GameObject("GameManager");
        GameManager gameManager = managerObj.AddComponent<GameManager>();
        managerObj.AddComponent<LevelManager>();
        managerObj.AddComponent<WordValidator>();
        managerObj.AddComponent<WordDatabase>();
        managerObj.AddComponent<AdsManager>();
        managerObj.AddComponent<IAPManager>();
        managerObj.AddComponent<AudioManager>();

        GameObject gridObj = new GameObject("PuzzleGrid");
        gridObj.AddComponent<PuzzleGrid>();
        PuzzleGrid puzzleGrid = gridObj.GetComponent<PuzzleGrid>();

        GameObject gridParent = new GameObject("GridParent");
        gridParent.transform.SetParent(gridObj.transform);
        RectTransform gridRect = gridParent.AddComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(500, 400);
        puzzleGrid.gridParent = gridParent.transform;

        GameObject letterPrefab = CreateLetterPrefab();
        puzzleGrid.letterPrefab = letterPrefab;

        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();

        GameObject levelText = CreateText(canvas.transform, "LevelText", "Level 1", 36, new Vector2(0, 350));
        GameObject scoreText = CreateText(canvas.transform, "ScoreText", "0", 30, new Vector2(-400, 350));
        GameObject timerText = CreateText(canvas.transform, "TimerText", "02:00", 30, new Vector2(400, 350));
        GameObject wordsText = CreateText(canvas.transform, "WordsText", "0/5", 24, new Vector2(0, 300));
        GameObject currentWordText = CreateText(canvas.transform, "CurrentWordText", "", 40, new Vector2(0, -300));
        GameObject coinsText = CreateText(canvas.transform, "CoinsText", "0", 24, new Vector2(-400, -350));
        GameObject hintsText = CreateText(canvas.transform, "HintsText", "3", 24, new Vector2(400, -350));

        GameObject submitButton = CreateButton(canvas.transform, "SubmitButton", "SUBMIT", new Vector2(0, -250));
        GameObject hintButton = CreateButton(canvas.transform, "HintButton", "HINT", new Vector2(-150, -350));
        GameObject shuffleButton = CreateButton(canvas.transform, "ShuffleButton", "SHUFFLE", new Vector2(150, -350));

        uiManager.levelText = levelText.GetComponent<TextMeshProUGUI>();
        uiManager.scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        uiManager.timerText = timerText.GetComponent<TextMeshProUGUI>();
        uiManager.wordsFoundText = wordsText.GetComponent<TextMeshProUGUI>();
        uiManager.currentWordText = currentWordText.GetComponent<TextMeshProUGUI>();
        uiManager.coinsText = coinsText.GetComponent<TextMeshProUGUI>();
        uiManager.hintsText = hintsText.GetComponent<TextMeshProUGUI>();
        uiManager.submitButton = submitButton.GetComponent<Button>();
        uiManager.hintButton = hintButton.GetComponent<Button>();
        uiManager.shuffleButton = shuffleButton.GetComponent<Button>();

        GameObject levelCompletePanel = CreatePanel(canvas.transform, "LevelCompletePanel", new Vector2(600, 400));
        uiManager.levelCompletePanel = levelCompletePanel;
        levelCompletePanel.SetActive(false);

        GameObject messagePanel = CreatePanel(canvas.transform, "MessagePanel", new Vector2(400, 100));
        uiManager.messagePanel = messagePanel;
        uiManager.messageText = CreateText(messagePanel.transform, "MessageText", "", 24, Vector2.zero).GetComponent<TextMeshProUGUI>();
        messagePanel.SetActive(false);

        GameObject pausePanel = CreatePanel(canvas.transform, "PausePanel", new Vector2(400, 300));
        uiManager.pausePanel = pausePanel;
        pausePanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
    }

    static GameObject CreateCanvas(string name)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        return canvasObj;
    }

    static GameObject CreateText(Transform parent, string name, string content, int fontSize, Vector2 position)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 60);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return textObj;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 position)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 60);
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f);
        btnObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return btnObj;
    }

    static GameObject CreatePanel(Transform parent, string name, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);
        return panel;
    }

    static GameObject CreateLetterPrefab()
    {
        GameObject tileObj = new GameObject("LetterTilePrefab");
        RectTransform rect = tileObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80, 80);
        Image img = tileObj.AddComponent<Image>();
        img.color = Color.white;
        tileObj.AddComponent<Button>();

        GameObject textObj = new GameObject("LetterText");
        textObj.transform.SetParent(tileObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "A";
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;

        LetterTile tile = tileObj.AddComponent<LetterTile>();
        tile.letterText = tmp;
        tile.backgroundImage = img;

        return tileObj;
    }

    static void SetupBuildSettings()
    {
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MenuScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true)
        };
    }
}
#endif
