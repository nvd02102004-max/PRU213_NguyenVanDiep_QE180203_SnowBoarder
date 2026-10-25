using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_2023_1_OR_NEWER
                instance = FindAnyObjectByType<GameManager>();
#else
                instance = FindObjectOfType<GameManager>();
#endif
                if (instance == null)
                {
                    GameObject gmObj = new GameObject("GameManager");
                    instance = gmObj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    [Header("Game State")]
    [SerializeField] int initialLives = 3;
    [SerializeField] int baseTrickPoints = 100;

    public int Score { get; private set; } = 0;
    public int HighScore { get; private set; } = 0;
    public int Lives { get; private set; } = 3;
    public int CurrentCombo { get; private set; } = 0;
    public float Multiplier { get; private set; } = 1.0f;
    public bool IsGameOver { get; private set; } = false;
    public bool IsGameFinished { get; private set; } = false;
    public bool PlayerWon { get; private set; } = false;

    public string LastTrickMessage { get; private set; } = "";
    float trickMessageTimer = 0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt("SnowBoarder_HighScore", 0);
        Lives = initialLives;
    }

    void Update()
    {
        if (trickMessageTimer > 0)
        {
            trickMessageTimer -= Time.deltaTime;
            if (trickMessageTimer <= 0)
            {
                LastTrickMessage = "";
            }
        }
    }

    public void ResetRunState()
    {
        Score = 0;
        CurrentCombo = 0;
        Multiplier = 1.0f;
        Lives = initialLives;
        IsGameOver = false;
        IsGameFinished = false;
        PlayerWon = false;
        LastTrickMessage = "";
        Time.timeScale = 1f;
    }

    public void ResetHighScore()
    {
        HighScore = 0;
        PlayerPrefs.DeleteKey("SnowBoarder_HighScore");
        PlayerPrefs.Save();
    }

    public void AddScore(int points)
    {
        int finalPoints = Mathf.RoundToInt(points * Multiplier);
        Score += finalPoints;

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("SnowBoarder_HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }

    public void AddTrick(string trickName, int flipsCount, float qualityMultiplier = 1.0f)
    {
        CurrentCombo++;
        Multiplier = 1.0f + (CurrentCombo * 0.5f);

        int points = Mathf.RoundToInt(baseTrickPoints * flipsCount * qualityMultiplier);
        int earned = Mathf.RoundToInt(points * Multiplier);
        Score += earned;

        LastTrickMessage = $"{trickName} x{Multiplier:0.0}! +{earned} pts";
        trickMessageTimer = 2.5f;

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("SnowBoarder_HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }

    public void OnPlayerCrash()
    {
        CurrentCombo = 0;
        Multiplier = 1.0f;
        Lives--;

        if (Lives <= 0)
        {
            IsGameOver = true;
            PlayerWon = false;
            PlayerPrefs.SetInt("SnowBoarder_LastScore", Score);
            PlayerPrefs.SetInt("SnowBoarder_PlayerWon", 0);
            PlayerPrefs.Save();
        }
    }

    public void OnPlayerFallOutOfBounds()
    {
        CurrentCombo = 0;
        Multiplier = 1.0f;
        Lives = 0;
        IsGameOver = true;
        PlayerWon = false;
        PlayerPrefs.SetInt("SnowBoarder_LastScore", Score);
        PlayerPrefs.SetInt("SnowBoarder_PlayerWon", 0);
        PlayerPrefs.Save();
    }

    public void OnLevelCompleted()
    {
        IsGameFinished = true;
        PlayerWon = true;
        AddScore(500); // Level completion bonus
        PlayerPrefs.SetInt("SnowBoarder_LastScore", Score);
        PlayerPrefs.SetInt("SnowBoarder_PlayerWon", 1);
        PlayerPrefs.Save();
    }

    public void RestartGame()
    {
        ResetRunState();
        SceneManager.LoadScene("Level1");
    }
}
