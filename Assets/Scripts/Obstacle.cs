using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] bool causesCrash = false;
    [SerializeField] float speedPenaltyMultiplier = 0.5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (causesCrash)
            {
                CrashDetector crash = collision.gameObject.GetComponent<CrashDetector>();
                if (crash != null)
                {
                    // Trigger crash via crash detector or disable
                }
            }
            else
            {
                // Slow down player on minor collision
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
#if UNITY_6000_0_OR_NEWER
                    rb.linearVelocity *= speedPenaltyMultiplier;
#else
                    rb.velocity *= speedPenaltyMultiplier;
#endif
                }
            }
        }
    }
}
