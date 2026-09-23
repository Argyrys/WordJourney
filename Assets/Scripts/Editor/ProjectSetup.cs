#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ProjectSetup : MonoBehaviour
{
    private static Sprite roundedSmall;
    private static Sprite roundedMed;
    private static Sprite roundedLarge;
    private static Sprite circle;
    private static Sprite pill;
    private static Sprite badge;
    private static TMP_FontAsset gameFontAsset;

    [MenuItem("Tools/Setup WordJourney Project")]
    public static void SetupProject()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Setup Failed",
                "Please exit Play Mode first, then run Tools > Setup WordJourney Project.",
                "OK");
            Debug.LogError("Cannot run setup during Play Mode. Exit Play Mode and try again.");
            return;
        }

        GenerateSprites();
        LoadFont();
        CreateMenuScene();
        CreateGameScene();
        SetupBuildSettings();
        Debug.Log("WordJourney project setup complete!");
    }

    static void GenerateSprites()
    {
        roundedSmall = UISpriteGenerator.CreateRoundedRect(128, 128, 12, Color.white);
        roundedMed = UISpriteGenerator.CreateRoundedRect(128, 128, 20, Color.white);
        roundedLarge = UISpriteGenerator.CreateRoundedRect(128, 128, 32, Color.white);
        circle = UISpriteGenerator.CreateCircle(128, Color.white);
        pill = UISpriteGenerator.CreateRoundedRect(256, 64, 32, Color.white);
        badge = UISpriteGenerator.CreateCircle(64, Color.white);
    }

    static void LoadFont()
    {
        gameFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Nunito SDF.asset");
        if (gameFontAsset == null)
        {
            Debug.LogWarning("Nunito SDF font asset not found. Using default TMP font. Run Tools > Create TMP Font Asset first.");
        }
    }

    static void CreateMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject canvas = CreateCanvas("MenuCanvas");

        Color bgColor = new Color(0.04f, 0.40f, 0.50f);
        Color darkBg = new Color(0.03f, 0.25f, 0.32f);
        Color accentGreen = new Color(0.30f, 0.78f, 0.40f);
        Color accentGold = new Color(1f, 0.78f, 0.15f);

        CreateBackground(canvas, bgColor);

        GameObject managerObj = new GameObject("MenuManager");
        managerObj.AddComponent<MenuManager>();
        managerObj.AddComponent<AudioManager>();

        GameObject fontManagerObj = new GameObject("FontManager");
        FontManager fontManager = fontManagerObj.AddComponent<FontManager>();
        fontManager.gameFont = gameFontAsset;

        GameObject titleShadow1 = CreateText(canvas.transform, "TitleShadow1", "WORD", 84, new Vector2(3, 253));
        titleShadow1.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.35f);
        GameObject title1 = CreateText(canvas.transform, "TitleText", "WORD", 82, new Vector2(0, 250));
        title1.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject titleShadow2 = CreateText(canvas.transform, "TitleShadow2", "JOURNEY", 84, new Vector2(3, 173));
        titleShadow2.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.35f);
        GameObject title2 = CreateText(canvas.transform, "TitleText2", "JOURNEY", 82, new Vector2(0, 170));
        title2.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        title2.GetComponent<TextMeshProUGUI>().color = accentGold;

        GameObject statsPanel = CreateRoundedPanel(canvas.transform, "StatsPanel", new Vector2(800, 55), roundedMed, darkBg);
        RectTransform spRect = statsPanel.GetComponent<RectTransform>();
        spRect.anchorMin = new Vector2(0.5f, 0.5f);
        spRect.anchorMax = new Vector2(0.5f, 0.5f);
        spRect.anchoredPosition = new Vector2(0, 330);

        GameObject coinsT = CreateText(statsPanel.transform, "CoinsText", "\u25CF 0", 22, new Vector2(-220, 0));
        coinsT.GetComponent<TextMeshProUGUI>().color = accentGold;
        GameObject streakT = CreateText(statsPanel.transform, "StreakText", "Streak: 0", 20, new Vector2(0, 0));
        GameObject levelT = CreateText(statsPanel.transform, "LevelText", "Level 1", 20, new Vector2(220, 0));

        GameObject playButton = CreateGradientButton(canvas.transform, "PlayButton", "PLAY", new Vector2(0, 30), accentGreen, new Color(0.18f, 0.60f, 0.28f), 340, 90, roundedLarge, fontSize: 36);
        GameObject levelsButton = CreateGradientButton(canvas.transform, "LevelsButton", "LEVELS", new Vector2(0, -80), new Color(0.18f, 0.48f, 0.90f), new Color(0.12f, 0.35f, 0.75f), 340, 75, roundedMed, fontSize: 28);
        GameObject shopButton = CreateGradientButton(canvas.transform, "ShopButton", "SHOP", new Vector2(0, -175), accentGold, new Color(0.80f, 0.60f, 0.10f), 340, 75, roundedMed, fontSize: 28);
        GameObject settingsButton = CreateGradientButton(canvas.transform, "SettingsButton", "SETTINGS", new Vector2(0, -265), new Color(0.40f, 0.45f, 0.55f), new Color(0.30f, 0.35f, 0.45f), 340, 75, roundedMed, fontSize: 28);

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

        Color bgColor = new Color(0.04f, 0.40f, 0.50f);
        Color darkBg = new Color(0.03f, 0.25f, 0.32f);
        Color topBarBg = new Color(0.03f, 0.22f, 0.28f, 0.95f);
        Color accentGreen = new Color(0.30f, 0.78f, 0.40f);
        Color accentGold = new Color(1f, 0.78f, 0.15f);
        Color foundGreen = new Color(0.30f, 0.75f, 0.35f);
        Color unfoundDark = new Color(0.06f, 0.12f, 0.22f, 0.92f);
        Color unfoundText = new Color(0.55f, 0.68f, 0.78f);
        Color wheelBgDark = new Color(0.03f, 0.18f, 0.25f, 0.85f);
        Color pillBg = new Color(0.15f, 0.55f, 0.60f, 0.9f);

        CreateBackground(canvas, bgColor);

        GameObject managerObj = new GameObject("GameManager");
        managerObj.AddComponent<GameManager>();
        managerObj.AddComponent<LevelManager>();
        managerObj.AddComponent<WordValidator>();
        managerObj.AddComponent<WordDatabase>();
        managerObj.AddComponent<AdsManager>();
        managerObj.AddComponent<IAPManager>();
        managerObj.AddComponent<AudioManager>();

        GameObject fontManagerObj = new GameObject("FontManager");
        FontManager fontManager = fontManagerObj.AddComponent<FontManager>();
        fontManager.gameFont = gameFontAsset;

        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();

        // Top bar with rounded corners
        GameObject topBar = CreateRoundedPanel(canvas.transform, "TopBar", new Vector2(1080, 120), roundedMed, topBarBg);
        RectTransform topRect = topBar.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.sizeDelta = new Vector2(0, 120);
        topRect.anchoredPosition = new Vector2(0, -60);

        GameObject topBottomLine = CreatePanel(topBar.transform, "TopLine", new Vector2(1080, 3));
        RectTransform tblRect = topBottomLine.GetComponent<RectTransform>();
        tblRect.anchorMin = new Vector2(0, 0);
        tblRect.anchorMax = new Vector2(1, 0);
        tblRect.sizeDelta = new Vector2(0, 3);
        tblRect.anchoredPosition = Vector2.zero;
        topBottomLine.GetComponent<Image>().color = new Color(0.15f, 0.55f, 0.60f, 0.5f);

        GameObject gameTitle = CreateText(topBar.transform, "GameTitle", "WORD JOURNEY", 28, new Vector2(0, 20));
        gameTitle.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject coinsArea = CreateRoundedPanel(topBar.transform, "CoinsArea", new Vector2(180, 40), roundedSmall, new Color(0.08f, 0.18f, 0.25f, 0.8f));
        RectTransform caRect = coinsArea.GetComponent<RectTransform>();
        caRect.anchorMin = new Vector2(1, 0.5f);
        caRect.anchorMax = new Vector2(1, 0.5f);
        caRect.anchoredPosition = new Vector2(-100, 0);

        GameObject coinIcon = CreateText(coinsArea.transform, "CoinIcon", "\u25CF", 20, new Vector2(-50, 0));
        coinIcon.GetComponent<TextMeshProUGUI>().color = accentGold;
        GameObject coinsT = CreateText(coinsArea.transform, "CoinsText", "0", 20, new Vector2(10, 0));
        coinsT.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        GameObject plusBtn = CreateText(coinsArea.transform, "PlusBtn", "+", 18, new Vector2(60, 0));
        plusBtn.GetComponent<TextMeshProUGUI>().color = accentGreen;
        plusBtn.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject levelT = CreateText(topBar.transform, "LevelText", "LEVEL 1", 16, new Vector2(0, -10));
        levelT.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject heartsT = CreateText(topBar.transform, "HeartsText", "\u2665\u2665\u2665\u2665\u2665", 18, new Vector2(-260, -12));
        heartsT.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.3f, 0.3f);
        GameObject timerT = CreateText(topBar.transform, "TimerText", "02:00", 18, new Vector2(260, -12));
        timerT.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        uiManager.levelText = levelT.GetComponent<TextMeshProUGUI>();
        uiManager.scoreText = coinsT.GetComponent<TextMeshProUGUI>();
        uiManager.timerText = timerT.GetComponent<TextMeshProUGUI>();
        uiManager.coinsText = coinsT.GetComponent<TextMeshProUGUI>();
        uiManager.heartsText = heartsT.GetComponent<TextMeshProUGUI>();

        // Progress bar with rounded corners
        GameObject progressBg = CreateRoundedPanel(canvas.transform, "ProgressBg", new Vector2(1000, 14), roundedSmall, darkBg);
        RectTransform pBgRect = progressBg.GetComponent<RectTransform>();
        pBgRect.anchorMin = new Vector2(0.5f, 1);
        pBgRect.anchorMax = new Vector2(0.5f, 1);
        pBgRect.anchoredPosition = new Vector2(0, -128);
        pBgRect.sizeDelta = new Vector2(1000, 14);

        GameObject progressFill = new GameObject("ProgressFill");
        progressFill.transform.SetParent(progressBg.transform, false);
        RectTransform pFillRect = progressFill.AddComponent<RectTransform>();
        pFillRect.anchorMin = Vector2.zero;
        pFillRect.anchorMax = new Vector2(0, 1);
        pFillRect.sizeDelta = new Vector2(1000, 0);
        Image fillImg = progressFill.AddComponent<Image>();
        fillImg.sprite = roundedSmall;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0.5f;
        fillImg.color = accentGreen;
        uiManager.progressBar = fillImg;

        // Crossword area
        GameObject crosswordObj = new GameObject("CrosswordDisplay");
        crosswordObj.transform.SetParent(canvas.transform, false);
        RectTransform cwRect = crosswordObj.AddComponent<RectTransform>();
        cwRect.anchorMin = new Vector2(0.03f, 0.46f);
        cwRect.anchorMax = new Vector2(0.97f, 0.87f);
        cwRect.sizeDelta = Vector2.zero;
        cwRect.anchoredPosition = Vector2.zero;
        crosswordObj.AddComponent<CrosswordDisplay>();
        CrosswordDisplay crossword = crosswordObj.GetComponent<CrosswordDisplay>();
        crossword.wordContainer = cwRect;
        crossword.emptySlotColor = unfoundDark;
        crossword.foundWordColor = foundGreen;
        crossword.emptyTextColor = unfoundText;
        crossword.foundTextColor = Color.white;

        // Word pill with rounded corners
        GameObject wordPillBg = CreateRoundedPanel(canvas.transform, "WordPillBg", new Vector2(340, 50), pill, pillBg);
        RectTransform wpRect = wordPillBg.GetComponent<RectTransform>();
        wpRect.anchorMin = new Vector2(0.5f, 0.43f);
        wpRect.anchorMax = new Vector2(0.5f, 0.43f);
        wpRect.anchoredPosition = Vector2.zero;
        wpRect.sizeDelta = new Vector2(340, 50);

        GameObject wordDisplay = CreateText(wordPillBg.transform, "CurrentWordText", "", 32, Vector2.zero);
        uiManager.currentWordText = wordDisplay.GetComponent<TextMeshProUGUI>();
        wordDisplay.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        wordDisplay.GetComponent<TextMeshProUGUI>().color = Color.white;

        // Letter wheel
        GameObject wheelObj = new GameObject("LetterWheel");
        wheelObj.transform.SetParent(canvas.transform, false);
        RectTransform wheelRect = wheelObj.AddComponent<RectTransform>();
        wheelRect.anchorMin = new Vector2(0.5f, 0.0f);
        wheelRect.anchorMax = new Vector2(0.5f, 0.38f);
        wheelRect.sizeDelta = Vector2.zero;
        wheelRect.anchoredPosition = Vector2.zero;
        wheelObj.AddComponent<CircularLetterWheel>();
        CircularLetterWheel wheel = wheelObj.GetComponent<CircularLetterWheel>();
        wheel.wheelCenter = wheelRect;
        wheel.wheelRadius = 150f;
        wheel.letterSize = 68f;

        // Wheel background with gradient
        GameObject wheelShadowOuter = new GameObject("WheelShadowOuter");
        wheelShadowOuter.transform.SetParent(wheelObj.transform, false);
        RectTransform wsoRect = wheelShadowOuter.AddComponent<RectTransform>();
        wsoRect.anchorMin = new Vector2(0.5f, 0.5f);
        wsoRect.anchorMax = new Vector2(0.5f, 0.5f);
        wsoRect.sizeDelta = new Vector2(390, 390);
        Image wsoImg = wheelShadowOuter.AddComponent<Image>();
        wsoImg.sprite = circle;
        wsoImg.color = new Color(0.01f, 0.12f, 0.18f, 0.6f);
        wheelShadowOuter.transform.SetAsFirstSibling();

        GameObject wheelBg = new GameObject("WheelBackground");
        wheelBg.transform.SetParent(wheelObj.transform, false);
        RectTransform wbRect = wheelBg.AddComponent<RectTransform>();
        wbRect.anchorMin = new Vector2(0.5f, 0.5f);
        wbRect.anchorMax = new Vector2(0.5f, 0.5f);
        wbRect.sizeDelta = new Vector2(360, 360);
        Image wbImg = wheelBg.AddComponent<Image>();
        wbImg.sprite = circle;
        wbImg.color = wheelBgDark;
        AddGradientOverlay(wheelBg, new Color(0.06f, 0.30f, 0.40f, 0.3f), new Color(0.01f, 0.10f, 0.15f, 0.3f));
        wheelBg.transform.SetAsFirstSibling();

        GameObject wheelBgInner = new GameObject("WheelInner");
        wheelBgInner.transform.SetParent(wheelObj.transform, false);
        RectTransform wbiRect = wheelBgInner.AddComponent<RectTransform>();
        wbiRect.anchorMin = new Vector2(0.5f, 0.5f);
        wbiRect.anchorMax = new Vector2(0.5f, 0.5f);
        wbiRect.sizeDelta = new Vector2(300, 300);
        Image wbiImg = wheelBgInner.AddComponent<Image>();
        wbiImg.sprite = circle;
        wbiImg.color = new Color(0.04f, 0.22f, 0.30f, 0.5f);
        wheelBgInner.transform.SetAsFirstSibling();

        // Buttons with gradient
        GameObject shuffleBtn = CreateGradientButton(canvas.transform, "ShuffleButton", "SHUFFLE", new Vector2(-280, 60), new Color(0.08f, 0.18f, 0.26f, 0.95f), new Color(0.04f, 0.10f, 0.16f, 0.95f), 150, 55, roundedMed, new Vector2(0.5f, 0.19f), new Vector2(0.5f, 0.19f), 16);
        GameObject hintBtn = CreateGradientButton(canvas.transform, "HintButton", "HINTS", new Vector2(280, 60), new Color(0.08f, 0.18f, 0.26f, 0.95f), new Color(0.04f, 0.10f, 0.16f, 0.95f), 150, 55, roundedMed, new Vector2(0.5f, 0.19f), new Vector2(0.5f, 0.19f), 16);

        CreateButtonBadge(hintBtn, "1");

        GameObject bonusBtn = CreateGradientButton(canvas.transform, "BonusButton", "BONUS WORD", new Vector2(280, -10), new Color(0.08f, 0.18f, 0.26f, 0.85f), new Color(0.04f, 0.10f, 0.16f, 0.85f), 170, 50, roundedMed, new Vector2(0.5f, 0.14f), new Vector2(0.5f, 0.14f), 14);

        uiManager.hintButton = hintBtn.GetComponent<Button>();
        uiManager.shuffleButton = shuffleBtn.GetComponent<Button>();

        // Coin display with rounded corners
        GameObject coinDisplay = CreateRoundedPanel(canvas.transform, "CoinDisplay", new Vector2(140, 45), roundedSmall, new Color(0.08f, 0.18f, 0.25f, 0.85f));
        RectTransform cdRect = coinDisplay.GetComponent<RectTransform>();
        cdRect.anchorMin = new Vector2(1, 0);
        cdRect.anchorMax = new Vector2(1, 0);
        cdRect.anchoredPosition = new Vector2(-90, 50);
        cdRect.sizeDelta = new Vector2(140, 45);

        GameObject coinLabel = CreateText(coinDisplay.transform, "CoinLabel", "\u25CF 140", 20, Vector2.zero);
        coinLabel.GetComponent<TextMeshProUGUI>().color = accentGold;
        coinLabel.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Message panel with rounded corners
        GameObject messagePanel = CreateRoundedPanel(canvas.transform, "MessagePanel", new Vector2(500, 70), roundedMed, new Color(0.04f, 0.08f, 0.12f, 0.95f));
        RectTransform msgRect = messagePanel.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.anchoredPosition = new Vector2(0, 150);
        uiManager.messagePanel = messagePanel;
        uiManager.messageText = CreateText(messagePanel.transform, "MessageText", "", 24, Vector2.zero).GetComponent<TextMeshProUGUI>();
        messagePanel.SetActive(false);

        // Level complete panel with rounded corners
        GameObject levelCompletePanel = CreateRoundedPanel(canvas.transform, "LevelCompletePanel", new Vector2(600, 450), roundedLarge, new Color(0.03f, 0.22f, 0.28f, 0.98f));
        RectTransform lcRect = levelCompletePanel.GetComponent<RectTransform>();
        lcRect.anchorMin = new Vector2(0.5f, 0.5f);
        lcRect.anchorMax = new Vector2(0.5f, 0.5f);
        lcRect.anchoredPosition = Vector2.zero;

        GameObject lcShadow = new GameObject("Shadow");
        lcShadow.transform.SetParent(levelCompletePanel.transform, false);
        RectTransform lcShadowRect = lcShadow.AddComponent<RectTransform>();
        lcShadowRect.anchorMin = Vector2.zero;
        lcShadowRect.anchorMax = Vector2.one;
        lcShadowRect.sizeDelta = new Vector2(8, -8);
        lcShadowRect.anchoredPosition = new Vector2(3, -3);
        Image lcShadowImg = lcShadow.AddComponent<Image>();
        lcShadowImg.sprite = roundedLarge;
        lcShadowImg.color = new Color(0, 0, 0, 0.3f);
        lcShadow.transform.SetAsFirstSibling();

        CreateText(levelCompletePanel.transform, "CompleteTitle", "LEVEL COMPLETE!", 38, new Vector2(0, 130));
        GameObject starsText = CreateText(levelCompletePanel.transform, "StarsText", "\u2605 \u2605 \u2605", 48, new Vector2(0, 60));
        starsText.GetComponent<TextMeshProUGUI>().color = accentGold;
        CreateText(levelCompletePanel.transform, "CompleteScore", "Score: 0", 26, new Vector2(0, 0));
        CreateText(levelCompletePanel.transform, "CompleteCoins", "+0 Coins", 26, new Vector2(0, -40));

        GameObject continueBtn = CreateGradientButton(levelCompletePanel.transform, "ContinueButton", "CONTINUE", new Vector2(0, -120), accentGreen, new Color(0.18f, 0.60f, 0.28f), 280, 60, roundedMed);
        AddButtonShadow(continueBtn);

        uiManager.levelCompletePanel = levelCompletePanel;
        levelCompletePanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GameScene.unity");
    }

    static void AddGradientOverlay(GameObject parent, Color topColor, Color bottomColor)
    {
        GameObject overlay = new GameObject("GradientOverlay");
        overlay.transform.SetParent(parent.transform, false);
        RectTransform rect = overlay.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        Image img = overlay.AddComponent<Image>();
        img.sprite = UISpriteGenerator.CreateGradient(128, 128, topColor, bottomColor);
        img.type = Image.Type.Simple;
        img.raycastTarget = false;
    }

    static void CreateButtonBadge(GameObject btnObj, string badgeText)
    {
        GameObject badgeObj = new GameObject("Badge");
        badgeObj.transform.SetParent(btnObj.transform, false);
        RectTransform bRect = badgeObj.AddComponent<RectTransform>();
        bRect.anchorMin = new Vector2(1, 1);
        bRect.anchorMax = new Vector2(1, 1);
        bRect.sizeDelta = new Vector2(28, 28);
        bRect.anchoredPosition = new Vector2(8, 8);
        Image bImg = badgeObj.AddComponent<Image>();
        bImg.sprite = badge;
        bImg.color = new Color(0.9f, 0.25f, 0.3f, 0.95f);

        GameObject badgeTextObj = new GameObject("Text");
        badgeTextObj.transform.SetParent(badgeObj.transform, false);
        RectTransform btRect = badgeTextObj.AddComponent<RectTransform>();
        btRect.anchorMin = Vector2.zero;
        btRect.anchorMax = Vector2.one;
        btRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI btmp = badgeTextObj.AddComponent<TextMeshProUGUI>();
        btmp.text = badgeText;
        btmp.fontSize = 14;
        btmp.fontStyle = FontStyles.Bold;
        btmp.alignment = TextAlignmentOptions.Center;
        btmp.color = Color.white;
        if (gameFontAsset != null) btmp.font = gameFontAsset;
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
        sImg.sprite = roundedMed;
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

        // Add subtle radial gradient overlay
        GameObject gradientOverlay = new GameObject("BackgroundGradient");
        gradientOverlay.transform.SetParent(bg.transform, false);
        RectTransform gRect = gradientOverlay.AddComponent<RectTransform>();
        gRect.anchorMin = Vector2.zero;
        gRect.anchorMax = Vector2.one;
        gRect.sizeDelta = Vector2.zero;
        gRect.anchoredPosition = Vector2.zero;
        Image gImg = gradientOverlay.AddComponent<Image>();
        gImg.sprite = UISpriteGenerator.CreateGradient(128, 128, new Color(0.08f, 0.50f, 0.60f, 0.4f), new Color(0.02f, 0.30f, 0.38f, 0.2f));
        gImg.type = Image.Type.Simple;
        gImg.raycastTarget = false;
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
        if (gameFontAsset != null) tmp.font = gameFontAsset;
        return textObj;
    }

    static GameObject CreateRoundedPanel(Transform parent, string name, Vector2 size, Sprite sprite, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        Image img = panel.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.color = color;
        return panel;
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

    static GameObject CreateGradientButton(Transform parent, string name, string label, Vector2 position, Color topColor, Color bottomColor, float width, float height, Sprite sprite, Vector2? anchorMin = null, Vector2? anchorMax = null, int fontSize = 24)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        if (anchorMin.HasValue) rect.anchorMin = anchorMin.Value;
        if (anchorMax.HasValue) rect.anchorMax = anchorMax.Value;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, height);

        Image img = btnObj.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.color = topColor;

        // Add gradient overlay
        GameObject gradientObj = new GameObject("Gradient");
        gradientObj.transform.SetParent(btnObj.transform, false);
        RectTransform gRect = gradientObj.AddComponent<RectTransform>();
        gRect.anchorMin = Vector2.zero;
        gRect.anchorMax = Vector2.one;
        gRect.sizeDelta = Vector2.zero;
        gRect.anchoredPosition = Vector2.zero;
        Image gImg = gradientObj.AddComponent<Image>();
        gImg.sprite = UISpriteGenerator.CreateGradient(128, 128, new Color(1, 1, 1, 0.15f), new Color(0, 0, 0, 0.15f));
        gImg.type = Image.Type.Simple;
        gImg.raycastTarget = false;

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
        if (gameFontAsset != null) tmp.font = gameFontAsset;

        return btnObj;
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
