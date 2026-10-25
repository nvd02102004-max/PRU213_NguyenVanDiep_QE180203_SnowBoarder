using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    [Header("Scene Routing")]
    [SerializeField] private string gameLevelName = "Level1";
    [SerializeField] private string mainMenuName = "MainMenu";

    private Canvas endCanvas;
    private Text scoreCounterText;
    private Text rankTitleText;
    private GameObject newRecordBadge;
    private Image[] starImages = new Image[3];

    private int finalScore = 0;
    private int highScore = 0;
    private bool playerWon = false;

    void Awake()
    {
        EnsureEventSystem();

        finalScore = PlayerPrefs.GetInt("SnowBoarder_LastScore", 0);
        highScore = PlayerPrefs.GetInt("SnowBoarder_HighScore", 0);
        playerWon = PlayerPrefs.GetInt("SnowBoarder_PlayerWon", 1) == 1;

        if (GameManager.Instance != null)
        {
            finalScore = GameManager.Instance.Score;
            highScore = GameManager.Instance.HighScore;
            playerWon = GameManager.Instance.PlayerWon;
        }

        BuildModernEndUI();
    }

    void Start()
    {
        if (playerWon && UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayVictory();
        }

        StartCoroutine(AnimateScoreRoutine());
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

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(gameLevelName);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRunState();
        }
        SceneManager.LoadScene(mainMenuName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator AnimateScoreRoutine()
    {
        // 1. Roll numbers from 0 to finalScore
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - (1f - t) * (1f - t);
            int currentVal = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
            if (scoreCounterText != null)
            {
                scoreCounterText.text = $"{currentVal:N0} PTS";
            }
            yield return null;
        }

        if (scoreCounterText != null)
        {
            scoreCounterText.text = $"{finalScore:N0} PTS";
        }

        // 2. Animate stars pop-in
        int starCount = 1;
        if (finalScore >= 2000) starCount = 3;
        else if (finalScore >= 1000) starCount = 2;

        Sprite starFilled = UIProceduralSprites.GetStarFilled();

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSecondsRealtime(0.18f);

            if (i < starCount)
            {
                starImages[i].sprite = starFilled;
                starImages[i].color = Color.white;
                StartCoroutine(PopStarAnimation(starImages[i].rectTransform));
                if (UISoundManager.Instance != null)
                {
                    UISoundManager.Instance.PlayPop();
                }
            }
        }

        // 3. New record badge check
        if (finalScore >= highScore && finalScore > 0 && newRecordBadge != null)
        {
            newRecordBadge.SetActive(true);
            StartCoroutine(PulseRecordBadge(newRecordBadge.GetComponent<RectTransform>()));
        }
    }

    private IEnumerator PopStarAnimation(RectTransform rt)
    {
        float dur = 0.22f;
        float el = 0f;
        while (el < dur)
        {
            el += Time.unscaledDeltaTime;
            float t = el / dur;
            float scale = Mathf.Sin(t * Mathf.PI * 0.75f) * 1.35f;
            rt.localScale = Vector3.one * Mathf.Max(scale, 1f);
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    private IEnumerator PulseRecordBadge(RectTransform rt)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.unscaledDeltaTime * 4f;
            float s = 1f + Mathf.Sin(timer) * 0.06f;
            if (rt != null) rt.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
    }

    // ==========================================
    // PROCEDURAL MODERN END UI BUILDER
    // ==========================================
    private void BuildModernEndUI()
    {
        // 1. Root Canvas
        GameObject canvasObj = new GameObject("ModernEndCanvas");
        endCanvas = canvasObj.AddComponent<Canvas>();
        endCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        endCanvas.sortingOrder = 10;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        endCanvas.pixelPerfect = true;

        canvasObj.AddComponent<GraphicRaycaster>();

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 2. High-contrast Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();

        Color topBg = playerWon
            ? new Color(0.04f, 0.08f, 0.18f, 1f)  // Deep triumphant midnight blue
            : new Color(0.12f, 0.05f, 0.08f, 1f); // Dark crimson dusk on crash
        Color btmBg = playerWon
            ? new Color(0.08f, 0.18f, 0.32f, 1f)
            : new Color(0.05f, 0.03f, 0.06f, 1f);

        bgImg.sprite = UIProceduralSprites.CreateGradientRoundedSprite(64, 64, 0, topBg, btmBg, Color.clear, 0);
        RectTransform bgRT = bgObj.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        // 3. Ambient snowfall in background
        GameObject sceneryObj = new GameObject("SceneryEffects");
        sceneryObj.transform.SetParent(canvasObj.transform, false);
        UIBackgroundScenery scenery = sceneryObj.AddComponent<UIBackgroundScenery>();
        scenery.Initialize(canvasObj.GetComponent<RectTransform>());

        // 4. Center End Summary Card
        GameObject cardObj = new GameObject("EndSummaryCard", typeof(RectTransform));
        cardObj.transform.SetParent(canvasObj.transform, false);
        Image cardImg = cardObj.AddComponent<Image>();
        cardImg.sprite = UIProceduralSprites.GetCardBackground();
        cardImg.type = Image.Type.Sliced;

        RectTransform cardRT = cardObj.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(620, 780);
        cardRT.anchoredPosition = Vector2.zero;

        // 4.1 Header Status Capsule Badge
        GameObject badgeObj = new GameObject("StatusBadge", typeof(RectTransform));
        badgeObj.transform.SetParent(cardObj.transform, false);
        Image badgeImg = badgeObj.AddComponent<Image>();
        badgeImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform badgeRT = badgeObj.GetComponent<RectTransform>();
        badgeRT.sizeDelta = new Vector2(320, 40);
        badgeRT.anchoredPosition = new Vector2(0, 335);

        GameObject badgeTxtObj = new GameObject("StatusBadgeText", typeof(RectTransform));
        badgeTxtObj.transform.SetParent(badgeObj.transform, false);
        Text badgeTxt = badgeTxtObj.AddComponent<Text>();
        badgeTxt.font = defaultFont;
        badgeTxt.text = playerWon ? "\u2605 MISSION ACCOMPLISHED \u2605" : "\u26A0 RUN TERMINATED \u26A0";
        badgeTxt.fontSize = 16;
        badgeTxt.fontStyle = FontStyle.Bold;
        badgeTxt.alignment = TextAnchor.MiddleCenter;
        badgeTxt.color = playerWon ? new Color(0.4f, 0.95f, 0.6f, 1f) : new Color(1f, 0.45f, 0.45f, 1f);
        badgeTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        badgeTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform badgeTxtRT = badgeTxtObj.GetComponent<RectTransform>();
        badgeTxtRT.sizeDelta = new Vector2(320, 40);

        // 4.2 Dynamic Title (Layered Typography with 3D Depth)
        string titleStr = playerWon ? "\uD83C\uDFC6 TRACK COMPLETED!" : "\uD83D\uDCA5 CRASHED! GAME OVER";
        Color titleColor = playerWon ? new Color(1f, 0.88f, 0.25f, 1f) : new Color(1f, 0.35f, 0.35f, 1f);
        CreateLayeredTitle(cardObj.transform, defaultFont, titleStr, titleColor, new Vector2(0, 275));

        // 4.3 Subtitle Message
        GameObject subObj = new GameObject("Subtitle", typeof(RectTransform));
        subObj.transform.SetParent(cardObj.transform, false);
        Text subTxt = subObj.AddComponent<Text>();
        subTxt.font = defaultFont;
        subTxt.text = playerWon
            ? "Outstanding freestyle control and perfect downhill finish!"
            : "Keep your board balanced and avoid steep head landings!";
        subTxt.fontSize = 20;
        subTxt.fontStyle = FontStyle.Italic;
        subTxt.alignment = TextAnchor.MiddleCenter;
        subTxt.color = new Color(0.85f, 0.94f, 1f, 1f);
        subTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        subTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform subRT = subObj.GetComponent<RectTransform>();
        subRT.sizeDelta = new Vector2(580, 40);
        subRT.anchoredPosition = new Vector2(0, 220);

        // 4.4 3-Star Rating Row
        GameObject starRow = new GameObject("StarRatingRow", typeof(RectTransform));
        starRow.transform.SetParent(cardObj.transform, false);
        RectTransform starRowRT = starRow.GetComponent<RectTransform>();
        starRowRT.sizeDelta = new Vector2(320, 76);
        starRowRT.anchoredPosition = new Vector2(0, 155);

        Sprite starEmpty = UIProceduralSprites.GetStarEmpty();
        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = new GameObject($"Star_{i}", typeof(RectTransform));
            starObj.transform.SetParent(starRow.transform, false);
            Image sImg = starObj.AddComponent<Image>();
            sImg.sprite = starEmpty;
            sImg.color = new Color(0.4f, 0.5f, 0.65f, 0.5f);

            RectTransform sRT = starObj.GetComponent<RectTransform>();
            sRT.sizeDelta = new Vector2(64, 64);
            sRT.anchoredPosition = new Vector2((i - 1) * 85f, 0);

            starImages[i] = sImg;
        }

        // 4.5 Performance Rank Label
        GameObject rankObj = new GameObject("RankLabel", typeof(RectTransform));
        rankObj.transform.SetParent(cardObj.transform, false);
        rankTitleText = rankObj.AddComponent<Text>();
        rankTitleText.font = defaultFont;
        string rankStr = finalScore >= 2000 ? "RANK: S \u2605 SNOW LEGEND" :
                         finalScore >= 1000 ? "RANK: A \u2605 PRO BOARDER" : "RANK: B \u2605 AMATEUR RIDER";
        rankTitleText.text = rankStr;
        rankTitleText.fontSize = 24;
        rankTitleText.fontStyle = FontStyle.Bold;
        rankTitleText.alignment = TextAnchor.MiddleCenter;
        rankTitleText.color = new Color(0.40f, 0.92f, 1f, 1f);
        rankTitleText.horizontalOverflow = HorizontalWrapMode.Overflow;
        rankTitleText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rankRT = rankObj.GetComponent<RectTransform>();
        rankRT.sizeDelta = new Vector2(520, 36);
        rankRT.anchoredPosition = new Vector2(0, 95);

        // 4.6 Score Stat Display Box
        GameObject scoreBoxObj = new GameObject("ScoreStatsBox", typeof(RectTransform));
        scoreBoxObj.transform.SetParent(cardObj.transform, false);
        Image scoreBoxImg = scoreBoxObj.AddComponent<Image>();
        scoreBoxImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform scoreBoxRT = scoreBoxObj.GetComponent<RectTransform>();
        scoreBoxRT.sizeDelta = new Vector2(500, 110);
        scoreBoxRT.anchoredPosition = new Vector2(0, 10);

        // "FINAL SCORE" Label
        GameObject finalLblObj = new GameObject("FinalScoreLabel", typeof(RectTransform));
        finalLblObj.transform.SetParent(scoreBoxObj.transform, false);
        Text finalLbl = finalLblObj.AddComponent<Text>();
        finalLbl.font = defaultFont;
        finalLbl.text = "FINAL SCORE";
        finalLbl.fontSize = 18;
        finalLbl.fontStyle = FontStyle.Bold;
        finalLbl.alignment = TextAnchor.MiddleCenter;
        finalLbl.color = new Color(0.80f, 0.90f, 1f, 1f);
        finalLbl.horizontalOverflow = HorizontalWrapMode.Overflow;
        finalLbl.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform finalLblRT = finalLblObj.GetComponent<RectTransform>();
        finalLblRT.sizeDelta = new Vector2(460, 26);
        finalLblRT.anchoredPosition = new Vector2(0, 30);

        // Animated Score Counter
        GameObject scoreCounterObj = new GameObject("ScoreCounterText", typeof(RectTransform));
        scoreCounterObj.transform.SetParent(scoreBoxObj.transform, false);
        scoreCounterText = scoreCounterObj.AddComponent<Text>();
        scoreCounterText.font = defaultFont;
        scoreCounterText.text = "0 PTS";
        scoreCounterText.fontSize = 42;
        scoreCounterText.fontStyle = FontStyle.Bold;
        scoreCounterText.alignment = TextAnchor.MiddleCenter;
        scoreCounterText.color = Color.white;
        scoreCounterText.horizontalOverflow = HorizontalWrapMode.Overflow;
        scoreCounterText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform scoreCounterRT = scoreCounterObj.GetComponent<RectTransform>();
        scoreCounterRT.sizeDelta = new Vector2(460, 52);
        scoreCounterRT.anchoredPosition = new Vector2(0, -10);

        Outline scoreOutline = scoreCounterObj.AddComponent<Outline>();
        scoreOutline.effectColor = new Color(0.05f, 0.25f, 0.5f, 0.85f);
        scoreOutline.effectDistance = new Vector2(1, -1);

        // High Score / Record comparison
        GameObject bestObj = new GameObject("HighScoreComparison", typeof(RectTransform));
        bestObj.transform.SetParent(cardObj.transform, false);
        Text bestTxt = bestObj.AddComponent<Text>();
        bestTxt.font = defaultFont;
        bestTxt.text = $"ALL-TIME HIGH SCORE: {highScore:N0} PTS";
        bestTxt.fontSize = 20;
        bestTxt.alignment = TextAnchor.MiddleCenter;
        bestTxt.color = new Color(0.90f, 0.95f, 1f, 1f);
        bestTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        bestTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform bestRT = bestObj.GetComponent<RectTransform>();
        bestRT.sizeDelta = new Vector2(520, 30);
        bestRT.anchoredPosition = new Vector2(0, -75);

        // New Record Banner Badge
        newRecordBadge = new GameObject("NewRecordBadge", typeof(RectTransform));
        newRecordBadge.transform.SetParent(cardObj.transform, false);
        Image nrbImg = newRecordBadge.AddComponent<Image>();
        nrbImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform nrbRT = newRecordBadge.GetComponent<RectTransform>();
        nrbRT.sizeDelta = new Vector2(340, 36);
        nrbRT.anchoredPosition = new Vector2(0, -115);

        GameObject nrbTxtObj = new GameObject("NewRecordText", typeof(RectTransform));
        nrbTxtObj.transform.SetParent(newRecordBadge.transform, false);
        Text nrbTxt = nrbTxtObj.AddComponent<Text>();
        nrbTxt.font = defaultFont;
        nrbTxt.text = "\u2605 NEW ALL-TIME RECORD! \u2605";
        nrbTxt.fontSize = 16;
        nrbTxt.fontStyle = FontStyle.Bold;
        nrbTxt.alignment = TextAnchor.MiddleCenter;
        nrbTxt.color = new Color(1f, 0.85f, 0.2f, 1f);
        nrbTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        nrbTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform nrbTxtRT = nrbTxtObj.GetComponent<RectTransform>();
        nrbTxtRT.sizeDelta = new Vector2(340, 36);

        newRecordBadge.SetActive(false);

        // 4.7 Standardized Buttons
        CreateModernButton(
            cardObj.transform, defaultFont,
            "RACE AGAIN", ModernUIButton.ButtonType.Primary,
            new Vector2(380, 66), new Vector2(0, -180),
            PlayAgain, true, 26
        );

        CreateModernButton(
            cardObj.transform, defaultFont,
            "MAIN MENU", ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -252),
            ReturnToMainMenu, false, 22
        );

        CreateModernButton(
            cardObj.transform, defaultFont,
            "QUIT GAME", ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -318),
            QuitGame, false, 22
        );
    }

    private void CreateLayeredTitle(Transform parent, Font font, string text, Color color, Vector2 pos)
    {
        GameObject titleRoot = new GameObject("EndTitleGroup", typeof(RectTransform));
        titleRoot.transform.SetParent(parent, false);
        RectTransform rootRT = titleRoot.GetComponent<RectTransform>();
        rootRT.anchoredPosition = pos;
        rootRT.sizeDelta = new Vector2(600, 56);

        GameObject shadowObj = new GameObject("TitleShadow", typeof(RectTransform));
        shadowObj.transform.SetParent(titleRoot.transform, false);
        Text shadowText = shadowObj.AddComponent<Text>();
        shadowText.font = font;
        shadowText.text = text;
        shadowText.fontSize = 36;
        shadowText.fontStyle = FontStyle.Bold;
        shadowText.alignment = TextAnchor.MiddleCenter;
        shadowText.color = new Color(0f, 0f, 0f, 0.8f);
        shadowText.horizontalOverflow = HorizontalWrapMode.Overflow;
        shadowText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform shadowRT = shadowObj.GetComponent<RectTransform>();
        shadowRT.anchoredPosition = new Vector2(0, -3);
        shadowRT.sizeDelta = new Vector2(600, 56);

        GameObject mainObj = new GameObject("TitleMain", typeof(RectTransform));
        mainObj.transform.SetParent(titleRoot.transform, false);
        Text mainText = mainObj.AddComponent<Text>();
        mainText.font = font;
        mainText.text = text;
        mainText.fontSize = 36;
        mainText.fontStyle = FontStyle.Bold;
        mainText.alignment = TextAnchor.MiddleCenter;
        mainText.color = color;
        mainText.horizontalOverflow = HorizontalWrapMode.Overflow;
        mainText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform mainRT = mainObj.GetComponent<RectTransform>();
        mainRT.anchoredPosition = Vector2.zero;
        mainRT.sizeDelta = new Vector2(600, 56);

        Outline outline = mainObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0.15f, 0.35f, 0.9f);
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
        sh.effectColor = new Color(0f, 0f, 0f, 0.5f);
        sh.effectDistance = new Vector2(1, -1);

        ModernUIButton modernBtn = btnObj.AddComponent<ModernUIButton>();
        modernBtn.Setup(type, label, clickAction, pulse);
    }
}
