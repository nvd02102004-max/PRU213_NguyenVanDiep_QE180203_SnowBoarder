using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;

    private Canvas mainCanvas;
    public Canvas HUDCanvas => mainCanvas;

    // Top-Left Stats
    private Text scoreText;
    private Text highScoreText;
    private Text livesText;
    private GameObject comboBadgeObj;
    private Text comboBadgeText;

    // Top-Center Speedometer & Nitro
    private GameObject speedBadgeObj;
    private Image speedBadgeImg;
    private Text speedText;
    private RectTransform nitroFillRT;
    private Image nitroFillImg;
    private Text nitroLabelText;

    // Top-Center Trick Banner
    private GameObject trickBannerObj;
    private Text trickBannerText;
    private CanvasGroup trickBannerCg;
    private RectTransform trickBannerRT;

    // Top-Right Progress Bar
    private RectTransform progressFillRT;
    private RectTransform progressIconRT;
    private Text progressText;
    private float startX = -44f;
    private float finishX = 152f;

    // Pause Menu
    private GameObject pauseModalObj;
    private Slider pauseVolumeSlider;
    private Text pauseVolumeText;
    private bool isPaused = false;

    private string currentTrickMessage = "";
    private float trickDisplayTimer = 0f;

    void Awake()
    {
        EnsureEventSystem();
        BuildModernGameHUD();
    }

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (playerController != null)
        {
            startX = playerController.transform.position.x;
        }

        GameObject finishLine = GameObject.Find("Finish Line");
        if (finishLine != null)
        {
            finishX = finishLine.transform.position.x;
        }

        // Ensure GameManager exists in scene
        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

        UpdateStaticDisplays();
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

    void Update()
    {
        HandleKeyboardInput();
        UpdateDynamicHUD();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseModalObj != null)
        {
            pauseModalObj.SetActive(isPaused);
            if (isPaused)
            {
                if (pauseVolumeSlider != null) pauseVolumeSlider.value = AudioListener.volume;
                if (pauseVolumeText != null) pauseVolumeText.text = $"{Mathf.RoundToInt(AudioListener.volume * 100)}%";
            }
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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

    private void OnVolumeSliderChanged(float val)
    {
        AudioListener.volume = val;
        if (pauseVolumeText != null)
        {
            pauseVolumeText.text = $"{Mathf.RoundToInt(val * 100)}%";
        }
    }

    private void UpdateStaticDisplays()
    {
        if (GameManager.Instance != null && highScoreText != null)
        {
            highScoreText.text = $"\u2605 BEST: {GameManager.Instance.HighScore:N0}";
        }
    }

    private void UpdateDynamicHUD()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // 1. Score
        if (scoreText != null)
        {
            scoreText.text = $"{gm.Score:N0} PTS";
        }

        // 2. High Score
        if (highScoreText != null)
        {
            highScoreText.text = $"\u2605 BEST: {gm.HighScore:N0}";
        }

        // 3. Lives Display (\u2665 = solid heart, \u2661 = hollow heart)
        if (livesText != null)
        {
            int lives = Mathf.Clamp(gm.Lives, 0, 3);
            string hearts = "";
            for (int i = 0; i < 3; i++)
            {
                hearts += (i < lives) ? "<color=#EF4444>\u2665 </color>" : "<color=#475569>\u2661 </color>";
            }
            livesText.text = hearts.TrimEnd();
        }

        // 4. Combo Multiplier Badge
        if (comboBadgeObj != null)
        {
            if (gm.CurrentCombo > 1)
            {
                comboBadgeObj.SetActive(true);
                if (comboBadgeText != null)
                {
                    comboBadgeText.text = $"COMBO x{gm.Multiplier:0.0}!";
                }
            }
            else
            {
                comboBadgeObj.SetActive(false);
            }
        }

        // 5. Speedometer & Nitro Gauge
        float currentSpeed = playerController != null ? playerController.CurrentSpeed * 3.6f : 0f;
        if (speedText != null)
        {
            bool isBoosting = playerController != null && playerController.IsNitroActive;
            bool isBraking = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

            if (isBoosting)
            {
                speedText.text = $"\u26A1 TURBO {currentSpeed:0.0} KM/H";
                speedText.color = new Color(0.35f, 0.95f, 1f, 1f); // Neon Cyan
            }
            else if (isBraking)
            {
                speedText.text = $"\u2744 BRAKE {currentSpeed:0.0} KM/H";
                speedText.color = new Color(0.70f, 0.85f, 1f, 1f);
            }
            else
            {
                speedText.text = $"{currentSpeed:0.0} KM/H";
                speedText.color = Color.white;
            }
        }

        // Nitro Fill Bar
        if (playerController != null && nitroFillRT != null)
        {
            float nitroRatio = Mathf.Clamp01(playerController.Nitro / playerController.MaxNitro);
            nitroFillRT.anchorMax = new Vector2(nitroRatio, 1f);

            if (nitroFillImg != null)
            {
                if (playerController.IsNitroActive)
                {
                    nitroFillImg.color = new Color(0.1f, 0.95f, 1f, 1f); // Blazing Cyan
                }
                else if (nitroRatio < 0.25f)
                {
                    nitroFillImg.color = new Color(1f, 0.4f, 0.2f, 0.9f); // Low warning orange
                }
                else
                {
                    nitroFillImg.color = new Color(0.2f, 0.75f, 1f, 0.85f);
                }
            }
        }

        // 6. Track Progress Bar
        if (playerController != null && progressFillRT != null && progressIconRT != null)
        {
            float playerX = playerController.transform.position.x;
            float totalDist = Mathf.Max(1f, finishX - startX);
            float currentProgress = Mathf.Clamp01((playerX - startX) / totalDist);

            progressFillRT.anchorMax = new Vector2(currentProgress, 1f);

            // Icon position across bar (bar width 260)
            float iconX = -130f + (currentProgress * 260f);
            progressIconRT.anchoredPosition = new Vector2(iconX, 0);

            float remainingMeters = Mathf.Max(0f, finishX - playerX);
            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(currentProgress * 100)}% \u2022 {remainingMeters:0}m LEFT";
            }
        }

        // 7. Trick Banner Notification
        if (!string.IsNullOrEmpty(gm.LastTrickMessage) && gm.LastTrickMessage != currentTrickMessage)
        {
            currentTrickMessage = gm.LastTrickMessage;
            trickDisplayTimer = 2.4f;
            ShowTrickBanner(currentTrickMessage);
        }

        if (trickDisplayTimer > 0)
        {
            trickDisplayTimer -= Time.deltaTime;
            if (trickDisplayTimer <= 0)
            {
                HideTrickBanner();
            }
        }
    }

    private void ShowTrickBanner(string msg)
    {
        if (trickBannerObj != null && trickBannerText != null)
        {
            trickBannerText.text = $"\u2728  {msg.ToUpper()}  \u2728";
            trickBannerObj.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(AnimateTrickBannerIn());
        }
    }

    private void HideTrickBanner()
    {
        if (trickBannerObj != null)
        {
            StartCoroutine(AnimateTrickBannerOut());
        }
    }

    private IEnumerator AnimateTrickBannerIn()
    {
        trickBannerCg.alpha = 0f;
        trickBannerRT.localScale = Vector3.one * 0.7f;

        float duration = 0.22f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.65f) * 1.25f;
            trickBannerRT.localScale = Vector3.one * Mathf.Max(scale, 1.0f);
            trickBannerCg.alpha = Mathf.Clamp01(t * 1.5f);
            yield return null;
        }

        trickBannerRT.localScale = Vector3.one;
        trickBannerCg.alpha = 1f;
    }

    private IEnumerator AnimateTrickBannerOut()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            trickBannerCg.alpha = 1f - t;
            trickBannerRT.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.85f, t);
            yield return null;
        }

        trickBannerObj.SetActive(false);
        currentTrickMessage = "";
    }

    // ==========================================
    // PROCEDURAL MODERN IN-GAME HUD BUILDER
    // ==========================================
    private void BuildModernGameHUD()
    {
        // 1. Root Canvas
        GameObject canvasObj = new GameObject("ModernGameHUDCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 20;
        mainCanvas.pixelPerfect = true;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 2. Top-Left Player Status Card (Score, High Score, Lives, Combo)
        BuildTopLeftCard(canvasObj.transform, defaultFont);

        // 3. Top-Center Speedometer & Nitro Gauge
        BuildTopCenterSpeedometer(canvasObj.transform, defaultFont);

        // 4. Top-Center Trick Announcement Banner
        BuildTrickAnnouncementBanner(canvasObj.transform, defaultFont);

        // 5. Top-Right Track Progress Bar (Race Minimap)
        BuildTopRightProgressBar(canvasObj.transform, defaultFont);

        // 6. Top-Right Quick Pause Button
        BuildQuickPauseButton(canvasObj.transform, defaultFont);

        // 7. Pause Menu Modal Dialog
        BuildPauseModal(canvasObj.transform, defaultFont);
    }

    private void BuildTopLeftCard(Transform parent, Font font)
    {
        GameObject cardObj = new GameObject("TopLeftHUDCard");
        cardObj.transform.SetParent(parent, false);

        Image cardImg = cardObj.AddComponent<Image>();
        cardImg.sprite = UIProceduralSprites.GetCardBackground();
        cardImg.type = Image.Type.Sliced;

        RectTransform cardRT = cardObj.GetComponent<RectTransform>();
        cardRT.anchorMin = new Vector2(0, 1);
        cardRT.anchorMax = new Vector2(0, 1);
        cardRT.pivot = new Vector2(0, 1);
        cardRT.sizeDelta = new Vector2(340, 150);
        cardRT.anchoredPosition = new Vector2(25, -25);

        // Game Badge Subtitle
        GameObject badgeObj = new GameObject("Badge");
        badgeObj.transform.SetParent(cardObj.transform, false);
        Text badge = badgeObj.AddComponent<Text>();
        badge.font = font;
        badge.text = "\u2744  SNOW BOARDER";
        badge.fontSize = 13;
        badge.fontStyle = FontStyle.Bold;
        badge.color = new Color(0.40f, 0.88f, 1f, 0.95f);
        RectTransform badgeRT = badgeObj.GetComponent<RectTransform>();
        badgeRT.anchorMin = new Vector2(0, 1);
        badgeRT.anchorMax = new Vector2(0, 1);
        badgeRT.pivot = new Vector2(0, 1);
        badgeRT.sizeDelta = new Vector2(200, 20);
        badgeRT.anchoredPosition = new Vector2(18, -14);

        // Score Display
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(cardObj.transform, false);
        scoreText = scoreObj.AddComponent<Text>();
        scoreText.font = font;
        scoreText.text = "0 PTS";
        scoreText.fontSize = 32;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.color = Color.white;
        RectTransform scoreRT = scoreObj.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(0, 1);
        scoreRT.anchorMax = new Vector2(0, 1);
        scoreRT.pivot = new Vector2(0, 1);
        scoreRT.sizeDelta = new Vector2(260, 42);
        scoreRT.anchoredPosition = new Vector2(18, -36);

        Outline scoreOutline = scoreObj.AddComponent<Outline>();
        scoreOutline.effectColor = new Color(0.04f, 0.18f, 0.35f, 0.9f);
        scoreOutline.effectDistance = new Vector2(1.5f, -1.5f);

        // High Score Pill Tag
        GameObject hsObj = new GameObject("HighScoreTag");
        hsObj.transform.SetParent(cardObj.transform, false);
        highScoreText = hsObj.AddComponent<Text>();
        highScoreText.font = font;
        highScoreText.text = "\u2605 BEST: 0";
        highScoreText.fontSize = 16;
        highScoreText.fontStyle = FontStyle.Bold;
        highScoreText.color = new Color(1f, 0.85f, 0.25f, 1f);
        RectTransform hsRT = hsObj.GetComponent<RectTransform>();
        hsRT.anchorMin = new Vector2(0, 1);
        hsRT.anchorMax = new Vector2(0, 1);
        hsRT.pivot = new Vector2(0, 1);
        hsRT.sizeDelta = new Vector2(180, 26);
        hsRT.anchoredPosition = new Vector2(18, -82);

        // Lives Icons
        GameObject livesObj = new GameObject("LivesDisplay");
        livesObj.transform.SetParent(cardObj.transform, false);
        livesText = livesObj.AddComponent<Text>();
        livesText.font = font;
        livesText.text = "<color=#EF4444>\u2665 </color><color=#EF4444>\u2665 </color><color=#EF4444>\u2665</color>";
        livesText.fontSize = 24;
        livesText.supportRichText = true;
        livesText.fontStyle = FontStyle.Bold;
        RectTransform livesRT = livesObj.GetComponent<RectTransform>();
        livesRT.anchorMin = new Vector2(0, 1);
        livesRT.anchorMax = new Vector2(0, 1);
        livesRT.pivot = new Vector2(0, 1);
        livesRT.sizeDelta = new Vector2(160, 32);
        livesRT.anchoredPosition = new Vector2(18, -112);

        // Combo Multiplier Badge
        comboBadgeObj = new GameObject("ComboBadge");
        comboBadgeObj.transform.SetParent(cardObj.transform, false);
        Image comboImg = comboBadgeObj.AddComponent<Image>();
        comboImg.sprite = UIProceduralSprites.GetPillBadge();
        RectTransform comboRT = comboBadgeObj.GetComponent<RectTransform>();
        comboRT.anchorMin = new Vector2(1, 0);
        comboRT.anchorMax = new Vector2(1, 0);
        comboRT.pivot = new Vector2(1, 0);
        comboRT.sizeDelta = new Vector2(145, 34);
        comboRT.anchoredPosition = new Vector2(-15, 15);

        GameObject comboTxtObj = new GameObject("ComboText");
        comboTxtObj.transform.SetParent(comboBadgeObj.transform, false);
        comboBadgeText = comboTxtObj.AddComponent<Text>();
        comboBadgeText.font = font;
        comboBadgeText.text = "COMBO x2.0!";
        comboBadgeText.fontSize = 15;
        comboBadgeText.fontStyle = FontStyle.Bold;
        comboBadgeText.alignment = TextAnchor.MiddleCenter;
        comboBadgeText.color = new Color(0.35f, 0.95f, 1f, 1f);
        RectTransform comboTxtRT = comboTxtObj.GetComponent<RectTransform>();
        comboTxtRT.sizeDelta = new Vector2(145, 34);

        comboBadgeObj.SetActive(false);
    }

    private void BuildTopCenterSpeedometer(Transform parent, Font font)
    {
        speedBadgeObj = new GameObject("SpeedometerBadge");
        speedBadgeObj.transform.SetParent(parent, false);

        speedBadgeImg = speedBadgeObj.AddComponent<Image>();
        speedBadgeImg.sprite = UIProceduralSprites.GetPillBadge();

        RectTransform speedRT = speedBadgeObj.GetComponent<RectTransform>();
        speedRT.anchorMin = new Vector2(0.5f, 1);
        speedRT.anchorMax = new Vector2(0.5f, 1);
        speedRT.pivot = new Vector2(0.5f, 1);
        speedRT.sizeDelta = new Vector2(250, 48);
        speedRT.anchoredPosition = new Vector2(0, -25);

        GameObject speedTxtObj = new GameObject("SpeedText");
        speedTxtObj.transform.SetParent(speedBadgeObj.transform, false);
        speedText = speedTxtObj.AddComponent<Text>();
        speedText.font = font;
        speedText.text = "0.0 KM/H";
        speedText.fontSize = 20;
        speedText.fontStyle = FontStyle.Bold;
        speedText.alignment = TextAnchor.MiddleCenter;
        speedText.color = Color.white;

        RectTransform speedTxtRT = speedTxtObj.GetComponent<RectTransform>();
        speedTxtRT.sizeDelta = new Vector2(250, 48);

        Outline outline = speedTxtObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0.2f, 0.4f, 0.8f);
        outline.effectDistance = new Vector2(1, -1);

        // Nitro Energy Gauge Bar
        GameObject nitroRoot = new GameObject("NitroGaugeBar", typeof(RectTransform));
        nitroRoot.transform.SetParent(parent, false);
        RectTransform nitroRootRT = nitroRoot.GetComponent<RectTransform>();
        nitroRootRT.anchorMin = new Vector2(0.5f, 1);
        nitroRootRT.anchorMax = new Vector2(0.5f, 1);
        nitroRootRT.pivot = new Vector2(0.5f, 1);
        nitroRootRT.sizeDelta = new Vector2(230, 20);
        nitroRootRT.anchoredPosition = new Vector2(0, -78);

        // Nitro Background Bar
        GameObject nitroBg = new GameObject("NitroBg");
        nitroBg.transform.SetParent(nitroRoot.transform, false);
        Image bgImg = nitroBg.AddComponent<Image>();
        bgImg.sprite = UIProceduralSprites.GetSecondaryNormal();
        bgImg.type = Image.Type.Sliced;
        RectTransform bgRT = nitroBg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        // Nitro Fill
        GameObject nitroFill = new GameObject("NitroFill");
        nitroFill.transform.SetParent(nitroBg.transform, false);
        nitroFillImg = nitroFill.AddComponent<Image>();
        nitroFillImg.sprite = UIProceduralSprites.GetPrimaryNormal();
        nitroFillImg.type = Image.Type.Sliced;
        nitroFillRT = nitroFill.GetComponent<RectTransform>();
        nitroFillRT.anchorMin = Vector2.zero;
        nitroFillRT.anchorMax = new Vector2(1f, 1f);
        nitroFillRT.sizeDelta = Vector2.zero;

        // Nitro Label
        GameObject nitroLbl = new GameObject("NitroLabel");
        nitroLbl.transform.SetParent(nitroRoot.transform, false);
        nitroLabelText = nitroLbl.AddComponent<Text>();
        nitroLabelText.font = font;
        nitroLabelText.text = "\u26A1 NITRO BOOST (SHIFT / W)";
        nitroLabelText.fontSize = 12;
        nitroLabelText.fontStyle = FontStyle.Bold;
        nitroLabelText.alignment = TextAnchor.MiddleCenter;
        nitroLabelText.color = new Color(0.90f, 0.96f, 1f, 0.95f);
        RectTransform lblRT = nitroLbl.GetComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero;
        lblRT.anchorMax = Vector2.one;
        lblRT.sizeDelta = Vector2.zero;
    }

    private void BuildTrickAnnouncementBanner(Transform parent, Font font)
    {
        trickBannerObj = new GameObject("TrickAnnouncementBanner");
        trickBannerObj.transform.SetParent(parent, false);

        Image bannerImg = trickBannerObj.AddComponent<Image>();
        bannerImg.sprite = UIProceduralSprites.GetModalBackground();
        bannerImg.type = Image.Type.Sliced;

        trickBannerRT = trickBannerObj.GetComponent<RectTransform>();
        trickBannerRT.anchorMin = new Vector2(0.5f, 1);
        trickBannerRT.anchorMax = new Vector2(0.5f, 1);
        trickBannerRT.pivot = new Vector2(0.5f, 1);
        trickBannerRT.sizeDelta = new Vector2(600, 60);
        trickBannerRT.anchoredPosition = new Vector2(0, -104);

        trickBannerCg = trickBannerObj.AddComponent<CanvasGroup>();

        GameObject txtObj = new GameObject("TrickBannerText");
        txtObj.transform.SetParent(trickBannerObj.transform, false);
        trickBannerText = txtObj.AddComponent<Text>();
        trickBannerText.font = font;
        trickBannerText.text = "\u2728  360\u00B0 BACKFLIP! +200 PTS  \u2728";
        trickBannerText.fontSize = 22;
        trickBannerText.fontStyle = FontStyle.Bold;
        trickBannerText.alignment = TextAnchor.MiddleCenter;
        trickBannerText.color = new Color(1f, 0.88f, 0.25f, 1f);

        RectTransform txtRT = txtObj.GetComponent<RectTransform>();
        txtRT.sizeDelta = new Vector2(580, 56);

        Outline outline = txtObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.02f, 0.1f, 0.25f, 0.9f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        trickBannerObj.SetActive(false);
    }

    private void BuildTopRightProgressBar(Transform parent, Font font)
    {
        GameObject progressRoot = new GameObject("CourseProgressBar", typeof(RectTransform));
        progressRoot.transform.SetParent(parent, false);

        RectTransform rootRT = progressRoot.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(1, 1);
        rootRT.anchorMax = new Vector2(1, 1);
        rootRT.pivot = new Vector2(1, 1);
        rootRT.sizeDelta = new Vector2(340, 68);
        rootRT.anchoredPosition = new Vector2(-85, -25);

        // Background Bar
        GameObject bgBar = new GameObject("TrackBackground");
        bgBar.transform.SetParent(progressRoot.transform, false);
        Image bgImg = bgBar.AddComponent<Image>();
        bgImg.sprite = UIProceduralSprites.GetSecondaryNormal();
        bgImg.type = Image.Type.Sliced;
        RectTransform bgRT = bgBar.GetComponent<RectTransform>();
        bgRT.sizeDelta = new Vector2(270, 16);
        bgRT.anchoredPosition = new Vector2(0, -6);

        // Fill Bar
        GameObject fillBar = new GameObject("TrackFill");
        fillBar.transform.SetParent(bgBar.transform, false);
        Image fillImg = fillBar.AddComponent<Image>();
        fillImg.sprite = UIProceduralSprites.GetPrimaryNormal();
        fillImg.type = Image.Type.Sliced;
        progressFillRT = fillBar.GetComponent<RectTransform>();
        progressFillRT.anchorMin = Vector2.zero;
        progressFillRT.anchorMax = new Vector2(0f, 1f);
        progressFillRT.sizeDelta = Vector2.zero;

        // Moving Boarder Icon
        GameObject iconObj = new GameObject("RiderIcon");
        iconObj.transform.SetParent(bgBar.transform, false);
        Image iconImg = iconObj.AddComponent<Image>();
        iconImg.sprite = UIProceduralSprites.GetPillBadge();
        progressIconRT = iconObj.GetComponent<RectTransform>();
        progressIconRT.sizeDelta = new Vector2(22, 22);
        progressIconRT.anchoredPosition = new Vector2(-130f, 0);

        GameObject iconDot = new GameObject("Dot");
        iconDot.transform.SetParent(iconObj.transform, false);
        Image dotImg = iconDot.AddComponent<Image>();
        dotImg.sprite = UIProceduralSprites.GetSnowflake();
        dotImg.color = Color.white;
        RectTransform dotRT = iconDot.GetComponent<RectTransform>();
        dotRT.sizeDelta = new Vector2(14, 14);

        // Finish Flag Label
        GameObject finishObj = new GameObject("FinishFlag");
        finishObj.transform.SetParent(bgBar.transform, false);
        Text finishTxt = finishObj.AddComponent<Text>();
        finishTxt.font = font;
        finishTxt.text = "\uD83C\uDFC1"; // checkered flag
        finishTxt.fontSize = 18;
        finishTxt.alignment = TextAnchor.MiddleCenter;
        RectTransform finishRT = finishObj.GetComponent<RectTransform>();
        finishRT.sizeDelta = new Vector2(24, 24);
        finishRT.anchoredPosition = new Vector2(146f, 0);

        // Course Status Text
        GameObject statusObj = new GameObject("ProgressStatusText");
        statusObj.transform.SetParent(progressRoot.transform, false);
        progressText = statusObj.AddComponent<Text>();
        progressText.font = font;
        progressText.text = "0% \u2022 196m LEFT";
        progressText.fontSize = 13;
        progressText.fontStyle = FontStyle.Bold;
        progressText.alignment = TextAnchor.MiddleCenter;
        progressText.color = new Color(0.75f, 0.88f, 1f, 0.9f);
        RectTransform statusRT = statusObj.GetComponent<RectTransform>();
        statusRT.sizeDelta = new Vector2(270, 20);
        statusRT.anchoredPosition = new Vector2(0, 18);
    }

    private void BuildQuickPauseButton(Transform parent, Font font)
    {
        GameObject pauseBtnObj = new GameObject("QuickPauseButton", typeof(RectTransform));
        pauseBtnObj.transform.SetParent(parent, false);

        RectTransform rt = pauseBtnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.sizeDelta = new Vector2(50, 50);
        rt.anchoredPosition = new Vector2(-25, -25);

        Image img = pauseBtnObj.AddComponent<Image>();
        img.type = Image.Type.Sliced;

        GameObject txtObj = new GameObject("PauseIcon");
        txtObj.transform.SetParent(pauseBtnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = font;
        txt.text = "\u275A\u275A"; // pause bars
        txt.fontSize = 18;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        RectTransform txtRT = txtObj.GetComponent<RectTransform>();
        txtRT.sizeDelta = new Vector2(50, 50);

        ModernUIButton modernBtn = pauseBtnObj.AddComponent<ModernUIButton>();
        modernBtn.Setup(ModernUIButton.ButtonType.Secondary, "\u275A\u275A", TogglePause, false);
    }

    private void BuildPauseModal(Transform parent, Font font)
    {
        pauseModalObj = new GameObject("PauseModal");
        pauseModalObj.transform.SetParent(parent, false);

        Image overlayImg = pauseModalObj.AddComponent<Image>();
        overlayImg.color = new Color(0.02f, 0.05f, 0.12f, 0.88f);
        RectTransform overlayRT = pauseModalObj.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.sizeDelta = Vector2.zero;

        // Modal Center Card
        GameObject cardObj = new GameObject("PauseCard", typeof(RectTransform));
        cardObj.transform.SetParent(pauseModalObj.transform, false);
        Image cardImg = cardObj.AddComponent<Image>();
        cardImg.sprite = UIProceduralSprites.GetModalBackground();
        cardImg.type = Image.Type.Sliced;

        RectTransform cardRT = cardObj.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(580, 660);
        cardRT.anchoredPosition = Vector2.zero;

        // Header Title
        GameObject headerObj = new GameObject("PauseHeader", typeof(RectTransform));
        headerObj.transform.SetParent(cardObj.transform, false);
        Text headerTxt = headerObj.AddComponent<Text>();
        headerTxt.font = font;
        headerTxt.text = "\u2744  GAME PAUSED  \u2744";
        headerTxt.fontSize = 34;
        headerTxt.fontStyle = FontStyle.Bold;
        headerTxt.alignment = TextAnchor.MiddleCenter;
        headerTxt.color = new Color(0.40f, 0.88f, 1f, 1f);
        headerTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        headerTxt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform headerRT = headerObj.GetComponent<RectTransform>();
        headerRT.sizeDelta = new Vector2(500, 48);
        headerRT.anchoredPosition = new Vector2(0, 255);

        // Volume Label & Slider
        GameObject volLblObj = new GameObject("VolumeLabel", typeof(RectTransform));
        volLblObj.transform.SetParent(cardObj.transform, false);
        Text volLbl = volLblObj.AddComponent<Text>();
        volLbl.font = font;
        volLbl.text = "AUDIO VOLUME";
        volLbl.fontSize = 22;
        volLbl.fontStyle = FontStyle.Bold;
        volLbl.alignment = TextAnchor.MiddleLeft;
        volLbl.color = new Color(0.85f, 0.93f, 1f, 1f);
        volLbl.horizontalOverflow = HorizontalWrapMode.Overflow;
        volLbl.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform volLblRT = volLblObj.GetComponent<RectTransform>();
        volLblRT.sizeDelta = new Vector2(460, 32);
        volLblRT.anchoredPosition = new Vector2(0, 185);

        GameObject volValObj = new GameObject("VolumeValText", typeof(RectTransform));
        volValObj.transform.SetParent(cardObj.transform, false);
        pauseVolumeText = volValObj.AddComponent<Text>();
        pauseVolumeText.font = font;
        pauseVolumeText.text = $"{Mathf.RoundToInt(AudioListener.volume * 100)}%";
        pauseVolumeText.fontSize = 24;
        pauseVolumeText.fontStyle = FontStyle.Bold;
        pauseVolumeText.alignment = TextAnchor.MiddleRight;
        pauseVolumeText.color = new Color(0.35f, 0.85f, 1f, 1f);
        pauseVolumeText.horizontalOverflow = HorizontalWrapMode.Overflow;
        pauseVolumeText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform volValRT = volValObj.GetComponent<RectTransform>();
        volValRT.sizeDelta = new Vector2(460, 32);
        volValRT.anchoredPosition = new Vector2(0, 185);

        CreatePauseVolumeSlider(cardObj.transform, new Vector2(0, 135));

        // Buttons
        CreateModernButton(
            cardObj.transform, font, "RESUME RUN",
            ModernUIButton.ButtonType.Primary,
            new Vector2(380, 64), new Vector2(0, 40),
            ResumeGame, true, 26
        );

        CreateModernButton(
            cardObj.transform, font, "RESTART RUN",
            ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -35),
            RestartRun, false, 22
        );

        CreateModernButton(
            cardObj.transform, font, "MAIN MENU",
            ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -100),
            ReturnToMainMenu, false, 22
        );

        CreateModernButton(
            cardObj.transform, font, "QUIT TO DESKTOP",
            ModernUIButton.ButtonType.Secondary,
            new Vector2(350, 56), new Vector2(0, -165),
            QuitGame, false, 22
        );

        pauseModalObj.SetActive(false);
    }

    private void CreatePauseVolumeSlider(Transform parent, Vector2 pos)
    {
        GameObject sliderObj = new GameObject("PauseVolumeSlider", typeof(RectTransform));
        sliderObj.transform.SetParent(parent, false);
        RectTransform sliderRT = sliderObj.GetComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(460, 32);
        sliderRT.anchoredPosition = pos;

        pauseVolumeSlider = sliderObj.AddComponent<Slider>();
        pauseVolumeSlider.minValue = 0f;
        pauseVolumeSlider.maxValue = 1f;
        pauseVolumeSlider.value = AudioListener.volume;

        // Background Track
        GameObject trackObj = new GameObject("BackgroundTrack", typeof(RectTransform));
        trackObj.transform.SetParent(sliderObj.transform, false);
        Image trackImg = trackObj.AddComponent<Image>();
        trackImg.sprite = UIProceduralSprites.GetSliderTrack();
        trackImg.type = Image.Type.Sliced;
        RectTransform trackRT = trackObj.GetComponent<RectTransform>();
        trackRT.anchorMin = new Vector2(0, 0.28f);
        trackRT.anchorMax = new Vector2(1, 0.72f);
        trackRT.offsetMin = Vector2.zero;
        trackRT.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillArea.GetComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0.28f);
        fillAreaRT.anchorMax = new Vector2(1, 0.72f);
        fillAreaRT.offsetMin = new Vector2(6f, 0f);
        fillAreaRT.offsetMax = new Vector2(-6f, 0f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.sprite = UIProceduralSprites.GetSliderFill();
        fillImg.type = Image.Type.Sliced;
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0f, 0f);
        fillRT.anchorMax = new Vector2(0f, 1f);
        fillRT.sizeDelta = new Vector2(10f, 0f);
        pauseVolumeSlider.fillRect = fillRT;

        // Handle Area
        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRT = handleArea.GetComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.offsetMin = new Vector2(16f, 0f);
        handleAreaRT.offsetMax = new Vector2(-16f, 0f);

        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.sprite = UIProceduralSprites.GetSliderKnob();
        RectTransform handleRT = handle.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(32, 32);
        pauseVolumeSlider.handleRect = handleRT;

        pauseVolumeSlider.targetGraphic = handleImg;
        pauseVolumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
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
