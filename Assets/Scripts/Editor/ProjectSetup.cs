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
        GameObject canvas = CreateCanvas("MenuCanvas");

        Color bgColor = new Color(0.06f, 0.48f, 0.55f);
        Color darkBg = new Color(0.04f, 0.32f, 0.38f);
        Color accentGreen = new Color(0.2f, 0.82f, 0.48f);
        Color accentGold = new Color(1f, 0.78f, 0.2f);

        CreateBackground(canvas, bgColor);

        GameObject managerObj = new GameObject("MenuManager");
        managerObj.AddComponent<MenuManager>();
        managerObj.AddComponent<AudioManager>();

        GameObject titleShadow = CreateText(canvas.transform, "TitleShadow", "WORD", 82, new Vector2(2, 252));
        titleShadow.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.3f);
        GameObject title1 = CreateText(canvas.transform, "TitleText", "WORD", 80, new Vector2(0, 250));
        title1.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject titleShadow2 = CreateText(canvas.transform, "TitleShadow2", "JOURNEY", 82, new Vector2(2, 172));
        titleShadow2.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.3f);
        GameObject title2 = CreateText(canvas.transform, "TitleText2", "JOURNEY", 80, new Vector2(0, 170));
        title2.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        title2.GetComponent<TextMeshProUGUI>().color = accentGold;

        GameObject statsPanel = CreatePanel(canvas.transform, "StatsPanel", new Vector2(800, 60));
        RectTransform spRect = statsPanel.GetComponent<RectTransform>();
        spRect.anchorMin = new Vector2(0.5f, 0.5f);
        spRect.anchorMax = new Vector2(0.5f, 0.5f);
        spRect.anchoredPosition = new Vector2(0, 330);
        statsPanel.GetComponent<Image>().color = darkBg;

        GameObject coinsT = CreateText(statsPanel.transform, "CoinsText", "\u25C6 0", 24, new Vector2(-220, 0));
        coinsT.GetComponent<TextMeshProUGUI>().color = accentGold;
        GameObject streakT = CreateText(statsPanel.transform, "StreakText", "Streak: 0", 22, new Vector2(0, 0));
        GameObject levelT = CreateText(statsPanel.transform, "LevelText", "Level 1", 22, new Vector2(220, 0));

        GameObject playButton = CreateButton(canvas.transform, "PlayButton", "PLAY", new Vector2(0, 20), accentGreen, 320, 80, fontSize: 32);
        GameObject levelsButton = CreateButton(canvas.transform, "LevelsButton", "LEVELS", new Vector2(0, -80), new Color(0.2f, 0.5f, 0.95f), 320, 70, fontSize: 26);
        GameObject shopButton = CreateButton(canvas.transform, "ShopButton", "SHOP", new Vector2(0, -170), accentGold, 320, 70, fontSize: 26);
        GameObject settingsButton = CreateButton(canvas.transform, "SettingsButton", "SETTINGS", new Vector2(0, -255), new Color(0.45f, 0.5f, 0.6f), 320, 70, fontSize: 26);

        AddButtonShadow(playButton);
        AddButtonShadow(levelsButton);
        AddButtonShadow(shopButton);
        AddButtonShadow(settingsButton);

        MenuManager menuManager = managerObj.GetComponent<MenuManager>();
        menuManager.coinsText = coinsT.GetComponent<TextMeshProUGUI>();
        menuManager.streakText = streakT.GetComponent<TextMeshProUGUI>();
        menuManager.highLevelText = levelT.GetComponent<TextMeshProUGUI>();
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

        Color bgColor = new Color(0.06f, 0.48f, 0.55f);
        Color darkBg = new Color(0.04f, 0.32f, 0.38f);
        Color accentGreen = new Color(0.2f, 0.82f, 0.48f);
        Color accentGold = new Color(1f, 0.78f, 0.2f);
        Color accentOrange = new Color(0.95f, 0.55f, 0.15f);
        Color slotBg = new Color(0.08f, 0.15f, 0.25f, 0.95f);
        Color slotBorder = new Color(0.12f, 0.22f, 0.35f, 0.9f);

        CreateBackground(canvas, bgColor);

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

        GameObject topBar = CreatePanel(canvas.transform, "TopBar", new Vector2(1080, 130));
        RectTransform topRect = topBar.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.sizeDelta = new Vector2(0, 130);
        topRect.anchoredPosition = new Vector2(0, -65);
        topBar.GetComponent<Image>().color = darkBg;

        GameObject topShadow = CreatePanel(topBar.transform, "TopShadow", new Vector2(1080, 6));
        RectTransform tsRect = topShadow.GetComponent<RectTransform>();
        tsRect.anchorMin = new Vector2(0, 0);
        tsRect.anchorMax = new Vector2(1, 0);
        tsRect.sizeDelta = new Vector2(0, 6);
        tsRect.anchoredPosition = Vector2.zero;
        topShadow.GetComponent<Image>().color = new Color(0.15f, 0.6f, 0.65f, 0.4f);

        GameObject scoreSection = CreatePanel(topBar.transform, "ScoreSection", new Vector2(200, 80));
        RectTransform ssRect = scoreSection.GetComponent<RectTransform>();
        ssRect.anchorMin = new Vector2(0, 0.5f);
        ssRect.anchorMax = new Vector2(0, 0.5f);
        ssRect.anchoredPosition = new Vector2(130, 0);
        scoreSection.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        CreateText(scoreSection.transform, "ScoreLabel", "SCORE", 13, new Vector2(0, 16));
        GameObject scoreT = CreateText(scoreSection.transform, "ScoreText", "0", 34, new Vector2(0, -12));
        scoreT.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject levelT = CreateText(topBar.transform, "LevelText", "Level 1", 30, new Vector2(0, -5));
        levelT.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject timerT = CreateText(topBar.transform, "TimerText", "02:00", 30, new Vector2(320, -5));
        timerT.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject heartsT = CreateText(topBar.transform, "HeartsText", "\u2665\u2665\u2665\u2665\u2665", 24, new Vector2(-80, 18));
        heartsT.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.35f, 0.35f);
        GameObject coinsT = CreateText(topBar.transform, "CoinsText", "\u25C6 0", 20, new Vector2(-80, -8));
        coinsT.GetComponent<TextMeshProUGUI>().color = accentGold;

        uiManager.levelText = levelT.GetComponent<TextMeshProUGUI>();
        uiManager.scoreText = scoreT.GetComponent<TextMeshProUGUI>();
        uiManager.timerText = timerT.GetComponent<TextMeshProUGUI>();
        uiManager.coinsText = coinsT.GetComponent<TextMeshProUGUI>();
        uiManager.heartsText = heartsT.GetComponent<TextMeshProUGUI>();

        GameObject progressBg = CreatePanel(canvas.transform, "ProgressBg", new Vector2(960, 18));
        RectTransform pBgRect = progressBg.GetComponent<RectTransform>();
        pBgRect.anchorMin = new Vector2(0.5f, 1);
        pBgRect.anchorMax = new Vector2(0.5f, 1);
        pBgRect.anchoredPosition = new Vector2(0, -142);
        pBgRect.sizeDelta = new Vector2(960, 18);
        progressBg.GetComponent<Image>().color = darkBg;

        GameObject progressFill = new GameObject("ProgressFill");
        progressFill.transform.SetParent(progressBg.transform, false);
        RectTransform pFillRect = progressFill.AddComponent<RectTransform>();
        pFillRect.anchorMin = Vector2.zero;
        pFillRect.anchorMax = new Vector2(0, 1);
        pFillRect.sizeDelta = new Vector2(960, 0);
        Image fillImg = progressFill.AddComponent<Image>();
        fillImg.color = accentGreen;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0.5f;
        uiManager.progressBar = fillImg;

        GameObject progressText = CreateText(topBar.transform, "ProgressText", "0%", 13, new Vector2(400, 18));
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
        crossword.emptySlotColor = slotBg;
        crossword.foundWordColor = accentGreen;
        crossword.emptyTextColor = new Color(0.4f, 0.55f, 0.65f);
        crossword.foundTextColor = Color.white;

        GameObject wordDisplay = CreateText(canvas.transform, "CurrentWordText", "", 50, new Vector2(0, 380));
        uiManager.currentWordText = wordDisplay.GetComponent<TextMeshProUGUI>();
        wordDisplay.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        wordDisplay.GetComponent<TextMeshProUGUI>().color = Color.white;

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

        GameObject wheelShadowOuter = new GameObject("WheelShadowOuter");
        wheelShadowOuter.transform.SetParent(wheelObj.transform, false);
        RectTransform wsoRect = wheelShadowOuter.AddComponent<RectTransform>();
        wsoRect.anchorMin = new Vector2(0.5f, 0.5f);
        wsoRect.anchorMax = new Vector2(0.5f, 0.5f);
        wsoRect.sizeDelta = new Vector2(340, 340);
        Image wsoImg = wheelShadowOuter.AddComponent<Image>();
        wsoImg.color = new Color(0.02f, 0.18f, 0.22f, 0.5f);
        wheelShadowOuter.transform.SetAsFirstSibling();

        GameObject wheelBg = new GameObject("WheelBackground");
        wheelBg.transform.SetParent(wheelObj.transform, false);
        RectTransform wbRect = wheelBg.AddComponent<RectTransform>();
        wbRect.anchorMin = new Vector2(0.5f, 0.5f);
        wbRect.anchorMax = new Vector2(0.5f, 0.5f);
        wbRect.sizeDelta = new Vector2(310, 310);
        Image wbImg = wheelBg.AddComponent<Image>();
        wbImg.color = new Color(0.04f, 0.28f, 0.33f, 0.7f);
        wheelBg.transform.SetAsFirstSibling();

        GameObject wheelBgInner = new GameObject("WheelInner");
        wheelBgInner.transform.SetParent(wheelObj.transform, false);
        RectTransform wbiRect = wheelBgInner.AddComponent<RectTransform>();
        wbiRect.anchorMin = new Vector2(0.5f, 0.5f);
        wbiRect.anchorMax = new Vector2(0.5f, 0.5f);
        wbiRect.sizeDelta = new Vector2(260, 260);
        Image wbiImg = wheelBgInner.AddComponent<Image>();
        wbiImg.color = new Color(0.05f, 0.32f, 0.38f, 0.4f);
        wheelBgInner.transform.SetAsFirstSibling();

        GameObject hintBtn = CreateButton(canvas.transform, "HintButton", "HINT", new Vector2(80, 30), accentGold, 130, 50, new Vector2(0, 0), new Vector2(0, 0));
        GameObject shuffleBtn = CreateButton(canvas.transform, "ShuffleButton", "SHUFFLE", new Vector2(-80, 30), new Color(0.45f, 0.5f, 0.6f), 130, 50, new Vector2(1, 0), new Vector2(1, 0));

        AddButtonShadow(hintBtn);
        AddButtonShadow(shuffleBtn);

        uiManager.hintButton = hintBtn.GetComponent<Button>();
        uiManager.shuffleButton = shuffleBtn.GetComponent<Button>();

        GameObject messagePanel = CreatePanel(canvas.transform, "MessagePanel", new Vector2(500, 70));
        RectTransform msgRect = messagePanel.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.anchoredPosition = new Vector2(0, 100);
        messagePanel.GetComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 0.95f);
        uiManager.messagePanel = messagePanel;
        uiManager.messageText = CreateText(messagePanel.transform, "MessageText", "", 24, Vector2.zero).GetComponent<TextMeshProUGUI>();
        messagePanel.SetActive(false);

        GameObject levelCompletePanel = CreatePanel(canvas.transform, "LevelCompletePanel", new Vector2(600, 450));
        RectTransform lcRect = levelCompletePanel.GetComponent<RectTransform>();
        lcRect.anchorMin = new Vector2(0.5f, 0.5f);
        lcRect.anchorMax = new Vector2(0.5f, 0.5f);
        lcRect.anchoredPosition = Vector2.zero;
        levelCompletePanel.GetComponent<Image>().color = new Color(0.04f, 0.32f, 0.38f, 0.98f);

        GameObject lcShadow = CreatePanel(levelCompletePanel.transform, "Shadow", new Vector2(610, 460));
        RectTransform lcShadowRect = lcShadow.GetComponent<RectTransform>();
        lcShadowRect.anchorMin = new Vector2(0.5f, 0.5f);
        lcShadowRect.anchorMax = new Vector2(0.5f, 0.5f);
        lcShadowRect.sizeDelta = new Vector2(610, 460);
        lcShadowRect.anchoredPosition = new Vector2(3, -3);
        lcShadow.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
        lcShadow.transform.SetAsFirstSibling();

        CreateText(levelCompletePanel.transform, "CompleteTitle", "LEVEL COMPLETE!", 38, new Vector2(0, 130));
        CreateText(levelCompletePanel.transform, "StarsText", "\u2605 \u2605 \u2605", 48, new Vector2(0, 60));
        GameObject starsText = levelCompletePanel.transform.Find("StarsText").gameObject;
        starsText.GetComponent<TextMeshProUGUI>().color = accentGold;
        CreateText(levelCompletePanel.transform, "CompleteScore", "Score: 0", 26, new Vector2(0, 0));
        CreateText(levelCompletePanel.transform, "CompleteCoins", "+0 Coins", 26, new Vector2(0, -40));

        GameObject continueBtn = CreateButton(levelCompletePanel.transform, "ContinueButton", "CONTINUE", new Vector2(0, -120), accentGreen, 280, 60);
        AddButtonShadow(continueBtn);

        uiManager.levelCompletePanel = levelCompletePanel;
        levelCompletePanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
    }

    static void AddButtonShadow(GameObject btnObj)
    {
        GameObject shadow = new GameObject("Shadow");
        shadow.transform.SetParent(btnObj.transform, false);
        RectTransform sRect = shadow.AddComponent<RectTransform>();
        sRect.anchorMin = Vector2.zero;
        sRect.anchorMax = Vector2.one;
        sRect.sizeDelta = new Vector2(4, -4);
        sRect.anchoredPosition = new Vector2(2, -2);
        Image sImg = shadow.AddComponent<Image>();
        sImg.color = new Color(0, 0, 0, 0.25f);
        shadow.transform.SetAsFirstSibling();
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

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 position, Color bgColor, float width = 280, float height = 65, Vector2? anchorMin = null, Vector2? anchorMax = null, int fontSize = 24)
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
        tmp.fontSize = fontSize;
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
