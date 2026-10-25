using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string firstLevelName = "Level1";

    [Header("Canvas & Layout Elements")]
    private Canvas mainCanvas;
    private GameObject mainCardObj;
    private GameObject optionsModalObj;
    private GameObject howToPlayModalObj;

    private Slider volumeSlider;
    private Text volumeValueText;
    private Text highScoreText;

    private float audioVolume = 1f;

    void Awake()
    {
        audioVolume = AudioListener.volume;
        EnsureEventSystem();
        BuildModernUI();
    }

    void Start()
    {
        if (optionsModalObj != null) optionsModalObj.SetActive(false);
        if (howToPlayModalObj != null) howToPlayModalObj.SetActive(false);
        UpdateHighScoreDisplay();
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void UpdateHighScoreDisplay()
    {
        int best = PlayerPrefs.GetInt("SnowBoarder_HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = $"\u2605 BEST RECORD: {best:N0} PTS \u2605";
        }
    }

    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRunState();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenHowToPlay()
    {
        if (howToPlayModalObj != null)
        {
            if (mainCardObj != null) mainCardObj.SetActive(false);
            howToPlayModalObj.SetActive(true);
            AnimateModalIn(howToPlayModalObj);
        }
    }

    public void CloseHowToPlay()
    {
        if (howToPlayModalObj != null)
        {
            howToPlayModalObj.SetActive(false);
            if (mainCardObj != null) mainCardObj.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        if (optionsModalObj != null)
        {
            if (mainCardObj != null) mainCardObj.SetActive(false);
            optionsModalObj.SetActive(true);
            AnimateModalIn(optionsModalObj);
        }
    }

    public void CloseSettings()
    {
        if (optionsModalObj != null)
        {
            optionsModalObj.SetActive(false);
            if (mainCardObj != null) mainCardObj.SetActive(true);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResetHighScore()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetHighScore();
        }
        else
        {
            PlayerPrefs.DeleteKey("SnowBoarder_HighScore");
            PlayerPrefs.Save();
        }
        UpdateHighScoreDisplay();
        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayPop();
        }
    }

    private void OnVolumeChanged(float val)
    {
        audioVolume = val;
        AudioListener.volume = audioVolume;
        if (volumeValueText != null)
        {
            volumeValueText.text = $"{Mathf.RoundToInt(audioVolume * 100)}%";
        }
    }

    private void AnimateModalIn(GameObject modal)
    {
        CanvasGroup cg = modal.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            StartCoroutine(FadeInRoutine(cg));
        }
    }

    private IEnumerator FadeInRoutine(CanvasGroup cg)
    {
        cg.alpha = 0f;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    // ==========================================
    // PROCEDURAL MODERN UI BUILDER
    // ==========================================
    private void BuildModernUI()
    {
        // 1. Root Canvas
        GameObject canvasObj = new GameObject("ModernMenuCanvas", typeof(RectTransform));
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 10;
        mainCanvas.pixelPerfect = false;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            scaler.dynamicPixelsPerUnit = 2f;
        }

        canvasObj.AddComponent<GraphicRaycaster>();

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 2. High-Contrast Deep Navy Gradient Background
        GameObject bgObj = new GameObject("Background", typeof(RectTransform));
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.sprite = UIProceduralSprites.CreateGradientRoundedSprite(
            64, 64, 0,
            new Color(0.04f, 0.07f, 0.14f, 1f),
            new Color(0.08f, 0.16f, 0.28f, 1f),
            Color.clear, 0
        );
        RectTransform bgRT = bgObj.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        // 3. Ambient Scenery (Clouds & Snowflakes)
        GameObject sceneryObj = new GameObject("SceneryEffects", typeof(RectTransform));
        sceneryObj.transform.SetParent(canvasObj.transform, false);
        UIBackgroundScenery scenery = sceneryObj.AddComponent<UIBackgroundScenery>();
        scenery.Initialize(canvasObj.GetComponent<RectTransform>());

        // 4. Center UI Card (Frosted Glass Container)
        mainCardObj = new GameObject("MainCardPanel", typeof(RectTransform));
        mainCardObj.transform.SetParent(canvasObj.transform, false);
        Image cardImg = mainCardObj.AddComponent<Image>();
        cardImg.sprite = UIProceduralSprites.GetCardBackground();
        cardImg.type = Image.Type.Sliced;

        RectTransform cardRT = mainCardObj.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(580, 740);
        cardRT.anchoredPosition = new Vector2(0, 0);

        // 4.1 Sub-badge Capsule
        GameObject badgeObj = new GameObject("CapsuleBadge", typeof(RectTransform));
        badgeObj.transform.SetParent(mainCardObj.transform, false);
        Image badgeImg = badgeObj.AddComponent<Image>();
        badgeImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform badgeRT = badgeObj.GetComponent<RectTransform>();
        badgeRT.sizeDelta = new Vector2(300, 40);
        badgeRT.anchoredPosition = new Vector2(0, 305);

        GameObject badgeTextObj = new GameObject("BadgeText", typeof(RectTransform));
        badgeTextObj.transform.SetParent(badgeObj.transform, false);
        Text badgeText = badgeTextObj.AddComponent<Text>();
        badgeText.font = defaultFont;
        badgeText.text = "EXTREME WINTER ACTION";
        badgeText.fontSize = 15;
        badgeText.fontStyle = FontStyle.Bold;
        badgeText.alignment = TextAnchor.MiddleCenter;
        badgeText.color = new Color(0.40f, 0.92f, 1f, 1f);
        RectTransform badgeTextRT = badgeTextObj.GetComponent<RectTransform>();
        badgeTextRT.sizeDelta = new Vector2(300, 40);

        // 4.2 Game Title
        CreateLayeredTitle(mainCardObj.transform, defaultFont, new Vector2(0, 230));

        // 4.3 Subtitle Banner
        GameObject subObj = new GameObject("SubtitleText", typeof(RectTransform));
        subObj.transform.SetParent(mainCardObj.transform, false);
        Text subText = subObj.AddComponent<Text>();
        subText.font = defaultFont;
        subText.text = "Downhill Rush & Freestyle Trick Master";
        subText.fontSize = 18;
        subText.fontStyle = FontStyle.Italic;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.85f, 0.94f, 1f, 1f);
        RectTransform subRT = subObj.GetComponent<RectTransform>();
        subRT.sizeDelta = new Vector2(520, 30);
        subRT.anchoredPosition = new Vector2(0, 170);

        // 4.4 High Score Ribbon
        GameObject hsObj = new GameObject("HighScoreBanner", typeof(RectTransform));
        hsObj.transform.SetParent(mainCardObj.transform, false);
        highScoreText = hsObj.AddComponent<Text>();
        highScoreText.font = defaultFont;
        highScoreText.fontSize = 22;
        highScoreText.fontStyle = FontStyle.Bold;
        highScoreText.alignment = TextAnchor.MiddleCenter;
        highScoreText.color = new Color(1f, 0.85f, 0.25f, 1f);
        RectTransform hsRT = hsObj.GetComponent<RectTransform>();
        hsRT.sizeDelta = new Vector2(480, 38);
        hsRT.anchoredPosition = new Vector2(0, 120);

        Shadow hsShadow = hsObj.AddComponent<Shadow>();
        hsShadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        hsShadow.effectDistance = new Vector2(0, -2);

        // 4.5 Standardized Buttons
        CreateModernButton(
            mainCardObj.transform, defaultFont,
            "START RUN", ModernUIButton.ButtonType.Primary,
            new Vector2(380, 68), new Vector2(0, 35),
            StartGame, true, 26
        );

        CreateModernButton(
            mainCardObj.transform, defaultFont,
            "HOW TO PLAY", ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -42),
            OpenHowToPlay, false, 22
        );

        CreateModernButton(
            mainCardObj.transform, defaultFont,
            "SETTINGS & AUDIO", ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -112),
            OpenSettings, false, 22
        );

        CreateModernButton(
            mainCardObj.transform, defaultFont,
            "QUIT GAME", ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -182),
            QuitGame, false, 22
        );

        // 4.6 Footer Tag
        GameObject footerObj = new GameObject("FooterText", typeof(RectTransform));
        footerObj.transform.SetParent(mainCardObj.transform, false);
        Text footerText = footerObj.AddComponent<Text>();
        footerText.font = defaultFont;
        footerText.text = "Lab 02 \u2022 PRU213 \u2022 Unity 2D";
        footerText.fontSize = 15;
        footerText.alignment = TextAnchor.MiddleCenter;
        footerText.color = new Color(0.55f, 0.70f, 0.88f, 0.8f);
        RectTransform footerRT = footerObj.GetComponent<RectTransform>();
        footerRT.sizeDelta = new Vector2(360, 28);
        footerRT.anchoredPosition = new Vector2(0, -325);

        // 5. Build Modal Windows
        BuildHowToPlayModal(canvasObj.transform, defaultFont);
        BuildSettingsModal(canvasObj.transform, defaultFont);
    }

    private void CreateLayeredTitle(Transform parent, Font font, Vector2 position)
    {
        GameObject titleRoot = new GameObject("GameTitleGroup", typeof(RectTransform));
        titleRoot.transform.SetParent(parent, false);
        RectTransform rootRT = titleRoot.GetComponent<RectTransform>();
        rootRT.anchoredPosition = position;
        rootRT.sizeDelta = new Vector2(560, 64);

        // Drop shadow layer
        GameObject shadowObj = new GameObject("TitleShadow", typeof(RectTransform));
        shadowObj.transform.SetParent(titleRoot.transform, false);
        Text shadowText = shadowObj.AddComponent<Text>();
        shadowText.font = font;
        shadowText.text = "\u2744  SNOW BOARDER  \u2744";
        shadowText.fontSize = 40;
        shadowText.fontStyle = FontStyle.Bold;
        shadowText.alignment = TextAnchor.MiddleCenter;
        shadowText.color = new Color(0.01f, 0.03f, 0.08f, 0.95f);
        RectTransform shadowRT = shadowObj.GetComponent<RectTransform>();
        shadowRT.anchoredPosition = new Vector2(0, -3);
        shadowRT.sizeDelta = new Vector2(560, 64);

        // Mid glow layer
        GameObject glowObj = new GameObject("TitleGlow", typeof(RectTransform));
        glowObj.transform.SetParent(titleRoot.transform, false);
        Text glowText = glowObj.AddComponent<Text>();
        glowText.font = font;
        glowText.text = "\u2744  SNOW BOARDER  \u2744";
        glowText.fontSize = 40;
        glowText.fontStyle = FontStyle.Bold;
        glowText.alignment = TextAnchor.MiddleCenter;
        glowText.color = new Color(0.20f, 0.80f, 1f, 0.50f);
        RectTransform glowRT = glowObj.GetComponent<RectTransform>();
        glowRT.anchoredPosition = new Vector2(0, -1);
        glowRT.sizeDelta = new Vector2(560, 64);

        // Top main layer
        GameObject mainObj = new GameObject("TitleForeground", typeof(RectTransform));
        mainObj.transform.SetParent(titleRoot.transform, false);
        Text mainText = mainObj.AddComponent<Text>();
        mainText.font = font;
        mainText.text = "\u2744  SNOW BOARDER  \u2744";
        mainText.fontSize = 40;
        mainText.fontStyle = FontStyle.Bold;
        mainText.alignment = TextAnchor.MiddleCenter;
        mainText.color = Color.white;
        RectTransform mainRT = mainObj.GetComponent<RectTransform>();
        mainRT.anchoredPosition = Vector2.zero;
        mainRT.sizeDelta = new Vector2(560, 64);

        Outline outline = mainObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.05f, 0.35f, 0.65f, 0.75f);
        outline.effectDistance = new Vector2(1, -1);
    }

    private void CreateModernButton(
        Transform parent, Font font, string label,
        ModernUIButton.ButtonType type, Vector2 size, Vector2 position,
        System.Action clickAction, bool pulse, int customFontSize = 0)
    {
        GameObject btnObj = new GameObject($"Btn_{label.Replace(" ", "_")}", typeof(RectTransform));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = position;

        Image img = btnObj.AddComponent<Image>();
        img.type = Image.Type.Sliced;

        GameObject txtObj = new GameObject("ButtonText", typeof(RectTransform));
        txtObj.transform.SetParent(btnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = font;
        txt.text = label;
        txt.fontSize = customFontSize > 0 ? customFontSize : (type == ModernUIButton.ButtonType.Primary ? 26 : 22);
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform txtRT = txtObj.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.sizeDelta = Vector2.zero;

        Shadow sh = txtObj.AddComponent<Shadow>();
        sh.effectColor = new Color(0f, 0f, 0f, 0.6f);
        sh.effectDistance = new Vector2(0, -2);

        ModernUIButton modernBtn = btnObj.AddComponent<ModernUIButton>();
        modernBtn.Setup(type, label, clickAction, pulse);
    }

    private void BuildHowToPlayModal(Transform parent, Font font)
    {
        howToPlayModalObj = CreateModalContainer(parent, "HowToPlayModal", 960, 700);
        Transform cardTransform = howToPlayModalObj.transform.Find("ModalCard") ?? howToPlayModalObj.transform;

        CreateModalHeader(cardTransform, font, "HOW TO PLAY & CONTROLS", 270f);

        CreateControlRow(cardTransform, font, "[A] / [D] or [\u2190] / [\u2192]", "Tilt & 360\u00B0 Air Flips", 170f);
        CreateControlRow(cardTransform, font, "[W] / [SHIFT] / [\u2191]", "Turbo Boost (45 km/h)", 90f);
        CreateControlRow(cardTransform, font, "[S] or [\u2193]", "Snow Brake (10 km/h)", 10f);

        // Tip Box
        GameObject tipObj = new GameObject("TrickTipBox", typeof(RectTransform));
        tipObj.transform.SetParent(cardTransform, false);
        Image tipImg = tipObj.AddComponent<Image>();
        tipImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform tipRT = tipObj.GetComponent<RectTransform>();
        tipRT.sizeDelta = new Vector2(880, 115);
        tipRT.anchoredPosition = new Vector2(0, -100);

        GameObject tipTxtObj = new GameObject("TipText", typeof(RectTransform));
        tipTxtObj.transform.SetParent(tipObj.transform, false);
        Text tipText = tipTxtObj.AddComponent<Text>();
        tipText.font = font;
        tipText.text = "\u2605 FREESTYLE COMBO TIP: Complete full 360\u00B0 rotations while airborne!\nLand parallel with the slope for a PERFECT LANDING (+50% score & Nitro refill).";
        tipText.fontSize = 24;
        tipText.lineSpacing = 1.3f;
        tipText.fontStyle = FontStyle.Bold;
        tipText.alignment = TextAnchor.MiddleCenter;
        tipText.color = new Color(0.95f, 0.98f, 1f, 1f);
        tipText.horizontalOverflow = HorizontalWrapMode.Wrap;
        tipText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform tipTxtRT = tipTxtObj.GetComponent<RectTransform>();
        tipTxtRT.sizeDelta = new Vector2(840, 100);

        Shadow tipSh = tipTxtObj.AddComponent<Shadow>();
        tipSh.effectColor = new Color(0.01f, 0.03f, 0.08f, 0.95f);
        tipSh.effectDistance = new Vector2(0, -2);

        // Close Button
        CreateModernButton(
            cardTransform, font, "GOT IT!",
            ModernUIButton.ButtonType.Primary,
            new Vector2(300, 64), new Vector2(0, -255),
            CloseHowToPlay, false, 26
        );

        howToPlayModalObj.SetActive(false);
    }

    private void BuildSettingsModal(Transform parent, Font font)
    {
        optionsModalObj = CreateModalContainer(parent, "SettingsModal", 880, 640);
        Transform cardTransform = optionsModalObj.transform.Find("ModalCard") ?? optionsModalObj.transform;

        CreateModalHeader(cardTransform, font, "GAME SETTINGS", 245f);

        // Volume Label
        GameObject volLblObj = new GameObject("VolumeLabel", typeof(RectTransform));
        volLblObj.transform.SetParent(cardTransform, false);
        Text volLbl = volLblObj.AddComponent<Text>();
        volLbl.font = font;
        volLbl.text = "MASTER AUDIO VOLUME";
        volLbl.fontSize = 24;
        volLbl.fontStyle = FontStyle.Bold;
        volLbl.alignment = TextAnchor.MiddleLeft;
        volLbl.color = new Color(0.95f, 0.98f, 1f, 1f);
        volLbl.horizontalOverflow = HorizontalWrapMode.Overflow;
        volLbl.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform volLblRT = volLblObj.GetComponent<RectTransform>();
        volLblRT.sizeDelta = new Vector2(500, 36);
        volLblRT.anchoredPosition = new Vector2(-120, 155);

        // Volume Value Text
        GameObject volValObj = new GameObject("VolumeValText", typeof(RectTransform));
        volValObj.transform.SetParent(cardTransform, false);
        volumeValueText = volValObj.AddComponent<Text>();
        volumeValueText.font = font;
        volumeValueText.text = $"{Mathf.RoundToInt(audioVolume * 100)}%";
        volumeValueText.fontSize = 26;
        volumeValueText.fontStyle = FontStyle.Bold;
        volumeValueText.alignment = TextAnchor.MiddleRight;
        volumeValueText.color = new Color(0.40f, 0.92f, 1f, 1f);
        volumeValueText.horizontalOverflow = HorizontalWrapMode.Overflow;
        volumeValueText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform volValRT = volValObj.GetComponent<RectTransform>();
        volValRT.sizeDelta = new Vector2(220, 36);
        volValRT.anchoredPosition = new Vector2(260, 155);

        CreateVolumeSlider(cardTransform, new Vector2(0, 100), 740f);

        // Reset High Score Section
        GameObject resetLblObj = new GameObject("ResetScoreLabel", typeof(RectTransform));
        resetLblObj.transform.SetParent(cardTransform, false);
        Text resetLbl = resetLblObj.AddComponent<Text>();
        resetLbl.font = font;
        resetLbl.text = "LOCAL DATA MANAGEMENT";
        resetLbl.fontSize = 22;
        resetLbl.fontStyle = FontStyle.Bold;
        resetLbl.alignment = TextAnchor.MiddleCenter;
        resetLbl.color = new Color(0.75f, 0.88f, 1f, 0.9f);
        resetLbl.horizontalOverflow = HorizontalWrapMode.Overflow;
        resetLbl.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform resetLblRT = resetLblObj.GetComponent<RectTransform>();
        resetLblRT.sizeDelta = new Vector2(600, 32);
        resetLblRT.anchoredPosition = new Vector2(0, 15);

        CreateModernButton(
            cardTransform, font, "RESET HIGH SCORE",
            ModernUIButton.ButtonType.Secondary,
            new Vector2(380, 58), new Vector2(0, -50),
            ResetHighScore, false, 22
        );

        CreateModernButton(
            cardTransform, font, "SAVE & CLOSE",
            ModernUIButton.ButtonType.Primary,
            new Vector2(320, 64), new Vector2(0, -185),
            CloseSettings, false, 26
        );

        optionsModalObj.SetActive(false);
    }

    private GameObject CreateModalContainer(Transform parent, string name, float width, float height)
    {
        GameObject overlay = new GameObject(name, typeof(RectTransform));
        overlay.transform.SetParent(parent, false);

        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0.02f, 0.05f, 0.12f, 0.88f);
        RectTransform overlayRT = overlay.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero;
        overlayRT.offsetMax = Vector2.zero;

        overlay.AddComponent<CanvasGroup>();

        GameObject card = new GameObject("ModalCard", typeof(RectTransform));
        card.transform.SetParent(overlay.transform, false);
        Image cardImg = card.AddComponent<Image>();
        cardImg.sprite = UIProceduralSprites.GetModalBackground();
        cardImg.type = Image.Type.Sliced;

        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.anchorMin = new Vector2(0.5f, 0.5f);
        cardRT.anchorMax = new Vector2(0.5f, 0.5f);
        cardRT.pivot = new Vector2(0.5f, 0.5f);
        cardRT.sizeDelta = new Vector2(width, height);
        cardRT.anchoredPosition = Vector2.zero;

        return overlay;
    }

    private void CreateModalHeader(Transform modalParent, Font font, string title, float yPos = 270f)
    {
        Transform cardTransform = modalParent.Find("ModalCard") ?? modalParent;

        GameObject headerObj = new GameObject("ModalHeader", typeof(RectTransform));
        headerObj.transform.SetParent(cardTransform, false);

        Text txt = headerObj.AddComponent<Text>();
        txt.font = font;
        txt.text = title;
        txt.fontSize = 36;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = new Color(0.40f, 0.92f, 1f, 1f);
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rt = headerObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(760, 52);
        rt.anchoredPosition = new Vector2(0, yPos);

        Shadow sh = headerObj.AddComponent<Shadow>();
        sh.effectColor = new Color(0f, 0.05f, 0.15f, 0.85f);
        sh.effectDistance = new Vector2(0, -3);
    }

    private void CreateControlRow(Transform modalParent, Font font, string keyLabel, string descLabel, float yPos)
    {
        Transform cardTransform = modalParent.Find("ModalCard") ?? modalParent;

        GameObject rowObj = new GameObject("ControlRow", typeof(RectTransform));
        rowObj.transform.SetParent(cardTransform, false);
        RectTransform rowRT = rowObj.GetComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(880, 64);
        rowRT.anchoredPosition = new Vector2(0, yPos);

        GameObject keycapObj = new GameObject("Keycap", typeof(RectTransform));
        keycapObj.transform.SetParent(rowObj.transform, false);
        Image keycapImg = keycapObj.AddComponent<Image>();
        keycapImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform keycapRT = keycapObj.GetComponent<RectTransform>();
        keycapRT.sizeDelta = new Vector2(360, 58);
        keycapRT.anchoredPosition = new Vector2(-240, 0);

        GameObject keycapTxtObj = new GameObject("KeyText", typeof(RectTransform));
        keycapTxtObj.transform.SetParent(keycapObj.transform, false);
        Text keyTxt = keycapTxtObj.AddComponent<Text>();
        keyTxt.font = font;
        keyTxt.text = keyLabel;
        keyTxt.fontSize = 24;
        keyTxt.fontStyle = FontStyle.Bold;
        keyTxt.alignment = TextAnchor.MiddleCenter;
        keyTxt.color = new Color(0.40f, 0.92f, 1f, 1f);
        keyTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        keyTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform keyTxtRT = keycapTxtObj.GetComponent<RectTransform>();
        keyTxtRT.sizeDelta = new Vector2(350, 56);

        Shadow keySh = keycapTxtObj.AddComponent<Shadow>();
        keySh.effectColor = new Color(0.01f, 0.03f, 0.08f, 0.95f);
        keySh.effectDistance = new Vector2(0, -2);

        GameObject descObj = new GameObject("DescText", typeof(RectTransform));
        descObj.transform.SetParent(rowObj.transform, false);
        Text descTxt = descObj.AddComponent<Text>();
        descTxt.font = font;
        descTxt.text = descLabel;
        descTxt.fontSize = 27;
        descTxt.fontStyle = FontStyle.Bold;
        descTxt.alignment = TextAnchor.MiddleLeft;
        descTxt.color = Color.white;
        descTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        descTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform descRT = descObj.GetComponent<RectTransform>();
        descRT.sizeDelta = new Vector2(480, 56);
        descRT.anchoredPosition = new Vector2(190, 0);

        Shadow descSh = descObj.AddComponent<Shadow>();
        descSh.effectColor = new Color(0.01f, 0.03f, 0.08f, 0.95f);
        descSh.effectDistance = new Vector2(0, -2);
    }

    private void CreateVolumeSlider(Transform cardTransform, Vector2 position, float width = 460f)
    {
        GameObject sliderObj = new GameObject("VolumeSlider", typeof(RectTransform));
        sliderObj.transform.SetParent(cardTransform, false);
        RectTransform sliderRT = sliderObj.GetComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(width, 32);
        sliderRT.anchoredPosition = position;

        volumeSlider = sliderObj.AddComponent<Slider>();
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = audioVolume;

        // Background Track
        GameObject trackObj = new GameObject("BackgroundTrack", typeof(RectTransform));
        trackObj.transform.SetParent(sliderObj.transform, false);
        Image trackImg = trackObj.AddComponent<Image>();
        trackImg.sprite = UIProceduralSprites.GetSliderTrack();
        trackImg.type = Image.Type.Sliced;
        RectTransform trackRT = trackObj.GetComponent<RectTransform>();
        trackRT.anchorMin = new Vector2(0f, 0.30f);
        trackRT.anchorMax = new Vector2(1f, 0.70f);
        trackRT.offsetMin = Vector2.zero;
        trackRT.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillArea.GetComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0f, 0.30f);
        fillAreaRT.anchorMax = new Vector2(1f, 0.70f);
        fillAreaRT.offsetMin = new Vector2(8f, 0f);
        fillAreaRT.offsetMax = new Vector2(-8f, 0f);

        // Fill
        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.sprite = UIProceduralSprites.GetSliderFill();
        fillImg.type = Image.Type.Sliced;
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0f, 0f);
        fillRT.anchorMax = new Vector2(0f, 1f);
        fillRT.sizeDelta = new Vector2(10f, 0f);
        volumeSlider.fillRect = fillRT;

        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRT = handleArea.GetComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.offsetMin = new Vector2(16f, 0f);
        handleAreaRT.offsetMax = new Vector2(-16f, 0f);

        // Handle
        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.sprite = UIProceduralSprites.GetSliderKnob();
        RectTransform handleRT = handle.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(32f, 32f);
        volumeSlider.handleRect = handleRT;

        volumeSlider.targetGraphic = handleImg;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }
}
