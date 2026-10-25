using UnityEngine;

public class SnowflakeItem : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] int pointValue = 50;
    [SerializeField] float rotateSpeed = 65f;
    [SerializeField] float floatAmplitude = 0.25f;
    [SerializeField] float floatFrequency = 2.4f;
    [SerializeField] AudioClip pickupSFX;

    Vector3 initialPosition;
    bool isCollected = false;

    public void SetPointValue(int value)
    {
        pointValue = value;
    }

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Gentle rotation
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        // Gentle floating animation
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency + initialPosition.x) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null || other.CompareTag("Player"))
        {
            isCollected = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(pointValue);
            }

            if (player != null)
            {
                player.AddNitro(20f);
            }

            // Floating score popup
            FloatingScorePopup.Create(transform.position, $"+{pointValue}", new Color(0.40f, 0.95f, 1f, 1f));

            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayPop();
            }

            // Spawn pickup burst particles
            SpawnPickupEffect();

            Destroy(gameObject);
        }
    }

    void SpawnPickupEffect()
    {
        GameObject burstObj = new GameObject("SnowflakeBurst");
        burstObj.transform.position = transform.position;

        ParticleSystem ps = burstObj.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ParticleSystemRenderer psr = burstObj.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            psr.material = UIProceduralSprites.GetParticleMaterial();
        }

        var main = ps.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = 0.5f;
        main.startSpeed = 3f;
        main.startSize = 0.12f;
        main.startColor = new Color(0.7f, 0.95f, 1f, 0.9f);
        main.maxParticles = 20;

        var emission = ps.emission;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        ps.Play();

        Destroy(burstObj, 0.8f);
    }
}
