using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDetector : MonoBehaviour
{
    [Header("Fall Bounds Settings")]
    [Tooltip("Y position threshold below which the player is considered fallen out of bounds.")]
    [SerializeField] private float fallThresholdY = -45f;
    [SerializeField] private float loadDelay = 1.0f;

    private bool hasTriggered = false;
    public bool HasTriggered => hasTriggered;

    public float FallThresholdY
    {
        get => fallThresholdY;
        set => fallThresholdY = value;
    }

    void Update()
    {
        if (hasTriggered) return;

        // Suppress fall if level is already completed or finish line has triggered
        if (FinishLine.IsTransitioning) return;
        if (GameManager.Instance != null && (GameManager.Instance.IsGameFinished || GameManager.Instance.PlayerWon)) return;

        // If player falls below the bottom of the map
        if (transform.position.y < fallThresholdY)
        {
            TriggerFallGameOver();
        }
    }

    public void TriggerFallGameOver()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        // Disable player controls
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            player.DisableControls();
        }

        // Cancel any pending crash reload so it doesn't conflict
        CrashDetector crashDetector = GetComponent<CrashDetector>();
        if (crashDetector != null)
        {
            crashDetector.CancelCrash();
            crashDetector.enabled = false;
        }

        // Trigger Game Over in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerFallOutOfBounds();
        }

        Invoke(nameof(LoadEndScene), loadDelay);
    }

    void LoadEndScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndScene");
    }

    void OnDisable()
    {
        CancelInvoke(nameof(LoadEndScene));
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(LoadEndScene));
    }
}
