using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [Header("Crash Settings")]
    [SerializeField] float loadDelay = 1.0f;
    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] AudioClip crashSFX;

    private bool hasCrashed = false;
    private bool isReloading = false;

    public bool HasCrashed => hasCrashed;

    void OnDisable()
    {
        CancelInvoke(nameof(ReloadScene));
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(ReloadScene));
    }

    public void CancelCrash()
    {
        hasCrashed = false;
        isReloading = false;
        CancelInvoke(nameof(ReloadScene));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Suppress crash if level is already completed or finish line has triggered
        if (FinishLine.IsTransitioning) return;
        if (GameManager.Instance != null && (GameManager.Instance.IsGameFinished || GameManager.Instance.PlayerWon)) return;

        if (other.CompareTag("Ground") && !hasCrashed && !isReloading)
        {
            hasCrashed = true;

            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.DisableControls();
            }

            if (crashEffect != null)
            {
                crashEffect.Play();
            }

            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null && crashSFX != null)
            {
                audioSource.PlayOneShot(crashSFX);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerCrash();
            }

            Invoke(nameof(ReloadScene), loadDelay);
        }
    }

    void ReloadScene()
    {
        if (isReloading) return;
        isReloading = true;

        // Ensure time scale is unpaused
        Time.timeScale = 1f;

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            SceneManager.LoadScene("EndScene");
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
