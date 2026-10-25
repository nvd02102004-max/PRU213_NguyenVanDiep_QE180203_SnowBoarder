using UnityEngine;

public class DustTrail : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] ParticleSystem dustParticles;

    public bool IsGrounded { get; private set; } = false;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
            if (dustParticles != null)
            {
                dustParticles.Play();
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
            if (dustParticles != null)
            {
                dustParticles.Stop();
            }
        }
    }
}
