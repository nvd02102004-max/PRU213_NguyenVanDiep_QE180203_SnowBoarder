using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [Header("Finish Settings")]
    [SerializeField] float loadDelay = 1.8f;
    [SerializeField] ParticleSystem finishEffect;

    private bool hasFinished = false;
    public bool HasFinished => hasFinished;

    public static bool IsTransitioning { get; private set; } = false;

    void Awake()
    {
        hasFinished = false;
        IsTransitioning = false;
    }

    void OnDisable()
    {
        CancelInvoke(nameof(ReloadScene));
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(ReloadScene));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasFinished || IsTransitioning) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponent<PlayerController>();
        }
        if (player == null && (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player"))))
        {
#if UNITY_2023_1_OR_NEWER
            player = FindAnyObjectByType<PlayerController>();
#else
            player = FindObjectOfType<PlayerController>();
#endif
        }

        if (player != null)
        {
            hasFinished = true;
            IsTransitioning = true;

            // Immediately cancel and disable CrashDetector so victory cannot be overridden by a crash
            CrashDetector crashDetector = player.GetComponent<CrashDetector>();
            if (crashDetector != null)
            {
                crashDetector.CancelCrash();
                crashDetector.enabled = false;
            }

            // Immediately disable FallDetector so victory cannot be overridden by falling after finish
            FallDetector fallDetector = player.GetComponent<FallDetector>();
            if (fallDetector != null)
            {
                fallDetector.enabled = false;
            }

            // Gently brake the player
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
#if UNITY_6000_0_OR_NEWER
                rb.linearDamping = 3.0f;
#else
                rb.drag = 3.0f;
#endif
            }

            if (finishEffect != null)
            {
                finishEffect.Play();
            }

            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }

            player.DisableControls();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelCompleted();
            }

            Invoke(nameof(ReloadScene), loadDelay);
        }
    }

    void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndScene");
    }
}
