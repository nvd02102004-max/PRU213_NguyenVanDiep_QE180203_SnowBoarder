using UnityEngine;

public class TrickDetector : MonoBehaviour
{
    private DustTrail dustTrail;
    private CrashDetector crashDetector;
    private PlayerController playerController;

    private float previousAngle = 0f;
    private float accumulatedRotation = 0f;
    private int flipsCompleted = 0;
    private bool wasGrounded = true;

    void Start()
    {
        dustTrail = GetComponent<DustTrail>();
        crashDetector = GetComponent<CrashDetector>();
        playerController = GetComponent<PlayerController>();
        previousAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        bool isGrounded = dustTrail != null && dustTrail.IsGrounded;
        bool hasCrashed = crashDetector != null && crashDetector.HasCrashed;

        if (hasCrashed)
        {
            accumulatedRotation = 0f;
            flipsCompleted = 0;
            return;
        }

        if (!isGrounded)
        {
            float currentAngle = transform.eulerAngles.z;
            float delta = Mathf.DeltaAngle(previousAngle, currentAngle);
            accumulatedRotation += delta;
            previousAngle = currentAngle;

            // 330-degree threshold for forgiving 360-degree rotation detection
            if (Mathf.Abs(accumulatedRotation) >= 330f)
            {
                flipsCompleted++;
                accumulatedRotation -= Mathf.Sign(accumulatedRotation) * 360f;

                // Mid-air audio chirp for every flip completed
                if (UISoundManager.Instance != null)
                {
                    UISoundManager.Instance.PlayHover();
                }
            }
        }
        else
        {
            previousAngle = transform.eulerAngles.z;

            // Just touched down on ground
            if (!wasGrounded)
            {
                EvaluateLanding(flipsCompleted);
                flipsCompleted = 0;
                accumulatedRotation = 0f;
            }
        }

        wasGrounded = isGrounded;
    }

    private void EvaluateLanding(int flips)
    {
        // If already crashed on this landing, suppress trick score
        if (crashDetector != null && crashDetector.HasCrashed) return;

        // Detect ground surface normal using raycast
        Vector2 groundNormal = Vector2.up;
        int groundLayerMask = LayerMask.GetMask("Ground");
        if (groundLayerMask == 0) groundLayerMask = ~0; // fallback all layers if layer mask not configured

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3.0f, groundLayerMask);
        if (hit.collider == null)
        {
            hit = Physics2D.Raycast(transform.position, -transform.up, 3.0f, groundLayerMask);
        }

        if (hit.collider != null)
        {
            groundNormal = hit.normal;
        }

        // Angle between player's upward orientation and ground normal
        float angleDiff = Vector2.Angle(transform.up, groundNormal);

        string qualityLabel;
        float qualityMultiplier;
        Color qualityColor;

        if (angleDiff <= 18f)
        {
            // PERFECT LANDING: clean angle with slope
            qualityLabel = "★ PERFECT LANDING! ★";
            qualityMultiplier = 1.5f;
            qualityColor = new Color(0.20f, 0.95f, 0.45f, 1f); // Vibrant Emerald

            if (playerController != null)
            {
                playerController.AddNitro(35f);
            }

            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayPop();
            }
        }
        else if (angleDiff <= 40f)
        {
            // GOOD LANDING: acceptable board alignment
            qualityLabel = "GOOD LANDING!";
            qualityMultiplier = 1.0f;
            qualityColor = new Color(0.35f, 0.88f, 1f, 1f); // Ice Cyan

            if (playerController != null)
            {
                playerController.AddNitro(15f);
            }
        }
        else
        {
            // ROUGH LANDING: board landed awkwardly without head crash
            qualityLabel = "ROUGH LANDING";
            qualityMultiplier = 0.7f;
            qualityColor = new Color(1f, 0.65f, 0.2f, 1f); // Warning Amber
        }

        if (flips > 0)
        {
            string trickName = flips == 1 ? "360° Spin" :
                              flips == 2 ? "720° Double Spin" :
                              $"{flips * 360}° Mega Spin";

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddTrick($"{qualityLabel} {trickName}", flips, qualityMultiplier);
            }

            FloatingScorePopup.Create(transform.position + Vector3.up * 1.6f, $"{qualityLabel}\n{trickName}!", qualityColor);
        }
        else
        {
            // Jumped high without full 360 flip
            if (angleDiff <= 18f)
            {
                FloatingScorePopup.Create(transform.position + Vector3.up * 1.2f, qualityLabel, qualityColor);
            }
        }
    }
}
