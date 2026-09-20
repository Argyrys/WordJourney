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

        GameObject bg = CreateBackground(scene, new Color(0.12f, 0.14f, 0.22f));
        GameObject canvas = CreateCanvas("MainCanvas");

        GameObject managerObj = new GameObject("MenuManager");
        managerObj.AddComponent<MenuManager>();
        managerObj.AddComponent<AudioManager>();

        CreateText(canvas.transform, "TitleText", "WORD", 72, new Vector2(0, 230));
        CreateText(canvas.transform, "TitleText2", "JOURNEY", 72, new Vector2(0, 160));
        coinsText = CreateText(canvas.transform, "CoinsText", "Coins: 0", 28, new Vector2(-350, 310));
        streakText = CreateText(canvas.transform, "StreakText", "Streak: 0", 28, new Vector2(0, 310));
        levelText = CreateText(canvas.transform, "LevelText", "Level 1", 28, new Vector2(350, 310));

        GameObject playButton = CreateButton(canvas.transform, "PlayButton", "PLAY", new Vector2(0, 30), new Color(0.2f, 0.75f, 0.35f));
        GameObject levelsButton = CreateButton(canvas.transform, "LevelsButton", "LEVELS", new Vector2(0, -50), new Color(0.2f, 0.6f, 0.9f));
        GameObject shopButton = CreateButton(canvas.transform, "ShopButton", "SHOP", new Vector2(0, -130), new Color(0.9f, 0.65f, 0.15f));
        GameObject settingsButton = CreateButton(canvas.transform, "SettingsButton", "SETTINGS", new Vector2(0, -210), new Color(0.55f, 0.55f, 0.6f));

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

    static TextMeshProUGUI coinsText;
    static TextMeshProUGUI streakText;
    static TextMeshProUGUI levelText;

    static void CreateGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject bg = CreateBackground(scene, new Color(0.15f, 0.18f, 0.28f));
        GameObject canvas = CreateCanvas("GameCanvas");

        GameObject managerObj = new GameObject("GameManager");
        managerObj.AddComponent<GameManager>();
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
        gridParent.transform.SetParent(canvas.transform, false);
        RectTransform gridRect = gridParent.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.1f, 0.25f);
        gridRect.anchorMax = new Vector2(0.9f, 0.72f);
        gridRect.sizeDelta = Vector2.zero;
        gridRect.anchoredPosition = Vector2.zero;
        puzzleGrid.gridParent = gridParent.transform;

        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();

        CreateText(canvas.transform, "ScoreLabel", "SCORE", 16, new Vector2(-350, 370));
        GameObject scoreText = CreateText(canvas.transform, "ScoreText", "0", 32, new Vector2(-350, 340));
        GameObject levelTextUI = CreateText(canvas.transform, "LevelText", "Level 1", 32, new Vector2(0, 355));
        CreateText(canvas.transform, "WordsLabel", "0/3", 18, new Vector2(0, 315));
        CreateText(canvas.transform, "TimerLabel", "TIME", 16, new Vector2(350, 370));
        GameObject timerText = CreateText(canvas.transform, "TimerText", "02:00", 32, new Vector2(350, 340));
        GameObject currentWordText = CreateText(canvas.transform, "CurrentWordText", "", 36, new Vector2(0, -290));
        CreateText(canvas.transform, "CoinsLabel", "", 22, new Vector2(-350, -340));
        GameObject coinsTextUI = CreateText(canvas.transform, "CoinsText", "0", 22, new Vector2(-300, -340));
        CreateText(canvas.transform, "HintLabel", "", 22, new Vector2(350, -340));
        GameObject hintsTextUI = CreateText(canvas.transform, "HintsText", "3", 22, new Vector2(400, -340));

        GameObject submitButton = CreateButton(canvas.transform, "SubmitButton", "SUBMIT", new Vector2(0, -340), new Color(0.2f, 0.75f, 0.35f));
        GameObject hintButton = CreateButton(canvas.transform, "HintButton", "HINT", new Vector2(-160, -410), new Color(0.9f, 0.65f, 0.15f), 160, 50);
        GameObject shuffleButton = CreateButton(canvas.transform, "ShuffleButton", "SHUFFLE", new Vector2(160, -410), new Color(0.55f, 0.55f, 0.6f), 160, 50);

        uiManager.levelText = levelTextUI.GetComponent<TextMeshProUGUI>();
        uiManager.scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        uiManager.timerText = timerText.GetComponent<TextMeshProUGUI>();
        uiManager.wordsFoundText = canvas.transform.Find("WordsLabel").GetComponent<TextMeshProUGUI>();
        uiManager.currentWordText = currentWordText.GetComponent<TextMeshProUGUI>();
        uiManager.coinsText = coinsTextUI.GetComponent<TextMeshProUGUI>();
        uiManager.hintsText = hintsTextUI.GetComponent<TextMeshProUGUI>();
        uiManager.submitButton = submitButton.GetComponent<Button>();
        uiManager.hintButton = hintButton.GetComponent<Button>();
        uiManager.shuffleButton = shuffleButton.GetComponent<Button>();

        GameObject levelCompletePanel = CreatePanel(canvas.transform, "LevelCompletePanel", new Vector2(600, 400));
        uiManager.levelCompletePanel = levelCompletePanel;
        CreateText(levelCompletePanel.transform, "CompleteTitle", "LEVEL COMPLETE!", 36, new Vector2(0, 120));
        CreateText(levelCompletePanel.transform, "CompleteScore", "Score: 0", 24, new Vector2(0, 50));
        CreateText(levelCompletePanel.transform, "CompleteCoins", "+0 Coins", 24, new Vector2(0, 0));
        levelCompletePanel.SetActive(false);

        GameObject messagePanel = CreatePanel(canvas.transform, "MessagePanel", new Vector2(500, 80));
        uiManager.messagePanel = messagePanel;
        uiManager.messageText = CreateText(messagePanel.transform, "MessageText", "", 24, Vector2.zero).GetComponent<TextMeshProUGUI>();
        messagePanel.SetActive(false);

        GameObject pausePanel = CreatePanel(canvas.transform, "PausePanel", new Vector2(400, 300));
        uiManager.pausePanel = pausePanel;
        CreateText(pausePanel.transform, "PauseTitle", "PAUSED", 36, new Vector2(0, 80));
        pausePanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
    }

    static GameObject CreateBackground(UnityEngine.SceneManagement.Scene scene, Color color)
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = color;
        }

        GameObject bg = new GameObject("Background");
        RectTransform rect = bg.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            bg.transform.SetParent(canvas.transform, false);
            bg.transform.SetAsFirstSibling();
        }
        Image img = bg.AddComponent<Image>();
        img.color = color;
        return bg;
    }

    static GameObject CreateCanvas(string name)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
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

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 position, Color bgColor, float width = 280, float height = 65)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, height);
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        btnObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 26;
        tmp.fontStyle = FontStyles.Bold;
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
        img.color = new Color(0.1f, 0.12f, 0.2f, 0.95f);
        return panel;
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
