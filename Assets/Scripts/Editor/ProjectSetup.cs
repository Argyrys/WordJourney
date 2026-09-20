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

        CreateBackground(canvas, new Color(0.08f, 0.55f, 0.6f));

        GameObject managerObj = new GameObject("MenuManager");
        managerObj.AddComponent<MenuManager>();
        managerObj.AddComponent<AudioManager>();

        CreateText(canvas.transform, "TitleText", "WORD", 80, new Vector2(0, 250));
        CreateText(canvas.transform, "TitleText2", "JOURNEY", 80, new Vector2(0, 170));
        CreateText(canvas.transform, "CoinsText", "Coins: 0", 28, new Vector2(-350, 330));
        CreateText(canvas.transform, "StreakText", "Streak: 0", 28, new Vector2(0, 330));
        CreateText(canvas.transform, "LevelText", "Level 1", 28, new Vector2(350, 330));

        GameObject playButton = CreateButton(canvas.transform, "PlayButton", "PLAY", new Vector2(0, 20), new Color(0.2f, 0.8f, 0.5f));
        GameObject levelsButton = CreateButton(canvas.transform, "LevelsButton", "LEVELS", new Vector2(0, -65), new Color(0.3f, 0.65f, 1f));
        GameObject shopButton = CreateButton(canvas.transform, "ShopButton", "SHOP", new Vector2(0, -150), new Color(1f, 0.75f, 0.2f));
        GameObject settingsButton = CreateButton(canvas.transform, "SettingsButton", "SETTINGS", new Vector2(0, -235), new Color(0.55f, 0.55f, 0.6f));

        MenuManager menuManager = managerObj.GetComponent<MenuManager>();
        menuManager.coinsText = canvas.transform.Find("CoinsText").GetComponent<TextMeshProUGUI>();
        menuManager.streakText = canvas.transform.Find("StreakText").GetComponent<TextMeshProUGUI>();
        menuManager.highLevelText = canvas.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
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

        CreateBackground(canvas, new Color(0.08f, 0.55f, 0.6f));

        GameObject managerObj = new GameObject("GameManager");
        managerObj.AddComponent<GameManager>();
        managerObj.AddComponent<LevelManager>();
        managerObj.AddComponent<WordValidator>();
        managerObj.AddComponent<WordDatabase>();
        managerObj.AddComponent<AdsManager>();
        managerObj.AddComponent<IAPManager>();
        managerObj.AddComponent<AudioManager>();

        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();

        GameObject topBar = CreatePanel(canvas.transform, "TopBar", new Vector2(1080, 120));
        RectTransform topRect = topBar.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.sizeDelta = new Vector2(0, 120);
        topRect.anchoredPosition = new Vector2(0, -60);
        topBar.GetComponent<Image>().color = new Color(0.06f, 0.4f, 0.45f, 0.9f);

        CreateText(topBar.transform, "ScoreLabel", "SCORE", 14, new Vector2(-320, 15));
        GameObject scoreT = CreateText(topBar.transform, "ScoreText", "0", 30, new Vector2(-320, -12));
        GameObject levelT = CreateText(topBar.transform, "LevelText", "Level 1", 28, new Vector2(0, -5));
        GameObject timerT = CreateText(topBar.transform, "TimerText", "02:00", 28, new Vector2(320, -5));
        GameObject heartsT = CreateText(topBar.transform, "HeartsText", "♥♥♥♥♥", 22, new Vector2(-100, 15));
        GameObject coinsT = CreateText(topBar.transform, "CoinsText", "0", 22, new Vector2(-60, -5));

        uiManager.levelText = levelT.GetComponent<TextMeshProUGUI>();
        uiManager.scoreText = scoreT.GetComponent<TextMeshProUGUI>();
        uiManager.timerText = timerT.GetComponent<TextMeshProUGUI>();
        uiManager.coinsText = coinsT.GetComponent<TextMeshProUGUI>();
        uiManager.heartsText = heartsT.GetComponent<TextMeshProUGUI>();

        GameObject progressBg = CreatePanel(canvas.transform, "ProgressBg", new Vector2(1000, 20));
        RectTransform pBgRect = progressBg.GetComponent<RectTransform>();
        pBgRect.anchorMin = new Vector2(0.5f, 1);
        pBgRect.anchorMax = new Vector2(0.5f, 1);
        pBgRect.anchoredPosition = new Vector2(0, -135);
        pBgRect.sizeDelta = new Vector2(1000, 20);
        progressBg.GetComponent<Image>().color = new Color(0.05f, 0.3f, 0.35f);

        GameObject progressFill = new GameObject("ProgressFill");
        progressFill.transform.SetParent(progressBg.transform, false);
        RectTransform pFillRect = progressFill.AddComponent<RectTransform>();
        pFillRect.anchorMin = Vector2.zero;
        pFillRect.anchorMax = new Vector2(0, 1);
        pFillRect.sizeDelta = new Vector2(1000, 0);
        Image fillImg = progressFill.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.85f, 0.5f);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        uiManager.progressBar = fillImg;

        GameObject progressText = CreateText(topBar.transform, "ProgressText", "0%", 14, new Vector2(400, 15));
        uiManager.progressText = progressText.GetComponent<TextMeshProUGUI>();

        GameObject crosswordObj = new GameObject("CrosswordDisplay");
        crosswordObj.transform.SetParent(canvas.transform, false);
        RectTransform cwRect = crosswordObj.AddComponent<RectTransform>();
        cwRect.anchorMin = new Vector2(0.05f, 0.58f);
        cwRect.anchorMax = new Vector2(0.95f, 0.82f);
        cwRect.sizeDelta = Vector2.zero;
        cwRect.anchoredPosition = Vector2.zero;
        crosswordObj.AddComponent<CrosswordDisplay>();
        CrosswordDisplay crossword = crosswordObj.GetComponent<CrosswordDisplay>();
        crossword.wordContainer = cwRect;

        GameObject wordDisplay = CreateText(canvas.transform, "CurrentWordText", "", 48, new Vector2(0, 380));
        uiManager.currentWordText = wordDisplay.GetComponent<TextMeshProUGUI>();
        wordDisplay.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject wheelObj = new GameObject("LetterWheel");
        wheelObj.transform.SetParent(canvas.transform, false);
        RectTransform wheelRect = wheelObj.AddComponent<RectTransform>();
        wheelRect.anchorMin = new Vector2(0.5f, 0.02f);
        wheelRect.anchorMax = new Vector2(0.5f, 0.30f);
        wheelRect.sizeDelta = Vector2.zero;
        wheelRect.anchoredPosition = Vector2.zero;
        wheelObj.AddComponent<CircularLetterWheel>();
        CircularLetterWheel wheel = wheelObj.GetComponent<CircularLetterWheel>();
        wheel.wheelCenter = wheelRect;
        wheel.wheelRadius = 130f;
        wheel.letterSize = 60f;

        GameObject wheelBg = new GameObject("WheelBackground");
        wheelBg.transform.SetParent(wheelObj.transform, false);
        RectTransform wbRect = wheelBg.AddComponent<RectTransform>();
        wbRect.anchorMin = new Vector2(0.5f, 0.5f);
        wbRect.anchorMax = new Vector2(0.5f, 0.5f);
        wbRect.sizeDelta = new Vector2(320, 320);
        Image wbImg = wheelBg.AddComponent<Image>();
        wbImg.color = new Color(0.05f, 0.35f, 0.4f, 0.6f);
        wheelBg.transform.SetAsFirstSibling();

        GameObject hintBtn = CreateButton(canvas.transform, "HintButton", "HINT", new Vector2(-100, 20), new Color(1f, 0.75f, 0.2f), 140, 55, new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        GameObject shuffleBtn = CreateButton(canvas.transform, "ShuffleButton", "SHUFFLE", new Vector2(100, 20), new Color(0.55f, 0.55f, 0.65f), 140, 55, new Vector2(0.5f, 0), new Vector2(0.5f, 0));

        uiManager.hintButton = hintBtn.GetComponent<Button>();
        uiManager.shuffleButton = shuffleBtn.GetComponent<Button>();

        GameObject messagePanel = CreatePanel(canvas.transform, "MessagePanel", new Vector2(500, 80));
        RectTransform msgRect = messagePanel.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.anchoredPosition = new Vector2(0, 100);
        messagePanel.GetComponent<Image>().color = new Color(0.1f, 0.15f, 0.2f, 0.95f);
        uiManager.messagePanel = messagePanel;
        uiManager.messageText = CreateText(messagePanel.transform, "MessageText", "", 24, Vector2.zero).GetComponent<TextMeshProUGUI>();
        messagePanel.SetActive(false);

        GameObject levelCompletePanel = CreatePanel(canvas.transform, "LevelCompletePanel", new Vector2(600, 400));
        levelCompletePanel.GetComponent<Image>().color = new Color(0.06f, 0.4f, 0.45f, 0.98f);
        CreateText(levelCompletePanel.transform, "CompleteTitle", "LEVEL COMPLETE!", 36, new Vector2(0, 120));
        CreateText(levelCompletePanel.transform, "CompleteScore", "Score: 0", 24, new Vector2(0, 50));
        CreateText(levelCompletePanel.transform, "CompleteCoins", "+0 Coins", 24, new Vector2(0, 0));
        uiManager.levelCompletePanel = levelCompletePanel;
        levelCompletePanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
    }

    static void CreateBackground(GameObject canvas, Color color)
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = color;
        }
        RenderSettings.skybox = null;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvas.transform, false);
        RectTransform rect = bg.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        bg.transform.SetAsFirstSibling();
        Image img = bg.AddComponent<Image>();
        img.color = color;
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

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 position, Color bgColor, float width = 280, float height = 65, Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        if (anchorMin.HasValue) rect.anchorMin = anchorMin.Value;
        if (anchorMax.HasValue) rect.anchorMax = anchorMax.Value;
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
        tmp.fontSize = 24;
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
        img.color = new Color(0.1f, 0.12f, 0.2f, 0.9f);
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
